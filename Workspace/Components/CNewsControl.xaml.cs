using Galaxy_Swapper_v2.Workspace.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Galaxy_Swapper_v2.Workspace.Components
{
    /// <summary>
    /// Interaction logic for CNewsControl.xaml
    /// </summary>
    public partial class CNewsControl : UserControl
    {
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(CNewsControl));

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(CNewsControl));

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty URLProperty =
            DependencyProperty.Register("URL", typeof(string), typeof(CNewsControl));

        public string URL
        {
            get { return (string)GetValue(URLProperty); }
            set { SetValue(URLProperty, value); }
        }

        public static readonly DependencyProperty NewsProperty =
            DependencyProperty.Register("News", typeof(ImageSource), typeof(CNewsControl));

        public ImageSource News
        {
            get { return (ImageSource)GetValue(NewsProperty); }
            set { SetValue(NewsProperty, value); }
        }

        public CNewsControl()
        {
            InitializeComponent();
        }

        private void News_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.URL))
                this.URL.UrlStart();
        }
    }
}
