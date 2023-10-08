using Galaxy_Swapper_v2.Workspace.Structs;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        }

        private void Hide_Click(object sender, MouseButtonEventArgs e)
        {
            Show.Visibility = Visibility.Visible;
            Hide.Visibility = Visibility.Hidden;

            Blur.Radius = 20;
            NsfwHeader.Visibility = Visibility.Visible;
        }

        private void Lobby_Convert(object sender, MouseButtonEventArgs e)
        {
            if (LobbyData.IsNsfw && Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Warning"), Languages.Read(Languages.Type.View, "LobbyView", "NSFW"), MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            var Stopwatch = new Stopwatch();
            Stopwatch.Start();

            if (!Directory.Exists(PersistentDownloadDir) || !File.Exists($"{PersistentDownloadDir}\\{DownloadCache}"))
            {
                Log.Error($"Caught exception while converting lobby screen: Directory does not exist: {PersistentDownloadDir}");
                Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Error"), string.Format(Languages.Read(Languages.Type.Message, "PersistentDownloadDirError"), PersistentDownloadDir), discord: true, solutions: Languages.ReadSolutions(Languages.Type.Message, "PersistentDownloadDirError"));
                return;
            }

            string content = File.ReadAllText($"{PersistentDownloadDir}\\{DownloadCache}");

            if (!content.ValidJson())
            {
                Log.Error($"Caught exception while converting lobby screen: DownloadCache.json is not in a valid JSON format");
                Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Error"), Languages.Read(Languages.Type.Message, "DownloadCacheInvalidJSON"), discord: true, solutions: Languages.ReadSolutions(Languages.Type.Message, "DownloadCacheInvalidJSON"));
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
                    try
                    {
                        WC.DownloadFile(LobbyData.Download, filePath);
                    }
                    catch (Exception Exception)
                    {
                        Log.Error(Exception, $"Failed to download {LobbyData.Name}");
                        Message.DisplaySTA("Error", $"Webclient caught a exception while downloading {LobbyData.Name}!", discord: true, solutions: new[] { "Disable Windows Defender Firewall", "Disable any anti-virus softwares", "Turn on a VPN" });
                        return;
                    }
                }
            }

            TimeSpan TimeSpan = Stopwatch.Elapsed;
            if (TimeSpan.Minutes > 0)
                Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"), string.Format(Languages.Read(Languages.Type.View, "SwapView", "ConvertedMinutes"), TimeSpan.Minutes), MessageBoxButton.OK);
            else
                Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"), string.Format(Languages.Read(Languages.Type.View, "SwapView", "Converted"), TimeSpan.Seconds), MessageBoxButton.OK);
        }
    }
}