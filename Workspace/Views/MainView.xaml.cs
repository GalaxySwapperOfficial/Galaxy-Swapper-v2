using Galaxy_Swapper_v2.Workspace.Components;
using Galaxy_Swapper_v2.Workspace.Swapping.Other;
using Galaxy_Swapper_v2.Workspace.Swapping.Providers;
using Galaxy_Swapper_v2.Workspace.Usercontrols;
using Galaxy_Swapper_v2.Workspace.Usercontrols.Overlays;
using Galaxy_Swapper_v2.Workspace.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Galaxy_Swapper_v2.Workspace.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            Memory.MainView = this;
        }

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            Main.Visibility = Visibility.Hidden;
            SetOverlay(new SplashView());
        }

        private CTabControl LastTabBorder { get; set; } = default!;
        public void Tab_Click(object sender, MouseButtonEventArgs e)
        {
            var Sender = (CTabControl)sender;

            LastTabBorder?.Tab_Default();
            LastTabBorder = Sender;
            Sender.Tab_Clicked();

            UserControl NewTab;
            switch (Sender.Name)
            {
                case "Dashboard":
                    NewTab = Memory.DashboardView;
                    SearchBar.Visibility = Visibility.Hidden;
                    break;
                case "Characters":
                    NewTab = Memory.LoadCharacters(SearchBar.Searchbar);
                    SearchBar.Visibility = Visibility.Visible;
                    break;
                case "Backpacks":
                    NewTab = Memory.LoadBackpacks(SearchBar.Searchbar);
                    SearchBar.Visibility = Visibility.Visible;
                    break;
                case "Pickaxes":
                    NewTab = Memory.LoadPickaxes(SearchBar.Searchbar);
                    SearchBar.Visibility = Visibility.Visible;
                    break;
                case "Dances":
                    NewTab = Memory.LoadDances(SearchBar.Searchbar);
                    SearchBar.Visibility = Visibility.Visible;
                    break;
                case "Gliders":
                    NewTab = Memory.LoadGliders(SearchBar.Searchbar);
                    SearchBar.Visibility = Visibility.Visible;
                    break;
                case "Weapons":
                    NewTab = Memory.LoadWeapons(SearchBar.Searchbar);
                    SearchBar.Visibility = Visibility.Visible;
                    break;
                case "Misc":
                    NewTab = Memory.MiscView;
                    SearchBar.Visibility = Visibility.Hidden;
                    break;
                case "Settings":
                    NewTab = Memory.SettingsView;
                    SearchBar.Visibility = Visibility.Hidden;
                    break;
                case "Plugins":
                    NewTab = Memory.PluginsView;
                    SearchBar.Visibility = Visibility.Hidden;
                    break;
                default:
                    return;
            }

            if (TabHolder.Child != null)
                TabHolder.Child = null;

            TabHolder.Child = NewTab;

            Interface.SetElementAnimations(new Interface.BaseAnim { Element = NewTab, Property = new PropertyPath(Control.OpacityProperty), ElementAnim = new DoubleAnimation() { From = 0, To = 1, Duration = new TimeSpan(0, 0, 0, 0, 400) } }).Begin();
        }

        private UserControl LastOverlay { get; set; } = default!;
        public void SetOverlay(UserControl Overlay)
        {
            if (LastOverlay != null)
                Base.Children.Remove(LastOverlay);

            LastOverlay = Overlay;
            Main.IsEnabled = false;

            var Storyboard = Interface.SetElementAnimations(new Interface.BaseAnim { Element = TabHolder, Property = new PropertyPath(Control.OpacityProperty), ElementAnim = new DoubleAnimation() { From = 1, To = 0, Duration = new TimeSpan(0, 0, 0, 0, 400) } });
            Storyboard.Completed += delegate
            {
                Interface.SetBlur(Main);
                Base.Children.Add(Overlay);
                Interface.SetElementAnimations(new Interface.BaseAnim { Element = Overlay, Property = new PropertyPath(Control.OpacityProperty), ElementAnim = new DoubleAnimation() { From = 0, To = 1, Duration = new TimeSpan(0, 0, 0, 0, 400) } }).Begin();
            };
            Storyboard.Begin();
        }

        public void RemoveOverlay()
        {
            Main.Effect = null;

            var Storyboard = Interface.SetElementAnimations(new Interface.BaseAnim { Element = LastOverlay, Property = new PropertyPath(Control.OpacityProperty), ElementAnim = new DoubleAnimation() { From = 1, To = 0, Duration = new TimeSpan(0, 0, 0, 0, 400) } });
            Storyboard.Completed += delegate
            {
                Main.IsEnabled = true;
                Base.Children.Remove(LastOverlay);
                LastOverlay = null!;
                Interface.SetElementAnimations(new Interface.BaseAnim { Element = TabHolder, Property = new PropertyPath(Control.OpacityProperty), ElementAnim = new DoubleAnimation() { From = 0, To = 1, Duration = new TimeSpan(0, 0, 0, 0, 400) } }).Begin();
            };
            Storyboard.Begin();
        }

        private void Close_Click(object sender, MouseButtonEventArgs e) => Memory.MainView.Close();
        private void Minimize_Click(object sender, MouseButtonEventArgs e) => Memory.MainView.WindowState = WindowState.Minimized;
        private void Drag_Click(object sender, MouseButtonEventArgs e) => Memory.MainView.DragMove();
    }
}