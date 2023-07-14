using CUE4Parse.UE4.IO;
using Galaxy_Swapper_v2.Workspace.Properties;
using Galaxy_Swapper_v2.Workspace.Usercontrols.Overlays;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Galaxy_Swapper_v2.Workspace.Usercontrols
{
    /// <summary>
    /// Interaction logic for SplashView.xaml
    /// </summary>
    public partial class SplashView : UserControl
    {
        public SplashView()
        {
            InitializeComponent();
        }

        private Storyboard Storyboard { get; set; } = default!;
        private void ProgressAnimation()
        {
            Storyboard = Interface.SetThicknessAnimations(new Interface.ThicknessAnim() { Element = Progess, ElementAnim = new ThicknessAnimation { From = new Thickness(9, 646, 953, 0), To = new Thickness(953, 646, 9, 0), Duration = new TimeSpan(0, 0, 0, 2, 0) } });
            Storyboard.Completed += delegate
            {
                Storyboard = Interface.SetThicknessAnimations(new Interface.ThicknessAnim() { Element = Progess, ElementAnim = new ThicknessAnimation { From = new Thickness(953, 646, 9, 0), To = new Thickness(9, 646, 953, 0), Duration = new TimeSpan(0, 0, 0, 2, 0) } });
                Storyboard.Completed += delegate
                {
                    ProgressAnimation();
                };
                Storyboard.Begin();
            };
            Storyboard.Begin();
        }

        private BackgroundWorker LoadWorker = new BackgroundWorker();
        private void SplashView_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressAnimation();

            LoadWorker.DoWork += LoadWorker_DoWork;
            LoadWorker.RunWorkerCompleted += LoadWorker_RunWorkerCompleted;
            LoadWorker.RunWorkerAsync();
        }

        private void LoadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var LastPos = Progess.Margin;

            Storyboard.Stop();

            Progess.Margin = LastPos;

            if (!Account.Valid())
            {
                Log.Information("Sending to LoginView");
                Memory.MainView.SetOverlay(new LoginView());
                return;
            }

            Memory.MainView.Main.Visibility = Visibility.Visible;
            Memory.MainView.Tab_Click(Memory.MainView.Dashboard, null!);

            DateTime CurrentDate = DateTime.Now;
            if (Settings.Read(Settings.Type.Reminded).Value<string>() != CurrentDate.ToString("dd/MM/yyyy"))
            {
                Log.Information("Reminded date is invalid displaying DiscordView");
                Settings.Edit(Settings.Type.Reminded, CurrentDate.ToString("dd/MM/yyyy"));
                Memory.MainView.SetOverlay(new DiscordView());
            }
            else
                Memory.MainView.RemoveOverlay();
        }

        private void LoadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var Parse = Endpoint.Read(Endpoint.Type.Version);

            Global.Discord = Parse["Discord"].Value<string>();
            Global.Website = Parse["Website"].Value<string>();
            Global.Download = Parse["Download"].Value<string>();
            Global.Key = Parse["Key"].Value<string>();

            Parse = Parse[Global.Version];

            if (Parse["Warning"]["Enabled"].Value<bool>())
                Message.DisplaySTA(Parse["Warning"]["Header"].Value<string>(), Parse["Warning"]["Content"].Value<string>(), MessageBoxButton.OK);

            if (Parse["DownTime"]["Enabled"].Value<bool>())
                Message.DisplaySTA(Parse["DownTime"]["Header"].Value<string>(), Parse["DownTime"]["Content"].Value<string>(), MessageBoxButton.OK, new List<string> { Global.Discord }, close: true);

            if (Parse["Update"]["Enabled"].Value<bool>())
            {
                if (Parse["Update"]["Force"]["Enabled"].Value<bool>())
                    Message.DisplaySTA(Parse["Update"]["Force"]["Header"].Value<string>(), Parse["Update"]["Force"]["Content"].Value<string>(), MessageBoxButton.OK, new List<string> { Global.Discord, Global.Download, Global.Website }, close: true);
                else if (Message.DisplaySTA(Parse["Update"]["Header"].Value<string>(), Parse["Update"]["Content"].Value<string>(), MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
                {
                    Global.Discord.UrlStart();
                    Global.Download.UrlStart();
                    Global.Website.UrlStart();
                    Environment.Exit(0);
                }    
            }

            Presence.Initialize();

            OverwriteOldData();

            string Installation = $"{Settings.Read(Settings.Type.Installtion).Value<string>()}\\FortniteGame\\Content\\Paks";

            if (string.IsNullOrEmpty(Installation))
                return;
            else if (!Directory.Exists(Installation))
            {
                Settings.Edit(Settings.Type.Installtion, string.Empty);
                return;
            }

            bool Notify = false;

            try
            {
                foreach (string PakChunk in Directory.GetFiles(Installation))
                {
                    if (PakChunk.ToLower().Contains("saturn") || PakChunk.ToLower().Contains("twilight") || PakChunk.ToLower().Contains("proswapper"))
                    {
                        if (!Notify)
                        {
                            if (Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Question"), Languages.Read(Languages.Type.Message, "UnknownBackup"), MessageBoxButton.YesNo) == MessageBoxResult.No)
                                return;
                            Notify = true;
                        }

                        File.Delete(PakChunk);
                    }
                }

                if (Directory.Exists($"{Installation}\\Elytra Swapper"))
                {
                    if (!Notify && Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Question"), Languages.Read(Languages.Type.Message, "UnknownBackup"), MessageBoxButton.YesNo) == MessageBoxResult.No)
                        return;

                    Directory.Delete($"{Installation}\\Elytra Swapper", true);
                }
                if (Directory.Exists($"{Installation}\\Galaxy Swapper v2"))
                {
                    Directory.Delete($"{Installation}\\Galaxy Swapper v2", true);
                }
                if (Directory.Exists($"{Installation}\\Apple"))
                {
                    Directory.Delete($"{Installation}\\Apple", true);
                }
            }
            catch
            {
                Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Error"), Languages.Read(Languages.Type.Message, "UnknownBackupError"), MessageBoxButton.OK, new List<string> { Global.Discord }, Languages.ReadSolutions(Languages.Type.Message, "UnknownBackupError"));
            }
        }

        //This will be removed once Fortnite updates.
        private void OverwriteOldData()
        {
            if (SwapData.Cache.Count == 0)
                return;

            Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"), string.Format(Languages.Read(Languages.Type.Message, "RemovedSwapData"), Global.Version), MessageBoxButton.OK);

            EpicGamesLauncher.Close();

            Log.Information("Starting to remove SwapData");
            try
            {
                foreach (var Asset in SwapData.Cache)
                {
                    if (!Asset["CompressionBlock"].KeyIsNullOrEmpty() && !Asset["ChunkOffsetAndLengths"].KeyIsNullOrEmpty())
                    {
                        string iostore = Asset["ChunkOffsetAndLengths"]["Path"].Value<string>();

                        if (!File.Exists(iostore))
                        {
                            Log.Warning($"Iostore no longer exist:\n{iostore}");
                            continue;
                        }

                        using (BinaryReader iostorereader = new BinaryReader(File.Open(iostore, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)))
                        {
                            iostorereader.BaseStream.Position = Asset["ChunkOffsetAndLengths"]["Offset"].Value<long>();

                            byte[] buffer = iostorereader.ReadBytes(5);
                            byte[] buffer2 = Compression.Decompress(Asset["ChunkOffsetAndLengths"]["Buffer"].Value<string>()).Take(5).ToArray();

                            if (!buffer.SequenceEqual(buffer2))
                            {
                                Log.Warning($"{Asset["ObjectPath"].Value<string>()} is outdated. Skipping!");
                                continue;
                            }

                            iostorereader.Close();
                        }

                        using (BinaryWriter iostorewriter = new BinaryWriter(File.Open(iostore, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)))
                        {
                            iostorewriter.BaseStream.Position = Asset["ChunkOffsetAndLengths"]["Offset"].Value<long>();
                            iostorewriter.Write(Compression.Decompress(Asset["ChunkOffsetAndLengths"]["Buffer"].Value<string>()));

                            iostorewriter.BaseStream.Position = Asset["CompressionBlock"]["Offset"].Value<long>();
                            iostorewriter.Write(Compression.Decompress(Asset["CompressionBlock"]["Buffer"].Value<string>()));

                            iostorewriter.Close();
                        }
                    }
                }

                SwapData.Delete();
                SwapLogs.Clear();
            }
            catch (Exception Exception)
            {
                Log.Error(Exception.Message);

                Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"), string.Format(Languages.Read(Languages.Type.Message, "RemovedSwapDataError"), Global.Version), MessageBoxButton.OK);
                SwapData.Delete();
                SwapLogs.Clear();
                EpicGamesLauncher.Verify();
                Environment.Exit(0);
            }
        }

        private void Drag_Click(object sender, MouseButtonEventArgs e) => Memory.MainView.DragMove();
    }
}