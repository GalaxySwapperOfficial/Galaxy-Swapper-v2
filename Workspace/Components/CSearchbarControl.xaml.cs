using System.Windows;
using System.Windows.Controls;

namespace Galaxy_Swapper_v2.Workspace.Components
{
    /// <summary>
    /// Interaction logic for CSearchbarControl.xaml
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
