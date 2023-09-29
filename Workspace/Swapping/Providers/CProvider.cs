using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Versions;
using CUE4Parse.Utils;
using Galaxy_Swapper_v2.Workspace.Generation.Formats;
using Galaxy_Swapper_v2.Workspace.Properties;
using Galaxy_Swapper_v2.Workspace.Swapping.Other;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Galaxy_Swapper_v2.Workspace.Global;

namespace Galaxy_Swapper_v2.Workspace.Swapping.Providers
{
    public static class CProvider
    {
        private static DefaultFileProvider Provider { get; set; } = default!;
        public static List<StreamData> OpenedStreamers;
        public static Workspace.Structs.Export Export { get; set; } = default!;
        public static byte[] ExportName { get; set; } = default!;
        public static bool SaveExport = false;
        private const string PaksPath = "\\FortniteGame\\Content\\Paks";
        public static void Initialize()
        {
            if (Provider is not null)
            {
                Log.Information("CProvider is already initialized");
                return;
            }

            string paks = Settings.Read(Settings.Type.Installtion).Value<string>();

            if (paks is null || string.IsNullOrEmpty(paks) || !Directory.Exists(paks))
            {
                throw new CustomException(Languages.Read(Languages.Type.Message, "FortniteDirectoryEmpty"));
            }

            paks = string.Concat(paks, PaksPath);
            OpenedStreamers = new();

            PaksCheck.Validate(paks);
            PaksCheck.Backup(paks);
            AesProvider.Initialize();
            
            Provider = new(paks, SearchOption.TopDirectoryOnly, isCaseInsensitive: false, new(EGame.GAME_UE5_3));
            Provider.Initialize();
            Provider.SubmitKeys(AesProvider.Keys);

            WaitForEpicGames(); //If Fortnite starts swapper will auto close to prevent read errors.

            Log.Information($"Loaded Provider with version: {Provider.Versions.Game} to path ({paks}) with {AesProvider.Keys.Count()} AES keys.");
        }

        public static void Add(string pakchunk)
        {
            Provider.Initialize(pakchunk);
            Provider.SubmitKeys(AesProvider.Keys);
            Log.Information($"Added {pakchunk} to provider.");
        }

        public static bool Save(string path)
        {
            SaveExport = true;
            Export = null;
            ExportName = Encoding.ASCII.GetBytes(System.IO.Path.GetFileNameWithoutExtension(path));

            bool Result = Provider.TrySaveAsset(path, out byte[] Data);

            if (Result)
            {
                Export.Buffer = Data;
                Log.Information($"Exported {path}");
            }
            else
                Log.Error($"Failed export {path}");

            return Result;
        }

        public static void Dispose()
        {
            if (Provider == null)
                return;

            Log.Information("Disposing Provider");

            if (OpenedStreamers != null && OpenedStreamers.Count != 0)
            {
                foreach (var stream in OpenedStreamers)
                {
                    Log.Information($"Disposing {stream.Name} stream");
                    stream.Stream.Close();
                }
            }

            Provider.Dispose();
            Provider = null!;
        }

        public static string FormatGamePath(string path) => Provider.FixPath(path).SubstringBeforeLast('.');

        public static string FormatUEFNGamePath(string path)
        {
            const string game = "/game/";
            const string plugin = "fortnitegame/plugins/gamefeatures/";
            string search = path.ToLower();
            string formatted = Provider.Files.Keys.FirstOrDefault(gamepath => gamepath.SubstringBeforeLast('.').ToLower().EndsWith(search))!;

            if (string.IsNullOrEmpty(formatted))
            {
                Log.Error($"Could not find suitable UEFN game path: {path}");
                throw new CustomException($"Failed to find suitable UEFN game path for:\n{path}\nEnsure the custom plugin path is correct and the game file contains the asset.");
            }

            if (formatted.ToLower().StartsWith(plugin))
                formatted = formatted.Substring(plugin.Length);
            else if (formatted.ToLower().StartsWith(game)) //It should never start with /Game/ but just in case.
                formatted = formatted.Substring(game.Length);
            else
            {
                Log.Error($"Failed to format UEFN game path:\n{path}\nDoes not start with {game} or {plugin}");
                throw new CustomException($"Failed to format UEFN game path:\n{path}");
            }

            formatted = string.Format("/{0}{1}", formatted.Split('/').First(), path);
            Log.Information($"Created new UEFN game path: {formatted}");

            return formatted;
        }

        public static void CloseStream(string path)
        {
            if (OpenedStreamers is not null && OpenedStreamers.Count != 0)
            {
                foreach (var stream in OpenedStreamers)
                {
                    if (stream.Path != path) continue;
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

        private static void WaitForEpicGames()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    if (Provider is null) return;
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