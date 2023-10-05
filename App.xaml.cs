using Galaxy_Swapper_v2.Workspace;
using Galaxy_Swapper_v2.Workspace.Properties;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Galaxy_Swapper_v2
{
    public partial class App : Application
    {
        public static readonly string Config = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Galaxy-Swapper-v2-Config";
        public static readonly string[] SubDirectories = { $"{Config}\\DLLS", $"{Config}\\Plugins", $"{Config}\\LOGS", $"{Config}\\Binaries" };
        protected override void OnStartup(StartupEventArgs e)
        {
            Process[] currentprocess = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
            if (currentprocess.Length > 1)
            {
                Message.DisplaySTA("Info", "It has been detected that Galaxy Swapper is already running. The process will be brought to the front.\nIf there is an issue, please end the 'Galaxy Swapper v2' process in Task Manager and restart the application.", MessageBoxButton.OK);
                Win32.SetProcessHigher(currentprocess);
                Environment.Exit(0);
            }

            if (File.Exists($"{Config}\\Key.config")) //Remove old config folder
            {
                Directory.Delete(Config, true);
            }

            if (!Directory.Exists(Config)) //Create config folder
            {
                Directory.CreateDirectory(Config);
            }

            foreach (string sub in SubDirectories)
            {
                Directory.CreateDirectory(sub);
            }

            Output.Initialize(); //Serilog logger

            Log.Information("Version {0}", Global.Version);
            Log.Information("API-Version {0}", Global.ApiVersion);
            Log.Information("Runtime {0}", RuntimeInformation.FrameworkDescription);
            Log.Information("Config-Path: {0}", Config);

            Settings.Initialize();
            SwapLogs.Initialize();
            UEFN.Initialize();
            Binaries.Initialize();

            UEFN.DownloadMain("C:\\Program Files\\Epic Games\\Fortnite\\FortniteGame\\Content\\Paks");

            base.OnStartup(e);
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Fatal("Application caught a unexpected error and can not recover");
            Log.Fatal(e.Exception.ToString());

            Message.DisplaySTA("Error", $"Application caught a unexpected error and can not recover.\n{e.Exception}", MessageBoxButton.OK, new() { Global.Discord });
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            Log.Information("Shutting down application..");
            Log.CloseAndFlush();
        }
    }
}