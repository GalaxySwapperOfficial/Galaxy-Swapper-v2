using Galaxy_Swapper_v2.Workspace.Hashes;
using Galaxy_Swapper_v2.Workspace.Properties;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Galaxy_Swapper_v2.Workspace.Verify.EpicGames;
using Galaxy_Swapper_v2.Workspace.Verify.EpicManifestParser.Objects;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
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
            var manifestPaths = new Dictionary<string, List<FileManifest>>();

            foreach (var fileManifest in manifest.FileManifests)
            {
                var directoryPath = fileManifest.Name.SubstringBeforeWithLast('/');

                if (manifestPaths.ContainsKey(directoryPath))
                {
                    manifestPaths[directoryPath].Add(fileManifest);
                }
                else
                {
                    manifestPaths[directoryPath] = new List<FileManifest>() { fileManifest };
                }
            }

            Output($"Validating game files in {manifestPaths.Count} directories", Type.Info);

            string paks = $"{Settings.Read(Settings.Type.Installtion)}";

            var failed = new List<string>();
            var success = new List<string>();

            foreach (var directoryPath in manifestPaths)
            {
                var directoryInfo = new DirectoryInfo($"{paks}\\{directoryPath.Key}");

                if (!directoryInfo.Exists) continue;

                foreach (var file in directoryInfo.GetFiles())
                {
                    if (ShouldSkip(file)) continue;

                    var fileManifest = directoryPath.Value.Find((FileManifest x) => x.Name.Contains(file.Name));
                    if (fileManifest is null)
                    {
                        Output($"Unknown file: {file.Name}", Type.Warning);
                        failed.Add($"{file.FullName}");
                        continue;
                    }

                    if (file.Length > 2090000000)
                    {
                        Output($"{file.Name} is a large file and may take a few minutes to verify", Type.Warning);
                        continue;
                    }

                    Output($"Validating {file.Name} hash", Type.Info);

                    using (FileStream fileStream = File.OpenRead(file.FullName))
                    {
                        if (SHA1Hash.HashStream(fileStream) == fileManifest.Hash)
                        {
                            Output($"{file.Name} hash Is valid", Type.Info);
                            success.Add(file.FullName);
                        }
                        else
                        {
                            Output($"{file.Name} hash Is invalid", Type.Warning);
                            failed.Add(file.FullName);
                        }
                    }
                }
            }

            if (Message.DisplaySTA("Info", string.Format("{0} files are valid.\n{1} files are invalid.\n\nWould you like to remove ALL invalid game files? If you agree the invalid files will be completely deleted\nThen we will redirect you to the Epic Games Launcher to reinstall these game files.", success.Count, failed.Count), System.Windows.MessageBoxButton.YesNoCancel) == System.Windows.MessageBoxResult.Yes)
            {
                foreach (string file in failed)
                {
                    if (!Misc.CanEdit(file))
                    {
                        Output($"{file} Is currently In use and can not be removed!", Type.Error);
                        continue;
                    }

                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception Exception)
                    {
                        Output($"Failed to delete {file}\n{Exception.Message}", Type.Error);
                        continue;
                    }

                    Output($"Deleted {file}", Type.Info);
                }
            }
        }

        private bool ShouldSkip(FileInfo file)
        {
            if (!file.Exists || file.Name.ToUpper().Contains("UEFN") || file.Name.ToUpper().Contains("UNREALEDITOR") || file.Name.ToUpper().Contains(".O."))
                return true;
            else if (file.Extension.ToUpper() == ".DLL")
            {
                foreach (string whiteListedDLL in new string[] { "SHADERCOMPILEWORKER", "AGENTINTERFACE", "BOOST_", "DXCOMPILER", "EMBREE3", "KITT-", "KITT_", "OO2CORE", "OO2TEX", "OPENCOLORIO", "RAD_TM", "D3D12SDKLAYERS" })
                {
                    if (file.Name.ToUpper().Contains(whiteListedDLL))
                        return true;
                }
            }
            else if (file.Extension.ToUpper() == ".EXE")
            {
                foreach (string whiteListedEXE in new string[] { "CRASHREPORT", "XGECONTROLWORKER", "URC", "SHADERCOMPILEWORKER" })
                {
                    if (file.Name.ToUpper().Contains(whiteListedEXE))
                        return true;
                }
            }

            return false;
        }
    }
}