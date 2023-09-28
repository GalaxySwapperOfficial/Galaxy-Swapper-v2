using Galaxy_Swapper_v2.Workspace.Usercontrols.Overlays;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Galaxy_Swapper_v2.Workspace.Usercontrols
{
    /// <summary>
    /// Interaction logic for MiscView.xaml
    /// </summary>
    public partial class MiscView : UserControl
    {
        public MiscView()
        {
            InitializeComponent();
        }

        private void MiscView_Loaded(object sender, RoutedEventArgs e)
        {
            var FOV = Endpoint.Read(Endpoint.Type.FOV);

            if (FOV["Enabled"].Value<bool>())
            {
                FOVBorder.Visibility = Visibility.Visible;
                FOVIcon.LoadImage(FOV["Icon"].Value<string>());
                FOVTextblock.Text = Languages.Read(Languages.Type.View, "FovView", "Frontend");
            }
        }

        private void FOV_Click(object sender, System.Windows.Input.MouseButtonEventArgs e) => Memory.MainView.SetOverlay(Memory.FovView);
    }
}
