using Galaxy_Swapper_v2.Workspace.Plugins;
using Galaxy_Swapper_v2.Workspace.Structs;
using Galaxy_Swapper_v2.Workspace.Usercontrols;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
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
    /// Interaction logic for CPluginControl.xaml
    /// </summary>
    public partial class CPluginControl : UserControl
    {
        private Storyboard Storyboard { get; set; } = default!;
        private PluginsView PluginsView { get; set; } = default!;
        private PluginData PluginData { get; set; } = default!;
        public CPluginControl(PluginsView pluginsview, PluginData plugindata, string removetip = "Remove", string reimporttip = "Reimport")
        {
            InitializeComponent();
            PluginsView = pluginsview;
            PluginData = plugindata;
            Remove.ToolTip = removetip;
            Import.ToolTip = reimporttip;

            if (!File.Exists(plugindata.Import))
                Import.Visibility = Visibility.Hidden;
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(CPluginControl));

        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        private void root_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Storyboard != null)
                Storyboard.Stop();

            Storyboard = Interface.SetElementAnimations(new Interface.BaseAnim { Element = Remove, Property = new PropertyPath(Control.OpacityProperty), ElementAnim = new DoubleAnimation() { From = 0, To = 1, Duration = new TimeSpan(0, 0, 0, 0, 200) } }, new Interface.BaseAnim { Element = Import, Property = new PropertyPath(Control.OpacityProperty), ElementAnim = new DoubleAnimation() { From = 0, To = 1, Duration = new TimeSpan(0, 0, 0, 0, 200) } });
            Storyboard.Begin();

            Margin = new Thickness(5);
            Height += 10;
            Width += 10;
        }

        private void root_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Storyboard != null)
                Storyboard.Stop();

            Storyboard = Interface.SetElementAnimations(new Interface.BaseAnim { Element = Remove, Property = new PropertyPath(Control.OpacityProperty), ElementAnim = new DoubleAnimation() { From = 1, To = 0, Duration = new TimeSpan(0, 0, 0, 0, 200) } }, new Interface.BaseAnim { Element = Import, Property = new PropertyPath(Control.OpacityProperty), ElementAnim = new DoubleAnimation() { From = 1, To = 0, Duration = new TimeSpan(0, 0, 0, 0, 200) } });
            Storyboard.Begin();

            Margin = new Thickness(10);
            Height -= 10;
            Width -= 10;
        }

        private void Remove_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (File.Exists(PluginData.Path))
                File.Delete(PluginData.Path);

            PluginsView.Refresh();
        }

        private bool IsReImporting = false;
        private void ReImport_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsReImporting) return;

            IsReImporting = true;
            Import.IsEnabled = false;

            if (PluginData.Import is not null && File.Exists(PluginData.Import))
            {
                var fileInfo = new FileInfo(PluginData.Import);
                if (Validate.IsValid(fileInfo, out JObject parse))
                {
                    Plugin.Import(fileInfo, parse);
                }
                else
                {
                    IsReImporting = false;
                    Import.IsEnabled = true;
                    return;
                }
            }
            if (File.Exists(PluginData.Path))
            {
                File.Delete(PluginData.Path);
            }

            PluginsView.Refresh();
        }
    }
}
