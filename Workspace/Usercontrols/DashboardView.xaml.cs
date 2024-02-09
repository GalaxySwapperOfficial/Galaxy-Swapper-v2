using Galaxy_Swapper_v2.Workspace.Components;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Galaxy_Swapper_v2.Workspace.Usercontrols
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
        }

        private Storyboard Storyboard { get; set; } = default!;
        private Storyboard Storyboard2 { get; set; } = default!;

        private void Banner_MouseEnter(object sender, MouseEventArgs e)
        {
            Storyboard2.Stop();
            Storyboard.Begin();
            ReadMore.Visibility = Visibility.Visible;
        }

        private void Banner_MouseLeave(object sender, MouseEventArgs e)
        {
            Storyboard.Stop();
            Storyboard2.Begin();
        }

        private void Banner_Click(object sender, MouseButtonEventArgs e) => Memory.MainView.SetOverlay(Memory.NotesView);

        private bool IsLoaded = false;
        private void DashboardView_Loaded(object sender, RoutedEventArgs e)
        {
            Version.Text = string.Format(Languages.Read(Languages.Type.View, "DashboardView", "Version"), Global.Version);
            ReadMore.Text = Languages.Read(Languages.Type.View, "DashboardView", "ReadMore");

            if (IsLoaded)
                return;

            Storyboard = Interface.SetThicknessAnimations(new Interface.ThicknessAnim() { Element = ReadMore, ElementAnim = new ThicknessAnimation { To = new Thickness(0, 0, 12, 7), Duration = new TimeSpan(0, 0, 0, 0, 200), EasingFunction = new QuadraticEase() } });
            Storyboard2 = Interface.SetThicknessAnimations(new Interface.ThicknessAnim() { Element = ReadMore, ElementAnim = new ThicknessAnimation { To = new Thickness(0, 0, 0, 7), Duration = new TimeSpan(0, 0, 0, 0, 200), EasingFunction = new QuadraticEase() } });

            Storyboard2.Completed += delegate
            {
                ReadMore.Visibility = Visibility.Hidden;
            };

            var versionParse = Endpoint.Read(Endpoint.Type.Version);

            if (!versionParse[Global.Version]["Banner"].KeyIsNullOrEmpty())
            {
                BannerImage.LoadImage(versionParse[Global.Version]["Banner"].Value<string>(), invalid: "/WorkSpace/Assets/Banner.png");
            }

            var Parse = Endpoint.Read(Endpoint.Type.News);

            foreach (var news in Parse)
            {
                var newInfo = new CNewsControl();

                if (!news["Header"].KeyIsNullOrEmpty())
                    newInfo.Header = news["Header"].Value<string>();

                if (!news["Description"].KeyIsNullOrEmpty())
                    newInfo.Description = news["Description"].Value<string>();

                if (!news["URL"].KeyIsNullOrEmpty())
                {
                    newInfo.URL = news["URL"].Value<string>();
                    newInfo.Embed.Visibility = Visibility.Visible;
                    newInfo.Cursor = Cursors.Hand;
                    newInfo.HeaderControl.Cursor = Cursors.Hand;
                    newInfo.DescriptionControl.Cursor = Cursors.Hand;
                }

                if (!news["IMG"].KeyIsNullOrEmpty())
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(news["IMG"].Value<string>(), UriKind.RelativeOrAbsolute);
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();

                    newInfo.News = image;
                }

                Wrap.Children.Add(newInfo);
            }

            if (Wrap.Children.Count < 3)
                Viewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;

            IsLoaded = true;
        }
    }
}
