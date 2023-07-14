using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;

namespace Galaxy_Swapper_v2.Workspace.Utilities
{
    public static class EpicGamesLauncher
    {
        const string AppName = "Fortnite";
        const string LaunchArg = "com.epicgames.launcher://apps/Fortnite?action=launch&silent=true";
        const string VerifyArg = "com.epicgames.launcher://apps/Fortnite?action=verify&silent=false";
        static readonly string[] Processes = { "EpicGamesLauncher", "FortniteLauncher", "FortniteClient-Win64-Shipping", "FortniteClient-Win64-Shipping_BE", "FortniteClient-Win64-Shipping_EAC", "FortniteClient-Win64-Shipping_EAC_EOS", "CrashReportClient" };
        static readonly string UnrealEngineLauncher = $"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\\Epic\\UnrealEngineLauncher";
        public static string InstallLocation(string InstallLocation = "")
        {
            TryInstallLocation(ref InstallLocation);
            return InstallLocation;
        }

        public static bool TryInstallLocation(ref string InstallLocation)
        {
            if (!Directory.Exists(UnrealEngineLauncher))
                return false;

            if (!File.Exists($"{UnrealEngineLauncher}\\LauncherInstalled.dat"))
                return false;

            string Content = File.ReadAllText($"{UnrealEngineLauncher}\\LauncherInstalled.dat");

            if (string.IsNullOrEmpty(Content) || !Content.ValidJson())
                return false;

            var Parse = JObject.Parse(Content);

            foreach (var Installation in Parse["InstallationList"])
            {
                if (Installation["AppName"].Value<string>() == AppName)
                {
                    string InstallationPath = Installation["InstallLocation"].Value<string>();

                    if (!Directory.Exists(InstallationPath) || !Directory.Exists($"{InstallationPath}\\FortniteGame\\Content\\Paks"))
                        continue;

                    InstallLocation = InstallationPath;
                    return true;
                }
            }

            return false;
        }

        public static void Launch() => TryLaunch();

        public static bool TryLaunch()
        {
            try
            {
                LaunchArg.UrlStart();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void Close() => TryClose();

        public static bool TryClose()
        {
            foreach (string Name in Processes)
            {
                try
                {
                    Process[] process = Process.GetProcessesByName(Name);
                    if (process.Length == 0)
                        continue;
                    process[0].Kill();
                    process[0].WaitForExit();
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsOpen()
        {
            foreach (string Name in Processes)
            {
                if (Name == "EpicGamesLauncher")
                    continue;

                Process[] process = Process.GetProcessesByName(Name);
                if (process.Length != 0)
                    return true;
            }

            return false;
        }

        public static void Verify() => TryVerify();

        public static bool TryVerify()
        {
            try
            {
                VerifyArg.UrlStart();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}