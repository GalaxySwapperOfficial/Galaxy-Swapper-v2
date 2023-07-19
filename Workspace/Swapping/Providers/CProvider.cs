using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Versions;
using CUE4Parse.Utils;
using Galaxy_Swapper_v2.Workspace.Generation.Formats;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Galaxy_Swapper_v2.Workspace.Global;

namespace Galaxy_Swapper_v2.Workspace.Swapping.Providers
{
    /// <summary>
    /// All the code below was provided from: https://github.com/GalaxySwapperOfficial/Galaxy-Swapper-v2
    /// You can also find us at https://galaxyswapperv2.com/Guilded
    /// </summary>
    public static class CProvider
    {
        private static DefaultFileProvider Provider { get; set; } = default!;
        public static List<StreamData> OpenedStreamers;
        private static Task FortniteCheckingTask;
        public static Export Export { get; set; } = default!;
        public static byte[] ExportName { get; set; } = default!;
        public static bool SaveExport = false;
        public static void Initialize(string Path)
        {
            if (Provider != null)
            {
                Log.Information("Provider was already loaded skipping.");
                return;
            }
            
            try
            {
                OpenedStreamers = new List<StreamData>();
                Provider = new DefaultFileProvider(Path, System.IO.SearchOption.TopDirectoryOnly, false, new VersionContainer(EGame.GAME_UE5_3));
                Provider.Initialize();
                Provider.SubmitKeys(FortniteApi.Keys);

                Log.Information($"Loaded Provider with version: {Provider.Versions.Game} to path ({Path}) with {FortniteApi.Keys.Count()} AES keys.");

                Log.Information("Starting FortniteCheckingTask");
                StartChecking();
            }
            catch (Exception Exception)
            {
                Log.Error(Exception, "Failed to initialize Provider!");
                throw;
            }
        }

        public static void Add(string pakchunk)
        {
            Provider.Initialize(pakchunk);
            Provider.SubmitKeys(FortniteApi.Keys);
            Log.Information($"Added {pakchunk} to provider.");
        }

        public static bool Save(string Path)
        {
            SaveExport = true;
            Export = null;
            ExportName = Encoding.ASCII.GetBytes(System.IO.Path.GetFileNameWithoutExtension(Path));

            bool Result = Provider.TrySaveAsset(Path, out byte[] Data);

            if (Result)
            {
                Export.Buffer = Data;
                Log.Information($"Exported {Path}");
            }
            else
                Log.Error($"Failed export {Path}");

            return Result;
        }

        public static string FormatGamePath(string path) => CProvider.Provider.FixPath(path).SubstringBeforeLast('.');

        public static string FormatUEFNGamePath(string path)
        {
            string pathtofind = path.ToLower();
            string newpath = null;

            foreach (string gamepath in Provider.Files.Keys)
            {
                if (gamepath.SubstringBeforeLast('.').ToLower().EndsWith(pathtofind))
                {
                    newpath = gamepath;
                    break;
                }
            }

            if (newpath == null)
                throw new CustomException($"Failed to find suitable UEFN game path for:\n{path}\nEnsure the custom plugin files are correct and the game path.");

            if (newpath.StartsWith("FortniteGame/Plugins/GameFeatures/") || newpath.StartsWith("fortnitegame/plugins/gamefeatures/"))
                newpath = newpath.Substring(34);
            else if (newpath.StartsWith("/Game/") || newpath.StartsWith("/game/"))
                newpath = newpath.Substring(6);
            else
                throw new CustomException($"Failed to find suitable UEFN game path for:\n{path}\nEnsure the custom plugin files are correct and the game path.");

            newpath = newpath.Split('/').First();
            newpath = $"/{newpath}{path}";

            Log.Information($"Created new UEFN game path: {newpath}");

            return newpath;
        }

        public static void CloseStream(string path)
        {
            if (CProvider.OpenedStreamers != null && CProvider.OpenedStreamers.Count != 0)
            {
                foreach (var stream in CProvider.OpenedStreamers)
                {
                    if (stream.Path == path)
                    {
                        try
                        {
                            stream.Stream.Close();
                            Log.Information($"Closed {stream.Name} stream");
                        }
                        catch
                        {
                            Log.Error($"Failed to close {stream.Name} stream");
                        }
                    }
                }
            }
        }

        private static void StartChecking()
        {
            FortniteCheckingTask = Task.Run(async () =>
            {
                while (true)
                {
                    if (EpicGamesLauncher.IsOpen())
                    {
                        Log.Warning("Detected Fortnite being opened! Closing swapper to prevent read errors");
                        Environment.Exit(0);
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            });
        }
    }
}