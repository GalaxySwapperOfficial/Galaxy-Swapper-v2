using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Galaxy_Swapper_v2.Workspace.Components
{
    /// <summary>
    /// Interaction logic for CCloseControl.xaml
    /// </summary>
    public partial class CCloseControl : UserControl
    {
        public CCloseControl()
        {
            InitializeComponent();
        }

        private void Close_Hover(object sender, MouseEventArgs e) => Close.Background = Properties.Colors.Red;

        private void Close_Leave(object sender, MouseEventArgs e) => Close.Background = Brushes.Transparent;
    }
}
