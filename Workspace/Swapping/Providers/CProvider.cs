using Galaxy_Swapper_v2.Workspace.CProvider;
using Galaxy_Swapper_v2.Workspace.Properties;
using Galaxy_Swapper_v2.Workspace.Swapping.Other;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Galaxy_Swapper_v2.Workspace.Global;

namespace Galaxy_Swapper_v2.Workspace.Swapping.Providers
{
    public static class CProvider
    {
        public static DefaultFileProvider DefaultProvider = null!;
        public static DefaultFileProvider UEFNProvider = null!;
        public const string Paks = "\\FortniteGame\\Content\\Paks";

        public static void InitDefault()
        {
            if (DefaultProvider is not null)
                return;

            string paks = $"{Settings.Read(Settings.Type.Installtion)}{Paks}";

            if (paks is null || string.IsNullOrEmpty(paks) || !Directory.Exists(paks))
            {
                throw new FortniteDirectoryEmptyException(Languages.Read(Languages.Type.Message, "FortniteDirectoryEmpty"));
            }

            Pakchunks.Validate(paks);
            AesProvider.Initialize();

            DefaultProvider = new(new(paks));
            DefaultProvider.SubmitKeys(AesProvider.Keys);

            var parse = Endpoint.Read(Endpoint.Type.UEFN);

            if (parse["Enabled"].Value<bool>())
            {
                DefaultProvider.Initialize(parse["Slots"].ToObject<List<string>>());
            }
            else
            {
                DefaultProvider.Initialize();
            }

            WaitForEpicGames();
        }

        public static void InitUEFN()
        {
            if (UEFNProvider is not null)
                return;

            string paks = $"{Settings.Read(Settings.Type.Installtion)}{Paks}";

            if (paks is null || string.IsNullOrEmpty(paks) || !Directory.Exists(paks))
            {
                throw new CustomException(Languages.Read(Languages.Type.Message, "FortniteDirectoryEmpty"));
            }

            var parse = Endpoint.Read(Endpoint.Type.UEFN);

            UEFNProvider = new(new(paks));
            UEFNProvider.Initialize(parse["Slots"].ToObject<List<string>>(), true);
        }

        public static string FormatUEFNGamePath(string path)
        {
            const string game = "/game/";
            const string plugin = "fortnitegame/plugins/gamefeatures/";
            string search = path.ToLower();
            string formatted = UEFNProvider.FindGameFile(search);

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