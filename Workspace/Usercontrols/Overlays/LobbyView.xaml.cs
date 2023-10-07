using Galaxy_Swapper_v2.Workspace.Components;
using Galaxy_Swapper_v2.Workspace.Structs;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace Galaxy_Swapper_v2.Workspace.Usercontrols.Overlays
{
    public partial class LobbyView : UserControl
    {
        private readonly string PersistentDownloadDir = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\FortniteGame\\Saved\\PersistentDownloadDir\\CMS";
        private readonly string DownloadCache = "DownloadCache.json";
        public LobbyView()
        {
            InitializeComponent();
        }

        private void LobbyView_Loaded(object sender, RoutedEventArgs e)
        {
            var parse = Endpoint.Read(Endpoint.Type.Lobby);
            foreach (var item in parse["Array"])
            {
                var newLobby = new LobbyData()
                {
                    Preview = item["Preview"].Value<string>(),
                    Download = item["Download"].Value<string>(),
                    Name = item["Name"].Value<string>()
                };
                var newLobbyControl = new CLobbyControl(newLobby) { Margin = new Thickness(10) };
                newLobbyControl.MouseLeftButtonDown += (s, args) => Lobby_Convert(s, args, newLobby);
                Options_Items.Children.Add(newLobbyControl);
            }
        }

        private void Lobby_Convert(object sender, MouseButtonEventArgs e, LobbyData lobbydata)
        {
            if (!Directory.Exists(PersistentDownloadDir))
            {
                return;
            }
            if (!File.Exists($"{PersistentDownloadDir}\\{DownloadCache}"))
            {
                return;
            }

            string content = File.ReadAllText($"{PersistentDownloadDir}\\{DownloadCache}");

            if (!content.ValidJson())
            {
                return;
            }

            var parse = JObject.Parse(content);

            foreach (var cache in parse["cache"] as JObject)
            {
                string key = cache.Key;

                if (!key.ToLower().Contains("lobby"))
                    continue;

                string filePath = cache.Value["filePath"].Value<string>();

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                using (WebClient WC = new WebClient())
                {
                    WC.DownloadFile(lobbydata.Download, filePath);
                }
            }
        }

        private void Close_Click(object sender, MouseButtonEventArgs e) => Memory.MainView.RemoveOverlay();
    }
}
