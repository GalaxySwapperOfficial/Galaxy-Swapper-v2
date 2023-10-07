using Galaxy_Swapper_v2.Workspace.Structs;
using Galaxy_Swapper_v2.Workspace.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Galaxy_Swapper_v2.Workspace.Components
{
    /// <summary>
    /// Interaction logic for CLobbyControl.xaml
    /// </summary>
    public partial class CLobbyControl : UserControl
    {
        public LobbyData LobbyData { get; set; }
        public CLobbyControl(LobbyData lobbydata)
        {
            InitializeComponent();
            LobbyData = lobbydata;
            Logo.LoadImage(lobbydata.Preview);
            Logo.ToolTip = lobbydata.Name;
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
    }
}