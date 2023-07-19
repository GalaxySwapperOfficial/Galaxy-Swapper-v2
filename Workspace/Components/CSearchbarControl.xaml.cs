using System.Windows;
using System.Windows.Controls;

namespace Galaxy_Swapper_v2.Workspace.Components
{
    /// <summary>
    /// All the code below was provided from: https://github.com/GalaxySwapperOfficial/Galaxy-Swapper-v2
    /// You can also find us at https://galaxyswapperv2.com/Guilded
    /// </summary>
    public partial class CSearchbarControl : UserControl
    {
        public CSearchbarControl()
        {
            InitializeComponent();
        }

        private void Searchbar_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Searchbar.Text == "Search")
                Searchbar.Clear();
        }

        private void Searchbar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Searchbar.Text.Length == 0)
                Searchbar.Text = "Search";
        }
    }
}
