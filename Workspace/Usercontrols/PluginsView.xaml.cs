using Galaxy_Swapper_v2.Workspace.Components;
using Galaxy_Swapper_v2.Workspace.Generation.Formats;
using Galaxy_Swapper_v2.Workspace.Properties;
using Galaxy_Swapper_v2.Workspace.Usercontrols.Overlays;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

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
                    Plugins.Add(file);
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

            foreach (string Plugin in Plugins.List())
            {
                string Content = File.ReadAllText(Plugin);
                if (Content.Length == 0)
                {
                    Log.Warning($"{Plugin} content length is 0? skipping");
                    continue;
                }

                Content = Encoding.ASCII.GetString(Compression.Decompress(Content));

                if (!Content.ValidJson())
                {
                    Log.Warning($"{Plugin} content is not in a valid json format? skipping");
                    continue;
                }

                var Parse = JObject.Parse(Content);
                var NewCosmetic = CreateCosmetic(Parse["Name"].Value<string>(), Parse["Icon"].Value<string>(), Plugin, Parse);
                Plugins_Items.Children.Add(NewCosmetic);
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

        private CPluginControl CreateCosmetic(string Name, string URL, string PluginPath, JToken Parse)
        {
            var NewCosmetic = new CPluginControl(this) { Height = 85, Width = 85, Margin = new Thickness(10), Cursor = Cursors.Hand, ToolTip = Name, PluginPath = PluginPath };

            NewCosmetic.Remove.ToolTip = Languages.Read(Languages.Type.View, "PluginsView", "Remove");

            var Icon = new BitmapImage();
            Icon.BeginInit();

            if (Parse["OverrideIcon"].KeyIsNullOrEmpty())
                Icon.UriSource = new Uri(URL, UriKind.RelativeOrAbsolute);
            else
                Icon.UriSource = new Uri(Parse["OverrideIcon"].Value<string>(), UriKind.RelativeOrAbsolute);

            Icon.CacheOption = BitmapCacheOption.OnLoad;
            Icon.EndInit();

            NewCosmetic.Icon = Icon;
            NewCosmetic.MouseEnter += Cosmetic_MouseEnter;
            NewCosmetic.MouseLeave += Cosmetic_MouseLeave;
            NewCosmetic.Cosmetic.MouseLeftButtonDown += delegate
            {
                var NewOption = new Option() { Name = Parse["Name"].Value<string>(), OverrideIcon = Name = Parse["Icon"].Value<string>(), Icon = Parse["Swapicon"].Value<string>(), Message = Parse["Message"].Value<string>() };
                foreach (var Asset in Parse["Assets"])
                {
                    if (Asset["DefaultSwap"] != null && Asset["DefaultSwap"].Value<bool>())
                    {
                        var DefaultParse = Endpoint.Read(Endpoint.Type.DefaultSwaps);
                        var Swaps = new JArray();
                        var CharacterParts = new List<string>();

                        if (!Asset["Swaps"].KeyIsNullOrEmpty())
                            Swaps = (JArray)Asset["Swaps"];

                        foreach (string CharacterPart in DefaultParse["SwappableCharacterParts"])
                            CharacterParts.Add(CharacterPart);

                        foreach (string CharacterPart in Asset["CharacterParts"])
                        {
                            Swaps.Add(JObject.FromObject(new { type = "string", search = CharacterParts.First(), replace = CharacterPart }));
                            CharacterParts.Remove(CharacterParts.First());
                        }

                        foreach (string CharacterPart in CharacterParts)
                            Swaps.Add(JObject.FromObject(new { type = "string", search = CharacterPart, replace = DefaultParse["InvalidCharacterPart"].Value<string>() }));

                        NewOption.Exports.Add(new Asset() { Object = Asset["AssetPath"].Value<string>(), OverrideObject = DefaultParse["ObjectPath"].Value<string>(), OverrideBuffer = DefaultParse["OverrideBuffer"].Value<string>(), Swaps = Swaps });

                        if (!Asset["InvalidDefaults"].KeyIsNullOrEmpty() && Asset["InvalidDefaults"].Value<bool>())
                        {
                            foreach (var Invalid in DefaultParse["Invalid"])
                                NewOption.Exports.Add(new Asset() { Object = Invalid["AssetPath"].Value<string>(), Swaps = Invalid["Swaps"] });
                        }
                    }
                    else
                    {
                        var NewAsset = new Asset() { Object = Asset["AssetPath"].Value<string>() };
                        if (!Asset["AssetPathTo"].KeyIsNullOrEmpty())
                            NewAsset.OverrideObject = Asset["AssetPathTo"].Value<string>();
                        if (Asset["Buffer"] != null)
                            NewAsset.OverrideBuffer = Asset["Buffer"].Value<string>();
                        if (Asset["Swaps"] != null)
                            NewAsset.Swaps = Asset["Swaps"];

                        NewOption.Exports.Add(NewAsset);
                    }
                }

                if (Parse["Downloadables"] != null)
                {
                    foreach (var downloadable in Parse["Downloadables"])
                    {
                        if (!downloadable["pak"].KeyIsNullOrEmpty() && !downloadable["sig"].KeyIsNullOrEmpty() && !downloadable["ucas"].KeyIsNullOrEmpty() && !downloadable["utoc"].KeyIsNullOrEmpty())
                            NewOption.Downloadables.Add(new Downloadable() { Pak = downloadable["pak"].Value<string>(), Sig = downloadable["sig"].Value<string>(), Ucas = downloadable["ucas"].Value<string>(), Utoc = downloadable["utoc"].Value<string>() });
                    }
                }

                if (!Parse["Nsfw"].KeyIsNullOrEmpty())
                    NewOption.Nsfw = Parse["Nsfw"].Value<bool>();

                Memory.MainView.SetOverlay(new SwapView(NewOption));
            };
            return NewCosmetic;
        }

        private void Cosmetic_MouseEnter(object sender, MouseEventArgs e)
        {
            ((CPluginControl)sender).Margin = new Thickness(5);
            ((CPluginControl)sender).Height += 10;
            ((CPluginControl)sender).Width += 10;
        }

        private void Cosmetic_MouseLeave(object sender, MouseEventArgs e)
        {
            ((CPluginControl)sender).Margin = new Thickness(10);
            ((CPluginControl)sender).Height -= 10;
            ((CPluginControl)sender).Width -= 10;
        }

        private void Discord_Click(object sender, MouseButtonEventArgs e) => Global.Discord.UrlStart();
    }
}