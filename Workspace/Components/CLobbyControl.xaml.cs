using Galaxy_Swapper_v2.Workspace.Structs;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Galaxy_Swapper_v2.Workspace.Components
{
    public partial class CLobbyControl : UserControl
    {
        private readonly string PersistentDownloadDir = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\FortniteGame\\Saved\\PersistentDownloadDir\\CMS";
        private readonly string DownloadCache = "DownloadCache.json";
        public LobbyData LobbyData { get; set; }
        public CLobbyControl(LobbyData lobbydata)
        {
            InitializeComponent();
            LobbyData = lobbydata;
            Logo.LoadImage(lobbydata.Preview);
            Logo.ToolTip = lobbydata.Name;

            if (!lobbydata.IsNsfw)
            {
                Blur.Radius = 0;
                NsfwHeader.Visibility = Visibility.Hidden;
                Show.Visibility = Visibility.Hidden;
                NsfwHeader.IsEnabled = false;
                Show.IsEnabled = false;
            }
        }

        private void root_MouseEnter(object sender, MouseEventArgs e)
        {
            Margin = new Thickness(5);
            Height += 10;
            Width += 10;
        }

        private void root_MouseLeave(object sender, MouseEventArgs e)
        {
            Margin = new Thickness(10);
            Height -= 10;
            Width -= 10;
        }

        private void Show_Click(object sender, MouseButtonEventArgs e)
        {
            Hide.Visibility = Visibility.Visible;
            Show.Visibility = Visibility.Hidden;

            Blur.Radius = 0;
            NsfwHeader.Visibility = Visibility.Hidden;
            NsfwHeader.IsEnabled = false;
        }

        private void Hide_Click(object sender, MouseButtonEventArgs e)
        {
            Show.Visibility = Visibility.Visible;
            Hide.Visibility = Visibility.Hidden;

            Blur.Radius = 20;
            NsfwHeader.Visibility = Visibility.Visible;
            NsfwHeader.IsEnabled = true;
        }

        private void Lobby_Convert(object sender, MouseButtonEventArgs e)
        {
            if (LobbyData.IsNsfw && Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Warning"), Languages.Read(Languages.Type.View, "LobbyView", "NSFW"), MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

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

                Misc.Download(filePath, LobbyData.Download, "Lobby");
            }
        }
    }
}