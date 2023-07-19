using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Galaxy_Swapper_v2.Workspace.Components
{
    /// <summary>
    /// All the code below was provided from: https://github.com/GalaxySwapperOfficial/Galaxy-Swapper-v2
    /// You can also find us at https://galaxyswapperv2.com/Guilded
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
