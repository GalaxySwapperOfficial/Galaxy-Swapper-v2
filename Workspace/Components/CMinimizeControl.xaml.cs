using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Galaxy_Swapper_v2.Workspace.Components
{
    /// <summary>
    /// Interaction logic for CMinimizeControl.xaml
    /// </summary>
    public partial class CMinimizeControl : UserControl
    {
        public CMinimizeControl()
        {
            InitializeComponent();
        }

        private void Minimize_Hover(object sender, MouseEventArgs e) => Minimize.Background = Properties.Colors.ControlHover_Brush;

        private void Minimize_Leave(object sender, MouseEventArgs e) => Minimize.Background = Brushes.Transparent;
    }
}