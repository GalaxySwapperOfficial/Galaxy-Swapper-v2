using Galaxy_Swapper_v2.Workspace.Generation.Formats;
using Galaxy_Swapper_v2.Workspace.Swapping.Providers;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using static Galaxy_Swapper_v2.Workspace.Global;

namespace Galaxy_Swapper_v2.Workspace.Properties
{
    public static class UEFN
    {
        public static JObject Cache { get; set; } = default!;
        public static readonly string Path = $"{App.Config}\\UEFN.json";
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

        private static readonly byte[] UEFNMountPoint = Encoding.ASCII.GetBytes("/FortniteGame/Plugins/GameFeatures/");
        public static void OpenSlots(string paks)
        {
            var Parse = Endpoint.Read(Endpoint.Type.UEFN);
            foreach (string slot in Parse["Slots"])
            {
                string path = $"{paks}\\{slot}.pak";
                if (File.Exists(path))
                {
                    Log.Information($"Slot {slot} is open checking if stream is as well");
                    CProvider.CloseStream($"{paks}\\{slot}.pak");

                    Log.Information("Checking if slot is a high resolution texture game file");
                    using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
                    {
                        long position = Misc.IndexOfSequence(stream, UEFNMountPoint);

                        stream.Close(); //Don't need it.

                        if (position < 0)
                        {
                            Log.Information($"{slot} is not a UEFN game file and will be removed (Including ucas, utoc, pak, sig");

                            Delete($"{paks}\\{slot}.ucas");
                            Delete($"{paks}\\{slot}.utoc");
                            Delete($"{paks}\\{slot}.pak");
                            Delete($"{paks}\\{slot}.sig");
                            Delete($"{paks}\\{slot}.backup");
                        }
                        else
                        {
                            Log.Information($"{slot} is a UEFN game file");
                        }
                    }
                }
            }
        }

        public static void DownloadMain(string paks)
        {
            var parse = Endpoint.Read(Endpoint.Type.UEFN);

            if (!parse["Enabled"].Value<bool>())
            {
                Log.Warning("UEFN Is disabled");
                throw new Exception($"UEFN game files are currently disabled");
            }

            if (Cache["Version"].KeyIsNullOrEmpty())
            {
                Log.Information("Main UEFN version was set as null");
            }
            else if (parse["Version"].Value<int>() != Cache["Version"].Value<int>())
            {
                Log.Information("Main UEFN files are outdated and will be removed");
                DeleteMainUEFN();
            }
            else
            {
                if (((JArray)Cache["Downloadables"]).Count != ((JArray)parse["Downloadables"]).Count)
                {
                    Log.Information("Main UEFN files amount does not match the api. UEFN game files will be deleted");
                    DeleteMainUEFN();
                }
                else
                {
                    bool valid = true;

                    foreach (string downloadable in Cache["Downloadables"])
                    {
                        if (!File.Exists($"{paks}\\{downloadable}.ucas") || !File.Exists($"{paks}\\{downloadable}.utoc") || !File.Exists($"{paks}\\{downloadable}.pak") || !File.Exists($"{paks}\\{downloadable}.sig"))
                        {
                            Log.Information($"Cached main UEFN file is missing. UEFN ame files will be deleted");
                            DeleteMainUEFN();
                            valid = false;
                            break;
                        }
                    }

                    if (valid) return;
                }
            }

            void DeleteMainUEFN()
            {
                foreach (string downloadable in Cache["Downloadables"])
                {
                    Delete($"{paks}\\{downloadable}.ucas");
                    Delete($"{paks}\\{downloadable}.utoc");
                    Delete($"{paks}\\{downloadable}.pak");
                    Delete($"{paks}\\{downloadable}.sig");
                    Delete($"{paks}\\{downloadable}.backup");
                }
            }

            var downloadables = new List<Downloadable>();
            foreach (var downloadable in parse["Downloadables"])
            {
                if (downloadable["zip"].KeyIsNullOrEmpty())
                {
                    downloadables.Add(new() { Ucas = downloadable["ucas"].Value<string>(), Utoc = downloadable["utoc"].Value<string>(), Pak = downloadable["pak"].Value<string>(), Sig = downloadable["sig"].Value<string>() });
                }
                else
                {
                    downloadables.Add(new() { Zip = downloadable["zip"].Value<string>() });
                }
            }

            Log.Information($"Downloading {downloadables.Count} main UEFN game files");
            Download(new DirectoryInfo(paks), downloadables, out List<string> usedslots);

            Cache["Version"] = parse["Version"].Value<int>();
            Cache["Downloadables"] = new JArray(usedslots);

            File.WriteAllText(Path, Cache.ToString());
            Log.Information($"Wrote UEFN cache to {Path}");

            foreach (string slot in usedslots)
            {
                CProvider.Add(slot); //Should be last
            }
        }

        public static void Add(string paks, string name, Downloadable downloadable)
        {
            Remove(name);

            Log.Information($"Downloading 1 UEFN game file");
            Download(new DirectoryInfo(paks), new () { downloadable }, out List<string> usedslots);

            var newobject = JObject.FromObject(new
            {
                Name = name,
                Pakchunk = $"{paks}\\{usedslots.First()}"
            });

            (Cache["Externals"] as JArray).Add(newobject);
            File.WriteAllText(Path, Cache.ToString());
            Log.Information($"Wrote UEFN cache to {Path}");

            CProvider.Add(usedslots.First()); //Should be last
        }

        private static void Download(DirectoryInfo paks, List<Downloadable> downloadables, out List<string> usedslots)
        {
            var temp = new DirectoryInfo($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Temp\\GalaxyDownloadables");
            usedslots = new List<string>();

            if (temp.Exists)
            {
                Directory.Delete(temp.FullName, true);
            }

            Directory.CreateDirectory(temp.FullName);

            Log.Information($"Created temp folder for downloading at: {temp.FullName}");

            var openSlots = FindOpenSlots(paks.FullName);

            if (downloadables.Count > openSlots.Count)
            {
                Log.Error($"Failed to find open slot for main UEFN game file.");
                throw new CustomException($"Failed to find a open slot for main UEFN game file!\nPlease revert a revert a item you have swapped to make space for this.");
            }

            Log.Information($"Found a total of {openSlots.Count} slots open and only need: {downloadables.Count}");

            foreach (var downloadable in downloadables)
            {
                if (!string.IsNullOrEmpty(downloadable.Zip))
                {
                    //zip
                }
                else
                {
                    Misc.Download($"{temp}\\{openSlots.First()}.ucas", downloadable.Ucas);
                    Misc.Download($"{temp}\\{openSlots.First()}.utoc", downloadable.Utoc);
                    Misc.Download($"{temp}\\{openSlots.First()}.pak", downloadable.Pak);
                    Misc.Download($"{temp}\\{openSlots.First()}.sig", downloadable.Sig);
                    Misc.Download($"{temp}\\{openSlots.First()}.backup", downloadable.Utoc);

                    Log.Information($"Downloaded UEFN game files to: {temp}\\{openSlots.First()}");
                }

                usedslots.Add(openSlots.First());
                openSlots.Remove(openSlots.First());
            }

            long requriedSize = 0;
            foreach (var downloaded in temp.GetFiles())
            {
                requriedSize += downloaded.Length;
            }

            var driveInfo = new DriveInfo(paks.Root.Name);
            if (requriedSize > driveInfo.TotalFreeSpace)
            {
                Log.Error($"Drive: {driveInfo.Name} does not have enough space for UEFN game files! Required: {requriedSize} Has: {driveInfo.TotalFreeSpace}");
                throw new CustomException($"Drive: {driveInfo.Name} does not have enough space for UEFN game files!\nRequired: {requriedSize}\nHas: {driveInfo.TotalFreeSpace}\nPlease make room on the {driveInfo.Name} drive for this process to continue.");
            }

            foreach (var pak in temp.GetFiles())
            {
                File.Move(pak.FullName, $"{paks.FullName}\\{pak.Name}", true);
                Log.Information($"Moved: {pak.FullName} To: {paks.FullName}");
            }
        }

        public static void Remove(string name)
        {
            if (Cache["Externals"] is null)
                return;

            var Externals = new JArray();
            foreach (var slot in Cache["Externals"])
            {
                if (slot["Name"].Value<string>() == name)
                {
                    Delete($"{slot["Pakchunk"].Value<string>()}.ucas");
                    Delete($"{slot["Pakchunk"].Value<string>()}.utoc");
                    Delete($"{slot["Pakchunk"].Value<string>()}.pak");
                    Delete($"{slot["Pakchunk"].Value<string>()}.sig");
                    Delete($"{slot["Pakchunk"].Value<string>()}.backup");
                }
                else
                    Externals.Add(slot);
            }

            Cache["Externals"] = Externals;
            File.WriteAllText(Path, Cache.ToString());
            Log.Information($"Wrote UEFN cache to {Path}");
        }

        public static void Clear(string paks)
        {
            Log.Information("Removing main UEFN game files");

            if (!Cache["Downloadables"].KeyIsNullOrEmpty())
            {
                foreach (string slot in Cache["Downloadables"])
                {
                    Delete($"{paks}\\{slot}.ucas");
                    Delete($"{paks}\\{slot}.utoc");
                    Delete($"{paks}\\{slot}.pak");
                    Delete($"{paks}\\{slot}.sig");
                    Delete($"{paks}\\{slot}.backup");
                }
            }

            Log.Information("Removing externals");
            foreach (var slot in Cache["Externals"])
            {
                Delete($"{slot["Pakchunk"].Value<string>()}.ucas");
                Delete($"{slot["Pakchunk"].Value<string>()}.utoc");
                Delete($"{slot["Pakchunk"].Value<string>()}.pak");
                Delete($"{slot["Pakchunk"].Value<string>()}.sig");
                Delete($"{slot["Pakchunk"].Value<string>()}.backup");
            }

            Reset();
            File.WriteAllText(Path, Cache.ToString());
        }

        private static List<string> FindOpenSlots(string paks)
        {
            var available = new List<string>();
            var parse = Endpoint.Read(Endpoint.Type.UEFN);

            foreach (string slot in parse["Slots"])
            {
                if (!File.Exists($"{paks}\\{slot}.ucas") || !File.Exists($"{paks}\\{slot}.utoc") || !File.Exists($"{paks}\\{slot}.pak") || !File.Exists($"{paks}\\{slot}.sig"))
                {
                    Log.Information($"Found open slot: {slot}");
                    available.Add(slot);
                }
            }

            return available;
        }

        private static bool Delete(string path)
        {
            if (File.Exists(path))
            {
                CProvider.CloseStream(path);

                try
                {
                    Log.Information($"Deleting {path}");
                    File.Delete(path);
                }
                catch (Exception Exception)
                {
                    Log.Error(Exception, $"Failed to delete {path}");
                    throw new CustomException($"Failed to delete {path}");
                }
            }
            return true;
        }

        private static void Reset()
        {
            Cache = JObject.FromObject(new
            {
                Downloadables = new JArray(),
                Version = JValue.CreateNull(),
                Externals = new JArray()
            });
        }
    }
}