using Galaxy_Swapper_v2.Workspace.Structs;
using Galaxy_Swapper_v2.Workspace.Swapping;
using Galaxy_Swapper_v2.Workspace.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Galaxy_Swapper_v2.Workspace.Components
{
    public partial class CLobbyControl : UserControl
    {
        public LobbyData LobbyData { get; set; }
        public CLobbyControl(LobbyData lobbydata)
        {
            InitializeComponent();
            LobbyData = lobbydata;
            Logo.LoadImage(lobbydata.Preview);
            Logo.ToolTip = lobbydata.Name;
            NsfwHeader.Text = Languages.Read(Languages.Type.View, "LobbyView", "NSFWHeader");

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

            LobbyBGSwap.Convert(LobbyData.Download);
        }
    }
}