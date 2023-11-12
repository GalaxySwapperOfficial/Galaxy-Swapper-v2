using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Galaxy_Swapper_v2.Workspace.Utilities
{
    public static class EpicGamesLauncher
    {
        private static readonly string ProgramData = $"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\\Epic";
        private static readonly string SavedLogs = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\EpicGamesLauncher\\Saved\\Logs";
        private static readonly string AppName = "Fortnite";
        private const string LaunchArg = "com.epicgames.launcher://apps/Fortnite?action=launch&silent=true";
        private const string VerifyArg = "com.epicgames.launcher://apps/Fortnite?action=verify&silent=false";
        private static readonly string[] Processes = { "EpicGamesLauncher", "FortniteLauncher", "FortniteClient-Win64-Shipping", "FortniteClient-Win64-Shipping_BE", "FortniteClient-Win64-Shipping_EAC", "FortniteClient-Win64-Shipping_EAC_EOS", "CrashReportClient" };

        public static string Installation() => TryInstallation(out string location) ? location : string.Empty;
        public static string FortniteInstallation() => TryFortniteInstallation(out string location) ? location : null!;
        public static void Launch() => LaunchArg.UrlStart();
        public static void Close(bool Fortnite = true) => TryClose(Fortnite: Fortnite);
        public static void Verify() => VerifyArg.UrlStart();
        public static bool IsOpen() => Process.GetProcessesByName("EpicGamesLauncher").Length != 0;

        public static bool TryInstallation(out string location)
        {
            location = string.Empty;
            if (!Directory.Exists(SavedLogs) || !File.Exists($"{SavedLogs}\\EpicGamesLauncher.log")) return false;
            if (File.Exists($"{SavedLogs}\\EpicGamesLauncher.temp")) File.Delete($"{SavedLogs}\\EpicGamesLauncher.temp");
            File.Copy($"{SavedLogs}\\EpicGamesLauncher.log", $"{SavedLogs}\\EpicGamesLauncher.temp");
            foreach (string line in File.ReadAllLines($"{SavedLogs}\\EpicGamesLauncher.temp"))
            {
                Match match = Regex.Match(line, @"LogInit: Base Directory: (.*)");
                if (match.Success)
                {
                    string win64 = $"{match.Groups[1].Value.Trim()}EpicGamesLauncher.exe";
                    if (!File.Exists(win64)) continue;
                    location = win64;
                    return true;
                }
            }
            return false;
        }

        public static bool TryFortniteInstallation(out string location)
        {
            location = null!;
            if (!Directory.Exists(ProgramData)) return false;
            string launcherinstalled = $"{ProgramData}\\UnrealEngineLauncher\\LauncherInstalled.dat";
            if (!File.Exists(launcherinstalled)) return false;
            string content = File.ReadAllText(launcherinstalled);
            if (string.IsNullOrEmpty(content) || !content.ValidJson()) return false;
            var parse = JObject.Parse(content);
            foreach (var installation in parse["InstallationList"])
            {
                if (installation["AppName"].Value<string>() == AppName)
                {
                    string installlocation = installation["InstallLocation"].Value<string>();
                    if (!Directory.Exists(installlocation) || !Directory.Exists($"{installlocation}\\FortniteGame\\Content\\Paks")) continue;
                    location = installlocation;
                }
            }
            return location != null;
        }

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

        public static bool TryClose(bool Fortnite = true)
        {
            try
            {
                foreach (string name in Processes)
                {
                    Process[] process = Process.GetProcessesByName(name);
                    if (process.Length == 0) continue;
                    process[0].Kill();
                    process[0].WaitForExit();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

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