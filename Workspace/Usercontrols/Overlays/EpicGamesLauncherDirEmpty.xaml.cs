using Galaxy_Swapper_v2.Workspace.Properties;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Serilog;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Galaxy_Swapper_v2.Workspace.Usercontrols.Overlays
{
    public partial class EpicGamesLauncherDirEmpty : UserControl
    {
        private BackgroundWorker DetectWorker;
        private bool EndWorker = false;
        public EpicGamesLauncherDirEmpty() => InitializeComponent();

        private void Close_Click(object sender, MouseButtonEventArgs e)
        {
            EndWorker = true;
            Memory.MainView.RemoveOverlay();
        }
        private void EpicGamesLauncherDirEmpty_Loaded(object sender, RoutedEventArgs e)
        {
            Header.Text = Languages.Read(Languages.Type.View, "EpicGamesLauncherDirEmpty", "Header");
            Description.Text = Languages.Read(Languages.Type.View, "EpicGamesLauncherDirEmpty", "Description");

            DetectWorker = new BackgroundWorker();
            DetectWorker.DoWork += DetectWorker_DoWork;
            DetectWorker.RunWorkerCompleted += DetectWorker_Completed;
            DetectWorker.RunWorkerAsync();
        }

        private async void DetectWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (EndWorker)
                    return;

                Process[] epicgamesLauncher = Process.GetProcessesByName("EpicGamesLauncher");

                if (epicgamesLauncher.Length != 0)
                {
                    string path = epicgamesLauncher[0].MainModule.FileName;

                    Log.Information($"Detected EpicGamesLauncher ({epicgamesLauncher[0].Id}) with path: {path}");

                    if (path.Contains("\\Epic Games"))
                        path = path.Split("\\Epic Games").First();

                    if (!Directory.Exists($"{path}\\Epic Games\\Launcher\\Portal\\Binaries\\Win64") || !File.Exists($"{path}\\Epic Games\\Launcher\\Portal\\Binaries\\Win64\\EpicGamesLauncher.exe"))
                    {
                        Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Error"), Languages.Read(Languages.Type.Message, "EpicDetectedInvalidPath"), MessageBoxButton.OK);
                        return;
                    }

                    path = $"{path}\\Epic Games\\Launcher\\Portal\\Binaries\\Win64\\EpicGamesLauncher.exe";

                    Settings.Edit(Settings.Type.EpicInstalltion, path);
                    Log.Information($"Set new path to: {path}");

                    EpicGamesLauncher.Close();
                    Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"), string.Format(Languages.Read(Languages.Type.Message, "EpicDetectedNewPath"), path), MessageBoxButton.OK);
                    return;
                }

                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }

        private void DetectWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            Memory.SettingsView.SettingsView_Loaded(null!, null!);
            Close_Click(null!, null!);
        }
    }
}
