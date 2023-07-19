using Galaxy_Swapper_v2.Workspace.Usercontrols;
using Galaxy_Swapper_v2.Workspace.Utilities;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Galaxy_Swapper_v2.Workspace.Components
{
    /// <summary>
    /// All the code below was provided from: https://github.com/GalaxySwapperOfficial/Galaxy-Swapper-v2
    /// You can also find us at https://galaxyswapperv2.com/Guilded
    /// </summary>
    public partial class CPluginControl : UserControl
    {
        private Storyboard Storyboard { get; set; } = default!;
        private PluginsView PluginsView { get; set; } = default!;
        public CPluginControl(PluginsView pluginsview)
        {
            InitializeComponent();
            PluginsView = pluginsview;
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(CPluginControl));

        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty PluginPathProperty = DependencyProperty.Register("PluginPath", typeof(string), typeof(CPluginControl));

        public string PluginPath
        {
            get { return (string)GetValue(PluginPathProperty); }
            set { SetValue(PluginPathProperty, value); }
        }

        private void root_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Storyboard != null)
                Storyboard.Stop();

            Storyboard = Interface.SetElementAnimations(new Interface.BaseAnim { Element = Remove, Property = new PropertyPath(Control.OpacityProperty), ElementAnim = new DoubleAnimation() { From = 0, To = 1, Duration = new TimeSpan(0, 0, 0, 0, 200) } });
            Storyboard.Begin();
        }

        private void root_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Storyboard != null)
                Storyboard.Stop();

            Storyboard = Interface.SetElementAnimations(new Interface.BaseAnim { Element = Remove, Property = new PropertyPath(Control.OpacityProperty), ElementAnim = new DoubleAnimation() { From = 1, To = 0, Duration = new TimeSpan(0, 0, 0, 0, 200) } });
            Storyboard.Begin();
        }

        private void Remove_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (File.Exists(PluginPath))
                File.Delete(PluginPath);

            PluginsView.Refresh();
        }
    }
}
