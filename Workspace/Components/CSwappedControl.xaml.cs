using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Galaxy_Swapper_v2.Workspace.Components
{
    /// <summary>
    /// All the code below was provided from: https://github.com/GalaxySwapperOfficial/Galaxy-Swapper-v2
    /// You can also find us at https://galaxyswapperv2.com/Guilded
    /// </summary>
    public partial class CSwappedControl : UserControl
    {
        public CSwappedControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(CSwappedControl));

        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty OverrideIconProperty = DependencyProperty.Register("OverrideIcon", typeof(ImageSource), typeof(CSwappedControl));

        public ImageSource OverrideIcon
        {
            get { return (ImageSource)GetValue(OverrideIconProperty); }
            set { SetValue(OverrideIconProperty, value); }
        }
    }
}
