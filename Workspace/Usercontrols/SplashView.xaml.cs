using Galaxy_Swapper_v2.Workspace.Properties;
using Galaxy_Swapper_v2.Workspace.Usercontrols.Overlays;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.ComponentModel;
using System.IO;
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
            var parse = Endpoint.Read(Endpoint.Type.Version);

            Global.Discord = parse["Discord"].Value<string>();
            Global.Website = parse["Website"].Value<string>();
            Global.Download = parse["Download"].Value<string>();
            Global.Key = parse["Key"].Value<string>();

            parse = parse[Global.Version];
            
            if (parse["Warning"]["Enabled"].Value<bool>())
                Message.DisplaySTA(parse["Warning"]["Header"].Value<string>(), parse["Warning"]["Content"].Value<string>(), MessageBoxButton.OK);

            if (parse["DownTime"]["Enabled"].Value<bool>() && !Settings.Read(Settings.Type.IsDev).Value<bool>())
                Message.DisplaySTA(parse["DownTime"]["Header"].Value<string>(), parse["DownTime"]["Content"].Value<string>(), discord: true, exit: true);

            // Der Code, der die Update-Nachricht anzeigt, wurde entfernt

            Presence.Initialize();

            string installation = $"{Settings.Read(Settings.Type.Installtion).Value<string>()}\\FortniteGame\\Content\\Paks";

            if (!string.IsNullOrEmpty(installation) && !Directory.Exists(installation))
            {
                Settings.Edit(Settings.Type.Installtion, string.Empty);
                return;
            }
        }

        private void Drag_Click(object sender, MouseButtonEventArgs e) => Memory.MainView.DragMove();
    }
}
