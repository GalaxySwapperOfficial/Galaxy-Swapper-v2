using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Galaxy_Swapper_v2.Workspace.Components
{
    /// <summary>
    /// Interaction logic for CTabControl.xaml
    /// </summary>
    public partial class CTabControl : UserControl
    {
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(CTabControl));

        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconClickedProperty =
            DependencyProperty.Register("IconClicked", typeof(ImageSource), typeof(CTabControl));

        public ImageSource IconClicked
        {
            get { return (ImageSource)GetValue(IconClickedProperty); }
            set { SetValue(IconClickedProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(CTabControl), new PropertyMetadata(Properties.Colors.External_Brush));

        public Brush Color
        {
            get { return (Brush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty PresenceProperty =
            DependencyProperty.Register("Presence", typeof(string), typeof(CNewsControl));

        public string Presence
        {
            get { return (string)GetValue(PresenceProperty); }
            set { SetValue(PresenceProperty, value); }
        }

        public CTabControl()
        {
            InitializeComponent();
        }

        public void Tab_Default()
        {
            this.Clicked.Visibility = Visibility.Hidden;
            this.Default.Visibility = Visibility.Visible;
            this.Color = Properties.Colors.External_Brush;
        }

        public void Tab_Clicked()
        {
            this.Default.Visibility = Visibility.Hidden;
            this.Clicked.Visibility = Visibility.Visible;
            this.Color = Properties.Colors.Blue_Brush;
            Utilities.Presence.Update(Presence);
        }

        private void CTabControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.Width = 42;
            this.Height = 42;
            this.Margin = new Thickness(7);
        }

        private void CTabControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.Width = 40;
            this.Height = 40;
            this.Margin = new Thickness(8);
        }
    }
}