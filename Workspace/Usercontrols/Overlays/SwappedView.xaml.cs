using Galaxy_Swapper_v2.Workspace.Components;
using Galaxy_Swapper_v2.Workspace.Properties;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Galaxy_Swapper_v2.Workspace.Usercontrols.Overlays
{
    /// <summary>
    /// Interaction logic for SwappedView.xaml
    /// </summary>
    public partial class SwappedView : UserControl
    {
        public SwappedView()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, MouseButtonEventArgs e) => Memory.MainView.RemoveOverlay();
        private void SwappedView_Loaded(object sender, RoutedEventArgs e)
        {
            var Parse = SwapLogs.Cache;
            foreach (var Swapped in Parse)
            {
                try
                {
                    var NewSwappedControl = new CSwappedControl();

                    NewSwappedControl.CosmeticName.Text = Swapped["Name"].Value<string>();
                    NewSwappedControl.TimeStamp.Text = string.Format(Languages.Read(Languages.Type.View, "SwappedView", "TimeStamp"), Swapped["SwappedAt"].Value<string>());
                    NewSwappedControl.AssetCount.Text = string.Format(Languages.Read(Languages.Type.View, "SwappedView", "AssetCount"), Swapped["AssetCount"].Value<int>());

                    var Icon = new BitmapImage();
                    Icon.BeginInit();
                    Icon.UriSource = new Uri(Swapped["Icon"].Value<string>(), UriKind.RelativeOrAbsolute);
                    Icon.CacheOption = BitmapCacheOption.OnLoad;
                    Icon.EndInit();

                    var OverrideIcon = new BitmapImage();
                    OverrideIcon.BeginInit();
                    OverrideIcon.UriSource = new Uri(Swapped["OverrideIcon"].Value<string>(), UriKind.RelativeOrAbsolute);
                    OverrideIcon.CacheOption = BitmapCacheOption.OnLoad;
                    OverrideIcon.EndInit();

                    NewSwappedControl.Icon = Icon;
                    NewSwappedControl.OverrideIcon = OverrideIcon;

                    Converted_Items.Children.Add(NewSwappedControl);
                    Log.Information($"Loaded {NewSwappedControl.CosmeticName.Text}");
                }
                catch (Exception Exception)
                {
                    Log.Error(Exception, "Failed to load cosmetic from swaplogs");
                }
            }

            Log.Information($"Loaded {Parse.Count} cosmetics from swaplogs");
        }
    }
}
