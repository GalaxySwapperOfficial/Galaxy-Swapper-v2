using Galaxy_Swapper_v2.Workspace.CProvider;
using Galaxy_Swapper_v2.Workspace.Generation.Formats;
using Galaxy_Swapper_v2.Workspace.Swapping.Other;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
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

        public static void DownloadMain(string paks, string tag)
        {
            var parse = Endpoint.Read(Endpoint.Type.UEFN);

            if (!parse["Enabled"].Value<bool>())
            {
                Log.Warning("UEFN Is disabled");
                throw new Exception($"UEFN game files are currently disabled");
            }

            if (Cache["Version"].KeyIsNullOrEmpty())
            {
                Log.Information("UEFN global version is null");
            }
            else if (parse["Version"].Value<int>() != Cache["Version"].Value<int>())
            {
                Log.Information("UEFN global version miss match");
                Clear(paks, externals: false);
            }

            var cacheDownloadable = Cache["Downloadables"].OfType<JObject>().FirstOrDefault(d => d.Value<string>("tag") == tag);
            var downloadable = parse["Downloadables"].OfType<JObject>().FirstOrDefault(d => d.Value<string>("tag") == tag);

            if (cacheDownloadable is not null)
            {
                if (!Exist(cacheDownloadable["Pakchunk"].Value<string>(), new[] { ".ucas", ".utoc", ".pak", ".sig" }))
                {
                    Log.Information($"UEFN {tag} is missing pakchunks");
                    Delete(cacheDownloadable["Pakchunk"].Value<string>(), new[] { ".ucas", ".utoc", ".pak", ".sig" });
                }
                else if (downloadable["version"].Value<int>() == cacheDownloadable["version"].Value<int>())
                {
                    Log.Information($"UEFN {tag} is up to date");
                    return;
                }
                else
                {
                    Log.Information($"UEFN {tag} is out of date");
                    Delete(cacheDownloadable["Pakchunk"].Value<string>(), new[] {  ".ucas", ".utoc", ".pak", ".sig" });
                }

                (Cache["Downloadables"] as JArray).Remove(cacheDownloadable);
            }

            //Dispose UEFN file provider so files aren't in use.
            Dispose();

            var downloadables = new List<Downloadable>();
            if (downloadable["zip"].KeyIsNullOrEmpty())
            {
                downloadables.Add(new() { Ucas = downloadable["ucas"].Value<string>(), Utoc = downloadable["utoc"].Value<string>(), Pak = downloadable["pak"].Value<string>(), Sig = downloadable["sig"].Value<string>() });
            }
            else
            {
                downloadables.Add(new() { Zip = downloadable["zip"].Value<string>() });
            }

            Log.Information($"Downloading {tag} downloadable");
            Download(new DirectoryInfo(paks), downloadables, out List<string> usedslots);

            var newobject = JObject.FromObject(new
            {
                tag = tag,
                version = downloadable["version"].Value<int>(),
                Pakchunk = $"{paks}\\{usedslots.First()}"
            });

            Cache["Version"] = parse["Version"].Value<int>();
            (Cache["Downloadables"] as JArray).Add(newobject);


            File.WriteAllText(Path, Cache.ToString());
            Log.Information($"Wrote UEFN cache to {Path}");

            CProviderManager.InitUEFN();
        }

        public static void Add(string paks, string name, Downloadable downloadable)
        {
            //Dispose UEFN file provider so files aren't in use.
            Dispose();
            Remove(name);

            Log.Information($"Downloading 1 downloadable");
            Download(new DirectoryInfo(paks), new() { downloadable }, out List<string> usedslots);

            var newobject = JObject.FromObject(new
            {
                Name = name,
                Pakchunk = $"{paks}\\{usedslots.First()}"
            });

            (Cache["Externals"] as JArray).Add(newobject);
            File.WriteAllText(Path, Cache.ToString());
            Log.Information($"Wrote UEFN cache to {Path}");

            CProviderManager.InitUEFN();
        }

        private static void Download(DirectoryInfo paks, List<Downloadable> downloadables, out List<string> usedslots)
        {
            var temp = new DirectoryInfo($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Temp\\GalaxyDownloadables");
            usedslots = new ();

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

            Log.Information($"Ensuring slots {downloadables.Count} are not taken");
            foreach (string slot in openSlots.Take(downloadables.Count))
            {
                Delete($"{paks.FullName}\\{slot}.ucas");
                Delete($"{paks.FullName}\\{slot}.utoc");
                Delete($"{paks.FullName}\\{slot}.pak");
                Delete($"{paks.FullName}\\{slot}.sig");
                Delete($"{paks.FullName}\\{slot}.backup");
            }

            foreach (var downloadable in downloadables)
            {
                if (!string.IsNullOrEmpty(downloadable.Zip))
                {
                    string zippedpath = $"{temp}\\Zipped ({openSlots.First()})";

                    Directory.CreateDirectory(zippedpath);
                    Log.Information($"Created temp zip directory at: {zippedpath}");

                    var gamefiles = new FileInfo($"{zippedpath}\\GameFiles.zip");

                    Misc.Download(gamefiles.FullName, downloadable.Zip, name: "UEFN zipped game file");

                    if (!gamefiles.Exists)
                    {
                        throw new CustomException($"{gamefiles.FullName} does not exist? How?");
                    }

                    var tempdriveInfo = new DriveInfo(gamefiles.FullName);

                    if (gamefiles.Length > tempdriveInfo.TotalFreeSpace) //Best way I can think of to check this
                    {
                        Log.Error($"Drive: {tempdriveInfo.Name} does not have enough space for UEFN game files! Required: {gamefiles.Length} Has: {tempdriveInfo.TotalFreeSpace}");
                        throw new CustomException($"Drive: {tempdriveInfo.Name} does not have enough space for UEFN game files!\nRequired: {gamefiles.Length}\nHas: {tempdriveInfo.TotalFreeSpace}\nPlease make room on the {tempdriveInfo.Name} drive for this process to continue.");
                    }

                    Log.Information("Extracting GameFiles.zip");
                    ZipFile.ExtractToDirectory(gamefiles.FullName, zippedpath);

                    string ucas = Misc.FindFileByExtension(zippedpath, ".ucas");
                    if (ucas is null)
                    {
                        throw new CustomException("Zipped UEFN game files does not contain a .ucas file!");
                    }

                    File.Move(ucas, $"{temp}\\{openSlots.First()}.ucas");
                    Log.Information($"Moved: {ucas} to: {temp}\\{openSlots.First()}.ucas");

                    string utoc = Misc.FindFileByExtension(zippedpath, ".utoc");
                    if (utoc is null)
                    {
                        throw new CustomException("Zipped UEFN game files does not contain a .utoc file!");
                    }

                    File.Copy(utoc, $"{temp}\\{openSlots.First()}.backup");
                    File.Move(utoc, $"{temp}\\{openSlots.First()}.utoc");
                    Log.Information($"Moved: {utoc} to: {temp}\\{openSlots.First()}.utoc");

                    string pak = Misc.FindFileByExtension(zippedpath, ".pak");
                    if (pak is null)
                    {
                        throw new CustomException("Zipped UEFN game files does not contain a .pak file!");
                    }

                    File.Move(pak, $"{temp}\\{openSlots.First()}.pak");
                    Log.Information($"Moved: {pak} to: {temp}\\{openSlots.First()}.pak");

                    string sig = Misc.FindFileByExtension(zippedpath, ".sig");
                    if (sig is null)
                    {
                        throw new CustomException("Zipped UEFN game files does not contain a .sig file!");
                    }

                    File.Move(sig, $"{temp}\\{openSlots.First()}.sig");
                    Log.Information($"Moved: {sig} to: {temp}\\{openSlots.First()}.sig");

                    Log.Information("Removing temp zip directory");
                    Directory.Delete(zippedpath, true);
                }
                else
                {
                    Misc.Download($"{temp}\\{openSlots.First()}.ucas", downloadable.Ucas, name: "UEFN game file (Ucas)");
                    Misc.Download($"{temp}\\{openSlots.First()}.utoc", downloadable.Utoc, name: "UEFN game file (Utoc)");
                    Misc.Download($"{temp}\\{openSlots.First()}.pak", downloadable.Pak, name: "UEFN game file (Pak)");
                    Misc.Download($"{temp}\\{openSlots.First()}.sig", downloadable.Sig, name: "UEFN game file (Sig)");
                    Misc.Download($"{temp}\\{openSlots.First()}.backup", downloadable.Utoc, name: "UEFN game file (Utoc)");

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

            Directory.Delete(temp.FullName, true);
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

        public static void Clear(string paks, bool main = true, bool externals = true)
        {
            //Add a if check since we changed the main to pakchunk and not downloadables slots
            if (main && !Cache["Downloadables"].KeyIsNullOrEmpty())
            {
                Log.Information("Removing main UEFN game files");

                //Old uefn config still has string array
                if (Cache["Downloadables"].First().Type == JTokenType.String)
                {
                    foreach (string slot in Cache["Downloadables"])
                    {
                        Delete($"{paks}\\{slot}", new[] { ".ucas", ".utoc", ".pak", ".sig", ".backup" });
                    }
                }
                else
                {
                    foreach (var slot in Cache["Downloadables"])
                    {
                        Delete(slot["Pakchunk"].Value<string>(), new[] { ".ucas", ".utoc", ".pak", ".sig", ".backup" });
                    }
                }
            }

            if (externals)
            {
                Log.Information("Removing externals");
                foreach (var slot in Cache["Externals"])
                {
                    Delete(slot["Pakchunk"].Value<string>(), new[] { ".ucas", ".utoc", ".pak", ".sig", ".backup" });
                }
            }

            Reset();
            File.WriteAllText(Path, Cache.ToString());
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

        #region Utils
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

        public static bool Delete(string path, string[] extensions)
        {
            foreach (string extension in extensions)
            {
                if (!Delete($"{path}{extension}"))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Delete(string path)
        {
            var fileInfo = new FileInfo(path);
            if (fileInfo.Exists)
            {
                try
                {
                    Log.Information($"Deleting {fileInfo.FullName}");
                    File.Delete(fileInfo.FullName);
                }
                catch (Exception Exception)
                {
                    Log.Error(Exception, $"Failed to delete {fileInfo.FullName}");
                    throw new CustomException($"Failed to delete {fileInfo.FullName}");
                }
            }
            return true;
        }

        public static bool Exist(string path, string[] extensions)
        {
            foreach (string extension in extensions)
            {
                if (!File.Exists($"{path}{extension}"))
                {
                    return false;
                }
            }

            return true;
        }

        public static void Dispose()
        {
            CProviderManager.UEFNProvider?.Dispose();
            CProviderManager.UEFNProvider = null!;
        }

        #endregion
    }
}