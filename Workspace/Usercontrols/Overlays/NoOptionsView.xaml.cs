using Galaxy_Swapper_v2.Workspace.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Galaxy_Swapper_v2.Workspace.Usercontrols.Overlays
{
    /// <summary>
    /// Interaction logic for NoOptionsView.xaml
    /// </summary>
    public partial class NoOptionsView : UserControl
    {
        public NoOptionsView()
        {
            InitializeComponent();
        }

        private void NoOptionsView_Loaded(object sender, RoutedEventArgs e)
        {
            Header.Text = Languages.Read(Languages.Type.View, "NoOptionsView", "Header");
            Tip_1.Text = Languages.Read(Languages.Type.View, "NoOptionsView", "Tip_1");
            Tip_2.Text = Languages.Read(Languages.Type.View, "NoOptionsView", "Tip_2");
            Tip_3.Text = Languages.Read(Languages.Type.View, "NoOptionsView", "Tip_3");
        }

        private void Close_Click(object sender, MouseButtonEventArgs e) => Memory.MainView.RemoveOverlay();
        private void Discord_Click(object sender, MouseButtonEventArgs e) => Global.Discord.UrlStart();
    }
}
