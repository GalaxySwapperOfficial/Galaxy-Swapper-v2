using Galaxy_Swapper_v2.Workspace.Utilities;
using Galaxy_Swapper_v2.Workspace.Verify.EpicGames;
using Galaxy_Swapper_v2.Workspace.Verify.EpicManifestParser.Objects;
using Serilog;
using System.ComponentModel;
using System.IO;
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
                        break;
                }

                if (OutputBlock.ActualHeight > 350)
                {
                    OutputBlock.Text = string.Empty;
                }

                OutputBlock.Text += $"[LOG] {content}\n";
            });
        }

        private void Exit() => Memory.MainView.RemoveOverlay();

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

            foreach (var item in manifest.FileManifests) 
            {
                Output(item.Name, Type.Info);
            }

            Log.Information(oauth.Access_Token);
        }
    }
}
