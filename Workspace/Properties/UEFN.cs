using Galaxy_Swapper_v2.Workspace.Generation.Formats;
using Galaxy_Swapper_v2.Workspace.Swapping.Providers;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using static Galaxy_Swapper_v2.Workspace.Global;

namespace Galaxy_Swapper_v2.Workspace.Properties
{
    public static class UEFN
    {
        public static JObject Cache { get; set; } = default!;
        public static readonly string Path = $"{App.Config}\\UEFN.json";
        public static readonly string[] Extensions = { ".ucas", ".utoc", ".pak", ".sig" };
        public static void Initialize()
        {
            if (Cache != null)
                return;

            if (!File.Exists(Path))
            {
                Log.Information($"{Path} Does not exist populating UEFN cache with empty object");
                Reset();
                return;
            }

            string Content = File.ReadAllText(Path);

            if (!Content.ValidJson())
            {
                Log.Information($"{Path} Is not in a valid json format populating UEFN cache with empty object");
                Reset();
                return;
            }

            Cache = JObject.Parse(Content);

            Log.Information("Successfully initialized UEFN");
        }

        public static void DownloadMain(string paks)
        {
            var Parse = Endpoint.Read(Endpoint.Type.UEFN);

            if (!Parse["Enabled"].Value<bool>())
            {
                Log.Warning("Main UEFN is disabled!");
                return;
            }

            if (Cache["Main"].KeyIsNullOrEmpty() || Cache["Version"].KeyIsNullOrEmpty())
                Log.Information("UEFN cache key 'Main' and 'Version' is null.");
            else if (Parse["Version"].Value<int>() != Cache["Version"].Value<int>())
            {
                Log.Information("Main UEFN game file is out of date.");
                foreach (string extension in Extensions)
                {
                    Delete($"{Cache["Main"].Value<string>()}{extension}");
                }
                Delete($"{Cache["Main"].Value<string>()}.backup"); //Might exist!
            }
            else if (!File.Exists($"{Cache["Main"].Value<string>()}.ucas") || !File.Exists($"{Cache["Main"].Value<string>()}.utoc") || !File.Exists($"{Cache["Main"].Value<string>()}.pak") || !File.Exists($"{Cache["Main"].Value<string>()}.sig"))
                Log.Warning("Main UEFN game file is cached but does not exist?");
            else
            {
                Log.Information($"Main UEFN pakchunk is already downloaded.");
                return;
            }

            if (!FindSlot(paks, out string slot))
            {
                Log.Error($"Failed to find open slot for main UEFN game file.");
                throw new CustomException($"Failed to find a open slot for main UEFN game file!\nPlease revert a revert a item you have swapped to make space for this.");
            }

            Delete($"{paks}\\{slot}.backup"); //Just in case there is a left over .backup

            Download($"{paks}\\{slot}.ucas", Parse["Main"]["ucas"].Value<string>());
            Download($"{paks}\\{slot}.utoc", Parse["Main"]["utoc"].Value<string>());
            Download($"{paks}\\{slot}.pak", Parse["Main"]["pak"].Value<string>());
            Download($"{paks}\\{slot}.sig", Parse["Main"]["sig"].Value<string>());
            Download($"{paks}\\{slot}.backup", Parse["Main"]["utoc"].Value<string>());

            CProvider.Add(slot);

            Cache["Main"] = $"{paks}\\{slot}";
            Cache["Version"] = Parse["Version"].Value<int>();

            File.WriteAllText(Path, Cache.ToString());
            Log.Information($"Wrote UEFN cache to {Path}");
        }

        public static void Add(string paks, string name, Downloadable downloadable)
        {
            Remove(name);

            if (!FindSlot(paks, out string slot))
            {
                Log.Error($"Failed to find open slot for UEFN game file.");
                throw new CustomException($"Failed to find a open slot for UEFN game file!\nPlease revert a revert a item you have swapped to make space for this.");
            }

            Delete($"{paks}\\{slot}.backup"); //Just in case there is a left over .backup

            Download($"{paks}\\{slot}.ucas", downloadable.Ucas);
            Download($"{paks}\\{slot}.utoc", downloadable.Utoc);
            Download($"{paks}\\{slot}.pak", downloadable.Pak);
            Download($"{paks}\\{slot}.sig", downloadable.Sig);
            Download($"{paks}\\{slot}.backup", downloadable.Utoc);

            CProvider.Add(slot);

            var newobject = JObject.FromObject(new
            {
                Name = name,
                Pakchunk = $"{paks}\\{slot}"
            });

            (Cache["Externals"] as JArray).Add(newobject);
            File.WriteAllText(Path, Cache.ToString());
            Log.Information($"Wrote UEFN cache to {Path}");
        }

        public static void Remove(string name)
        {
            if (Cache["Externals"] == null)
                return;

            var Externals = new JArray();
            foreach (var external in Cache["Externals"])
            {
                if (external["Name"].Value<string>() == name)
                {
                    foreach (string extension in Extensions)
                    {
                        Delete($"{external["Pakchunk"].Value<string>()}{extension}");
                    }
                    Delete($"{external["Pakchunk"].Value<string>()}.backup");//Might exist!
                }
                else
                    Externals.Add(external);
            }

            Cache["Externals"] = Externals;
            File.WriteAllText(Path, Cache.ToString());
            Log.Information($"Wrote UEFN cache to {Path}");
        }

        public static void Clear()
        {
            foreach (var external in Cache["Externals"])
            {
                Log.Warning($"{external["Pakchunk"].Value<string>()}");
                foreach (string extension in Extensions)
                {
                    Log.Warning($"{external["Pakchunk"].Value<string>()}{extension}");
                    Delete($"{external["Pakchunk"].Value<string>()}{extension}");
                }
                Delete($"{external["Pakchunk"].Value<string>()}.backup");//Might exist!
            }

            if (!Cache["Main"].KeyIsNullOrEmpty())
            {
                foreach (string extension in Extensions)
                {
                    Log.Warning($"{Cache["Main"].Value<string>()}{extension}");
                    Delete($"{Cache["Main"].Value<string>()}{extension}");
                }
                Delete($"{Cache["Main"].Value<string>()}.backup");//Might exist!
            }

            Reset();
            File.WriteAllText(Path, Cache.ToString());
        }

        private static bool FindSlot(string paks, out string available)
        {
            available = null;

            var Parse = Endpoint.Read(Endpoint.Type.UEFN);
            foreach (string slot in Parse["Slots"])
            {
                if (!File.Exists($"{paks}\\{slot}.ucas") || !File.Exists($"{paks}\\{slot}.utoc") || !File.Exists($"{paks}\\{slot}.pak") || !File.Exists($"{paks}\\{slot}.sig"))
                {
                    Log.Information($"Found open slot: {slot}");
                    available = slot;
                    return true;
                }
            }

            Log.Error("Failed to find open slot!");
            return false;
        }

        private static bool Delete(string path)
        {
            if (File.Exists(path))
            {
                CProvider.CloseStream(path);

                try
                {
                    Log.Information($"Deleting old uefn file: {path}");
                    File.Delete(path);
                }
                catch (Exception Exception)
                {
                    Log.Error(Exception, $"Failed to delete old uefn file: {path}");
                    return false;
                }
            }
            return true;
        }

        private static void Download(string path, string url)
        {
            using (WebClient WC = new WebClient())
            {
                try
                {
                    Delete(path);

                    WC.DownloadFile(url, path);
                    WC.Dispose();

                    Log.Information($"Downloaded data from {url} to {path}");
                }
                catch (IOException ioException)
                {
                    Message.DisplaySTA("Error", "Failed to download UEFN game file. There is not enough space on the disk!", MessageBoxButton.OK, solutions: new List<string> { "Make room on the disk and try again." });
                    Log.Error(ioException, $"Failed to download from: {url} disk ran out of space!");
                    throw new CustomException("Failed to download custom UEFN game file! Ran out of space.");
                }
                catch (Exception Exception)
                {
                    Message.DisplaySTA("Error", "Webclient caught a exception while downloading custom UEFN game file!", MessageBoxButton.OK, solutions: new List<string> { "Disable Windows Defender Firewall", "Disable any anti-virus softwares", "Turn on a VPN" });
                    Log.Error(Exception, $"Failed to download from: {url}");
                    throw new CustomException("Failed to download custom UEFN game file!");
                }
            }
        }

        private static void Reset()
        {
            Cache = JObject.FromObject(new
            {
                Main = JValue.CreateNull(),
                Version = JValue.CreateNull(),
                Externals = new JArray()
            });
        }
    }
}