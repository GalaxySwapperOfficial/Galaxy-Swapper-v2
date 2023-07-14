using Galaxy_Swapper_v2.Workspace.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Galaxy_Swapper_v2.Workspace.Usercontrols.Overlays
{
    /// <summary>
    /// Interaction logic for DiscordView.xaml
    /// </summary>
    public partial class DiscordView : UserControl
    {
        public DiscordView()
        {
            InitializeComponent();
        }

        private void DiscordView_Loaded(object sender, RoutedEventArgs e)
        {
            Header.Text = Languages.Read(Languages.Type.View, "DiscordView", "Header");
            Tip_1.Text = Languages.Read(Languages.Type.View, "DiscordView", "Tip_1");
            Tip_2.Text = Languages.Read(Languages.Type.View, "DiscordView", "Tip_2");
        }

        private void Close_Click(object sender, MouseButtonEventArgs e) => Memory.MainView.RemoveOverlay();
        private void Discord_Click(object sender, MouseButtonEventArgs e) => Global.Discord.UrlStart();
    }
}
