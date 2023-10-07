using Galaxy_Swapper_v2.Workspace.Components;
using Galaxy_Swapper_v2.Workspace.Properties;
using Galaxy_Swapper_v2.Workspace.Structs;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Galaxy_Swapper_v2.Workspace.Usercontrols.Overlays
{
    public partial class LobbyView : UserControl
    {
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
                    Name = item["Name"].Value<string>(),
                    IsNsfw = item["IsNsfw"].Value<bool>()
                };

                if (newLobby.IsNsfw && Settings.Read(Settings.Type.HideNsfw).Value<bool>())
                    continue;

                var newLobbyControl = new CLobbyControl(newLobby) { Margin = new Thickness(10) };
                Options_Items.Children.Add(newLobbyControl);
            }
        }

        private void Close_Click(object sender, MouseButtonEventArgs e) => Memory.MainView.RemoveOverlay();
    }
}
