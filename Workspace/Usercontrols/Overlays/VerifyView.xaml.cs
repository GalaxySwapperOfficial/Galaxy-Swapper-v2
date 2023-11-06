using Galaxy_Swapper_v2.Workspace.CProvider;
using Galaxy_Swapper_v2.Workspace.CProvider.Objects;
using Galaxy_Swapper_v2.Workspace.Hashes;
using Galaxy_Swapper_v2.Workspace.Properties;
using Galaxy_Swapper_v2.Workspace.Swapping.Other;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Galaxy_Swapper_v2.Workspace.Verify.EpicGames;
using Galaxy_Swapper_v2.Workspace.Verify.EpicManifestParser.Objects;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Galaxy_Swapper_v2.Workspace.Usercontrols.Overlays
{
    /// <summary>
    /// Interaction logic for VerifyView.xaml
    /// </summary>
    public partial class VerifyView : UserControl
    {
        private BackgroundWorker Worker { get; set; } = default!;
        public VerifyView()
        {
            InitializeComponent();
            Worker = new BackgroundWorker();
            Worker.DoWork += Worker_Convert;
            Worker.RunWorkerAsync();
        }

        public enum Type
        {
            None, // ?
            Info,
            Warning,
            Error
        }

        public void Output(string content, Type type)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (OutputBlock.ActualHeight > 350)
                {
                    OutputBlock.Text = string.Empty;
                }

                OutputBlock.Text += $"[LOG] {content}\n";

                switch (type)
                {
                    case Type.Info:
                        Log.Information(content); 
                        break;
                    case Type.Warning:
                        Log.Warning(content);
                        break;
                    case Type.Error: 
                        Log.Error(content);
                        Thread.Sleep(TimeSpan.FromSeconds(10));
                        break;
                }
            });
        }

        private void Exit() => Memory.MainView.RemoveOverlay();

        //Need to check If fortnite dir is null
        private void Worker_Convert(object sender, DoWorkEventArgs e)
        {
            string installtion = $"{Settings.Read(Settings.Type.Installtion)}";
            var pakDirectoryInfo = new DirectoryInfo($"{installtion}\\FortniteGame\\Content\\Paks");

            Output("Disposing providers", Type.Info);
            CProviderManager.DefaultProvider?.Dispose();
            CProviderManager.UEFNProvider?.Dispose();

            Output("Restoring Epic Games Launcher", Type.Info);
            CustomEpicGamesLauncher.Revert();

            Output("Removing UEFN game files", Type.Info);
            UEFN.Clear(pakDirectoryInfo.FullName);

            Output("Attempting to download Fortnite manifest file", Type.Info);

            var oauth = new OAuth();
            if (!oauth.Download(this))
            {
                Message.DisplaySTA("Error", "Failed to establish a connection to the Epic Games endpoint!", solutions: new[] { "Disable Windows Defender Firewall", "Disable any anti-virus softwares", "Turn on a VPN" });
                Exit(); return;
            }

            var liveManifest = new LiveManifest();
            if (!liveManifest.Download(this, oauth.Access_Token))
            {
                Message.DisplaySTA("Error", "Failed to establish a connection to the Epic Games endpoint!", solutions: new[] { "Disable Windows Defender Firewall", "Disable any anti-virus softwares", "Turn on a VPN" });
                Exit(); return;
            }

            Output("Parsing manifest", Type.Info);

            byte[] manifestBufer = new ManifestInfo(liveManifest.Parse.ToString()).DownloadManifestData();
            var manifest = new Manifest(manifestBufer);

            Output("Populating file list", Type.Info);

            var pakFileManifests = new List<FileManifest>();
            foreach (var fileManifest in manifest.FileManifests)
            {
                var directoryPath = fileManifest.Name.SubstringBeforeWithLast('/');
                if (directoryPath != "FortniteGame/Content/Paks/") continue;

                pakFileManifests.Add(fileManifest);
            }

            Output("Validating game files", Type.Info);

            foreach (var gameFile in pakDirectoryInfo.GetFiles())
            {
                if (!gameFile.Exists || gameFile.Name.ToUpper().Contains("UEFN") || gameFile.Name.ToUpper().Contains("UNREALEDITOR") || gameFile.Name.ToUpper().Contains(".O."))
                    continue;

                Output($"Validating {gameFile.Name}", Type.Info);

                var fileManifest = pakFileManifests.Find((FileManifest x) => x.Name.Contains(gameFile.Name));
                if (fileManifest is null)
                {
                    Output($"Deleting: {gameFile.Name} unknown file", Type.Warning);
                    File.Delete(gameFile.FullName);
                    continue;
                }

                if (gameFile.Length > 2090000000)
                {
                    Output($"{gameFile.Name} is a large file and wont be hash checked", Type.Warning);
                    continue;
                }

                using (FileStream fileStream = File.OpenRead(gameFile.FullName))
                {
                    if (SHA1Hash.HashStream(fileStream) == fileManifest.Hash)
                    {
                        Output($"{gameFile.Name} hash Is OK", Type.Info);
                        fileStream.Close();
                    }
                    else
                    {
                        Output($"Deleting: {gameFile.Name} hash Is invalid", Type.Warning);
                        fileStream.Close(); File.Delete(gameFile.FullName);
                    }
                }
            }

            Output("Checking for left over .backup files", Type.Info);

            foreach (var gamefile in pakDirectoryInfo.GetFiles("*.backup*"))
            {
                Output($"Deleting: {gamefile.Name}", Type.Info);
                Directory.Delete(gamefile.FullName, true);
            }

            Output("Scanning for unknown game directories", Type.Info);
            foreach (var gameDirectoryInfo in pakDirectoryInfo.GetDirectories())
            {
                Output($"Deleting: {gameDirectoryInfo.Name}", Type.Info);
                Directory.Delete(gameDirectoryInfo.FullName, true);
            }

            SwapLogs.Clear();
            EpicGamesLauncher.Verify();

            Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"), Languages.Read(Languages.Type.Message, "Verify"), MessageBoxButton.OK);
            Environment.Exit(0);
        }
    }
}