using Galaxy_Swapper_v2.Workspace.Components;
using Galaxy_Swapper_v2.Workspace.Generation.Formats;
using Galaxy_Swapper_v2.Workspace.Plugins;
using Galaxy_Swapper_v2.Workspace.Structs;
using Galaxy_Swapper_v2.Workspace.Usercontrols.Overlays;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Galaxy_Swapper_v2.Workspace.Views;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Galaxy_Swapper_v2.Workspace.Usercontrols
{
    /// <summary>
    /// Interaction logic for PluginsView.xaml
    /// </summary>
    public partial class PluginsView : UserControl
    {
        public PluginsView()
        {
            InitializeComponent();
        }

        private void Plugins_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string file in files)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.Exists && Validate.IsValid(fileInfo, out JObject parse))
                    {
                        Plugin.Import(fileInfo, parse);
                    }
                }

                Refresh();
            }
        }

        private bool IsLoaded = false;
        private void PluginsView_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
                return;

            Header.Text = Languages.Read(Languages.Type.View, "PluginsView", "Header");
            Tip_1.Text = Languages.Read(Languages.Type.View, "PluginsView", "Tip");
            Tip_2.Text = Languages.Read(Languages.Type.View, "PluginsView", "Tip_2");

            Refresh();

            IsLoaded = true;
        }

        public void Refresh()
        {
            var Stopwatch = new Stopwatch();
            Stopwatch.Start();

            if (Plugins_Items.Children.Count != 0)
                Plugins_Items.Children.Clear();

            foreach (var plugindata in Plugin.GetPlugins())
            {
                Plugins_Items.Children.Add(CreateCosmetic(plugindata));
            }

            if (Plugins_Items.Children.Count == 0)
            {
                Header.Visibility = Visibility.Visible;
                Tip.Visibility = Visibility.Visible;
            }
            else
            {
                Header.Visibility = Visibility.Hidden;
                Tip.Visibility = Visibility.Hidden;
            }

            Log.Information($"Loaded plugins in {Stopwatch.Elapsed.TotalSeconds} seconds, With {Plugins_Items.Children.Count} items!");
        }

        private CPluginControl CreateCosmetic(PluginData plugindata)
        {
            var parse = plugindata.Parse;
            string icon = null!;
            string frontendicon = null!;
            string swapicon = null!;
            string name = parse["Name"].Value<string>();
            var newcomsetic = new CPluginControl(this, plugindata, Languages.Read(Languages.Type.View, "PluginsView", "Remove")) { Height = 85, Width = 85, Margin = new Thickness(10), Cursor = Cursors.Hand, ToolTip = name };
            
            if (!parse["FrontendIcon"].KeyIsNullOrEmpty())
                frontendicon = parse["FrontendIcon"].Value<string>();
            if (!parse["Icon"].KeyIsNullOrEmpty())
                icon = parse["Icon"].Value<string>();
            if (!parse["Swapicon"].KeyIsNullOrEmpty())
                swapicon = parse["Swapicon"].Value<string>();

            if (frontendicon is not null)
            {
                newcomsetic.Icon = Misc.LoadImageToBitmap(frontendicon);
            }
            else if (icon is not null)
            {
                newcomsetic.Icon = Misc.LoadImageToBitmap(icon);
            }
            else
            {
                throw new Exception($"'FrontendIcon' and Icon was invalid! Failed to load image from plugin file:\n{plugindata.Path}");
            }

            newcomsetic.Cosmetic.MouseLeftButtonDown += delegate
            {
                var newoption = new Option() { Name = name, OverrideIcon = icon, Message = parse["Message"].Value<string>() };

                //These functions can load no matter what type it is
                if (parse["Downloadables"] != null)
                {
                    foreach (var downloadable in parse["Downloadables"])
                    {
                        if (!downloadable["zip"].KeyIsNullOrEmpty())
                        {
                            newoption.Downloadables.Add(new() { Zip = downloadable["zip"].Value<string>() });
                        }
                        else if (!downloadable["pak"].KeyIsNullOrEmpty() && !downloadable["sig"].KeyIsNullOrEmpty() && !downloadable["ucas"].KeyIsNullOrEmpty() && !downloadable["utoc"].KeyIsNullOrEmpty())
                            newoption.Downloadables.Add(new Downloadable() { Pak = downloadable["pak"].Value<string>(), Sig = downloadable["sig"].Value<string>(), Ucas = downloadable["ucas"].Value<string>(), Utoc = downloadable["utoc"].Value<string>() });
                    }
                }

                if (!parse["Nsfw"].KeyIsNullOrEmpty())
                    newoption.Nsfw = parse["Nsfw"].Value<bool>();

                if (!parse["UseMainUEFN"].KeyIsNullOrEmpty())
                    newoption.UseMainUEFN = parse["UseMainUEFN"].Value<bool>();

                //If type is default or null (which would be default?)
                if (parse["Type"].KeyIsNullOrEmpty() || parse["Type"].Value<string>() == "default" || parse["Type"].Value<string>() == "Skin:Mesh")
                {
                    newoption.Icon = swapicon;

                    foreach (var asset in parse["Assets"])
                    {
                        var newasset = new Asset() { Object = asset["AssetPath"].Value<string>() };

                        if (!asset["AssetPathTo"].KeyIsNullOrEmpty())
                            newasset.OverrideObject = asset["AssetPathTo"].Value<string>();
                        if (asset["Buffer"] != null)
                            newasset.OverrideBuffer = asset["Buffer"].Value<string>();
                        if (asset["Swaps"] != null)
                            newasset.Swaps = asset["Swaps"];

                        newoption.Exports.Add(newasset);
                    }

                    ((MainView)App.Current.MainWindow).SetOverlay(new SwapView(newoption));
                    return;
                }
                
                //Check for custom types like uefn fix, defaults ect!
                switch (parse["Type"].Value<string>())
                {
                    case "UEFN_Character":
                        var optionlist = new List<Option>();
                        var uefn = Endpoint.Read(Endpoint.Type.UEFN);

                        newoption.UEFNFormat = true; //set it here so we don't need to keep doing it.

                        foreach (var option in uefn["Swaps"])
                        {
                            var uefnoption = (Option)newoption.Clone();

                            uefnoption.Name = $"{option["Name"].Value<string>()} to {newoption.Name}";
                            uefnoption.Exports = new List<Asset>();

                            if (!option["Override"].KeyIsNullOrEmpty())
                                uefnoption.Icon = option["Override"].Value<string>();
                            else if (!option["Icon"].KeyIsNullOrEmpty())
                                uefnoption.Icon = option["Icon"].Value<string>();
                            else
                                uefnoption.Icon = string.Format("https://fortnite-api.com/images/cosmetics/br/{0}/smallicon.png", option["ID"].Value<string>());

                            if (!option["Message"].KeyIsNullOrEmpty())
                                uefnoption.OptionMessage = option["Message"].Value<string>();

                            foreach (var Asset in option["Assets"])
                            {
                                var NewAsset = new Asset() { Object = Asset["AssetPath"].Value<string>() };

                                if (Asset["AssetPathTo"] != null)
                                    NewAsset.OverrideObject = Asset["AssetPathTo"].Value<string>();

                                if (Asset["Buffer"] != null)
                                    NewAsset.OverrideBuffer = Asset["Buffer"].Value<string>();

                                if (Asset["Swaps"] != null)
                                    NewAsset.Swaps = Asset["Swaps"];

                                uefnoption.Exports.Add(NewAsset);
                            }

                            var fallback = new Asset() { Object = "/Game/Athena/Heroes/Meshes/Bodies/CP_Athena_Body_F_Fallback", OverrideObject = parse["AssetPathTo"].Value<string>(), Swaps = parse["Swaps"] };
                            uefnoption.Exports.Add(fallback);
                            optionlist.Add(uefnoption);
                        }
                        
                        ((MainView)App.Current.MainWindow).SetOverlay(new OptionsView(newoption.Name, optionlist));
                        return;
                }
            };

            return newcomsetic;
        }

        private void Discord_Click(object sender, MouseButtonEventArgs e) => Global.Discord.UrlStart();
    }
}