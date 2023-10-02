using Galaxy_Swapper_v2.Workspace.Properties;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using static Galaxy_Swapper_v2.Workspace.Global;

namespace Galaxy_Swapper_v2.Workspace.Swapping.Other
{
    public static class CustomEpicGamesLauncher
    {
        private static readonly List<byte[]> cpatterns = new List<byte[]>()
        {
                new byte[] {
                    0x44, 0x3A, 0x5C, 0x62, 0x75, 0x69, 0x6C, 0x64,
                    0x5C, 0x2B, 0x2B, 0x50, 0x6F, 0x72, 0x74, 0x61,
                    0x6C, 0x5C, 0x53, 0x79, 0x6E, 0x63, 0x5C, 0x50,
                    0x6F, 0x72, 0x74, 0x61, 0x6C, 0x5C, 0x53, 0x6F,
                    0x75, 0x72, 0x63, 0x65, 0x5C, 0x50, 0x72, 0x6F,
                    0x67, 0x72, 0x61, 0x6D, 0x73, 0x5C, 0x45, 0x70,
                    0x69, 0x63, 0x47, 0x61, 0x6D, 0x65, 0x73, 0x4C,
                    0x61, 0x75, 0x6E, 0x63, 0x68, 0x65, 0x72, 0x5C,
                    0x4C, 0x61, 0x79, 0x65, 0x72, 0x73, 0x5C, 0x44,
                    0x61, 0x74, 0x61, 0x41, 0x63, 0x63, 0x65, 0x73,
                    0x73, 0x5C, 0x50, 0x75, 0x62, 0x6C, 0x69, 0x63,
                    0x5C, 0x4E, 0x6F, 0x74, 0x69, 0x66, 0x69, 0x63,
                    0x61, 0x74, 0x69, 0x6F, 0x6E, 0x2E, 0x68, 0x00,
                    0x21, 0x47, 0x65, 0x74, 0x50, 0x65, 0x72, 0x73,
                    0x69, 0x73, 0x74, 0x28, 0x29
                },
                new byte[] {
                    0x44, 0x3A, 0x2F, 0x62, 0x75, 0x69, 0x6C, 0x64,
                    0x2F, 0x2B, 0x2B, 0x50, 0x6F, 0x72, 0x74, 0x61,
                    0x6C, 0x2F, 0x53, 0x79, 0x6E, 0x63, 0x2F, 0x45,
                    0x6E, 0x67, 0x69, 0x6E, 0x65, 0x2F, 0x53, 0x6F,
                    0x75, 0x72, 0x63, 0x65, 0x2F, 0x52, 0x75, 0x6E,
                    0x74, 0x69, 0x6D, 0x65, 0x2F, 0x43, 0x6F, 0x72,
                    0x65, 0x2F, 0x50, 0x72, 0x69, 0x76, 0x61, 0x74,
                    0x65, 0x2F, 0x57, 0x69, 0x6E, 0x64, 0x6F, 0x77,
                    0x73, 0x2F, 0x57, 0x69, 0x6E, 0x64, 0x6F, 0x77,
                    0x73, 0x45, 0x72, 0x72, 0x6F, 0x72, 0x4F, 0x75,
                    0x74, 0x70, 0x75, 0x74, 0x44, 0x65, 0x76, 0x69,
                    0x63, 0x65, 0x2E, 0x63, 0x70, 0x70
                },
        };

        public static void Convert()
        {
            string EpicInstalltion = Settings.Read(Settings.Type.EpicInstalltion).Value<string>();

            if (!File.Exists(EpicInstalltion)) throw new CustomException($"Epic Games Launcher does not exist at:{EpicInstalltion}\nPlease ensure you have the corredct path for Epic Games Launcher selected!");
            if (!EpicInstalltion.CanEdit()) throw new CustomException($"Failed to open stream for Epic Games Launcher!\nPlease ensure Epic Games Launcher is closed during this process.");

            Log.Information($"Opening stream for: {EpicInstalltion}");

            using (FileStream stream = new FileStream(EpicInstalltion, FileMode.Open, FileAccess.ReadWrite))
            {
                stream.Position = stream.Length - 6;

                byte[] footer = new byte[6];
                stream.Read(footer, 0, 6);

                if (Encoding.ASCII.GetString(footer) == "galaxy") return;

                Log.Information("Replacing patterns");
                foreach (byte[] pattern in cpatterns)
                {
                    long position = Misc.IndexOfSequence(stream, pattern, pos: 20000000);

                    if (position < 0) continue;

                    stream.Position = position + pattern.Length;
                    stream.Write(new byte[59], 0, 59);
                }

                Log.Information("Adding a watermark");
                stream.Position = stream.Length;
                stream.Write(Encoding.ASCII.GetBytes("galaxy"), 0, 6);
            }
        }

        public static void Revert()
        {
            string EpicInstalltion = Settings.Read(Settings.Type.EpicInstalltion).Value<string>();

            if (!File.Exists(EpicInstalltion)) return;
            if (!EpicInstalltion.CanEdit()) throw new CustomException($"Failed to open stream for Epic Games Launcher!\nPlease ensure Epic Games Launcher is closed during this process.");

            Log.Information($"Deleting: {EpicInstalltion}");

            try
            {
                File.Delete(EpicInstalltion);
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message);
                throw new CustomException($"Failed to delete old Epic Games Launcher");
            }

            const string url = "https://raw.githubusercontent.com/GalaxySwapperOfficial/Galaxy-Swapper-API/main/In%20Game/EpicGamesLauncher";
            Log.Information($"Downloading {url} to {EpicInstalltion}");

            using (WebClient WC = new WebClient())
            {
                try
                {
                    WC.DownloadFile(url, EpicInstalltion);
                    WC.Dispose();
                }
                catch (Exception exception)
                {
                    Log.Error(exception.Message);
                    throw new CustomException($"Failed to download new Epic Games Launcher");
                }
            }
        }
    }
}