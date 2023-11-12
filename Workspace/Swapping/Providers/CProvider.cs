﻿using CUE4Parse.UE4.Versions;
using CUE4Parse.Utils;
using Galaxy_Swapper_v2.Workspace.Properties;
using Galaxy_Swapper_v2.Workspace.Structs;
using Galaxy_Swapper_v2.Workspace.Swapping.Other;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Galaxy_Swapper_v2.Workspace.Global;

namespace Galaxy_Swapper_v2.Workspace.Swapping.Providers
{
    public static class CProvider
    {
        public static ProviderData DefaultProvider, UEFNProvider;
        public static Export Export;
        public static byte[] ExportName;
        public const string Paks = "\\FortniteGame\\Content\\Paks";

        public static void InitProvider(ref ProviderData provider)
        {
            if (provider is not null)
                return;

            string paks = $"{Settings.Read(Settings.Type.Installtion)}{Paks}";

            if (string.IsNullOrEmpty(paks) || !Directory.Exists(paks))
            {
                throw new FortniteDirectoryEmptyException(Languages.Read(Languages.Type.Message, "FortniteDirectoryEmpty"));
            }

            Pakchunks.Validate(paks);
            AesProvider.Initialize();

            provider = new ProviderData
            {
                SaveStreams = true,
                FileProvider = new(paks, SearchOption.TopDirectoryOnly, isCaseInsensitive: false, new(EGame.GAME_UE5_3))
            };
            provider.FileProvider.Initialize();
            provider.FileProvider.SubmitKeys(AesProvider.Keys);
            provider.SaveStreams = false;

            WaitForEpicGames();
            Log.Information($"Loaded Provider with version: {provider.FileProvider.Versions.Game} to path ({paks}) with {AesProvider.Keys.Count()} AES keys.");
        }

        public static void InitDefault() => InitProvider(ref DefaultProvider);
        public static void InitUEFN() => InitProvider(ref UEFNProvider);

        public static bool Save(string path)
        {
            DefaultProvider.Save = true;
            Export = null!;
            ExportName = Encoding.ASCII.GetBytes(System.IO.Path.GetFileNameWithoutExtension(path));

            bool result = DefaultProvider.FileProvider.TrySaveAsset(path, out byte[] buffer);

            if (result)
            {
                Export.Buffer = buffer;
                Log.Information($"Exported: {path}");
                return true;
            }

            Log.Information($"Failed to export: {path}");
            return false;
        }

        public static string FormatUEFNGamePath(string path)
        {
            const string game = "/game/";
            const string plugin = "fortnitegame/plugins/gamefeatures/";
            string search = path.ToLower();
            string formatted = UEFNProvider.FileProvider.Files.Keys.FirstOrDefault(gamepath => gamepath.SubstringBeforeLast('.').ToLower().EndsWith(search))!;

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

        private static void WaitForEpicGames()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    if (DefaultProvider is null) return;
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
