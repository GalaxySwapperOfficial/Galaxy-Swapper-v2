using Galaxy_Swapper_v2.Workspace.Generation.Formats;
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
        public static JArray Cache { get; set; } = default!;
        public static readonly string Path = $"{Config.Path}\\UEFN.json";
        public static void Initialize()
        {
            try
            {
                if (Cache != null)
                    return;

                if (!File.Exists(Path))
                {
                    Log.Information($"{Path} Does not exist populating cache with empty array");
                    Cache = new JArray();
                    return;
                }

                string Content = File.ReadAllText(Path);

                if (!Content.ValidArray())
                {
                    Log.Information($"{Path} Is not in a valid json format populating cache with empty array");
                    Cache = new JArray();
                    return;
                }

                Cache = JArray.Parse(Content);

                Log.Information("Successfully initialized uefn");
            }
            catch (Exception Exception)
            {
                Log.Error(Exception.Message, $"Caught a error while initializing uefn cache loading with empty array");
                Cache = new JArray();
            }
        }

        public static void Add(string paks, string name, Downloadable downloadable)
        {
            Remove(name);

            int count = 100;
            string filePath;

            while (true)
            {
                filePath = $"{paks}\\pakchunk{count}-WindowsClient";

                if (!File.Exists($"{filePath}.ucas") && !File.Exists($"{filePath}.utoc") &&
                    !File.Exists($"{filePath}.pak") && !File.Exists($"{filePath}.sig"))
                    break;

                count++;
            }

            Log.Information($"Found a open slot: {filePath}");

            Download($"{filePath}.pak", downloadable.Pak);
            Download($"{filePath}.sig", downloadable.Sig);
            Download($"{filePath}.ucas", downloadable.Ucas);
            Download($"{filePath}.utoc", downloadable.Utoc);

            var newobject = JObject.FromObject(new
            {
                Name = name,
                pak = $"{filePath}.pak",
                sig = $"{filePath}.sig",
                ucas = $"{filePath}.ucas",
                utoc = $"{filePath}.utoc",
                backup = $"{filePath}.backup"
            });

            Cache.Add(newobject);
            File.WriteAllText(Path, Cache.ToString());
        }

        public static void Remove(string name)
        {
            var NewCache = new JArray();
            foreach (var uefn in Cache)
            {
                if (uefn["Name"].Value<string>() == name)
                {
                    string[] files = { "pak", "sig", "ucas", "utoc", "backup" };
                    foreach (string file in files)
                    {
                        Delete(uefn[file].Value<string>());
                    }
                }
                else
                    NewCache.Add(uefn);
            }

            Cache = NewCache;
            File.WriteAllText(Path, Cache.ToString());
        }

        public static void Clear()
        {
            foreach (var uefn in Cache)
            {
                string[] files = { "pak", "sig", "ucas", "utoc", "backup" };
                foreach (string file in files)
                {
                    Delete(uefn[file].Value<string>());
                }
            }

            var NewCache = new JArray();
            Cache = NewCache;

            File.WriteAllText(Path, Cache.ToString());
        }

        private static bool Delete(string path)
        {
            if (File.Exists(path))
            {
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
                    WC.DownloadFile(url, path);
                    WC.Dispose();

                    Log.Information($"Downloaded data from {url} to {path}");
                }
                catch (Exception Exception)
                {
                    Message.DisplaySTA("Error", "Webclient caught a exception while downloading custom UEFN game file!", MessageBoxButton.OK, solutions: new List<string> { "Disable Windows Defender Firewall", "Disable any anti-virus softwares", "Turn on a VPN" });
                    Log.Error(Exception, $"Failed to download from: {url}");
                    throw new CustomException("Failed to download custom UEFN game file!");
                }
            }
        }
    }
}