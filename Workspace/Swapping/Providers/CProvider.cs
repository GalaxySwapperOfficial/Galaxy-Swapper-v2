using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Versions;
using CUE4Parse.Utils;
using Galaxy_Swapper_v2.Workspace.Generation.Formats;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Serilog;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy_Swapper_v2.Workspace.Swapping.Providers
{
    public static class CProvider
    {
        private static DefaultFileProvider Provider { get; set; } = default!;
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