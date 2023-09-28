using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Galaxy_Swapper_v2.Workspace.Components
{
    /// <summary>
    /// Interaction logic for CSwappedControl.xaml
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
