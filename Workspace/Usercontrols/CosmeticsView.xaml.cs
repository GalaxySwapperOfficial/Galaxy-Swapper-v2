using Galaxy_Swapper_v2.Workspace.Generation;
using Galaxy_Swapper_v2.Workspace.Usercontrols.Overlays;
using Galaxy_Swapper_v2.Workspace.Utilities;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Galaxy_Swapper_v2.Workspace.Usercontrols
{
    /// <summary>
    /// Interaction logic for CosmeticsView.xaml
    /// </summary>
    public partial class CosmeticsView : UserControl
    {
        private Generate.Type Type;
        public CosmeticsView(Generate.Type type)
        {
            InitializeComponent();
            Type = type;
        }

        private void CosmeticsView_Loaded(object sender, RoutedEventArgs e)
        {
            if (Frontend_Viewer.Visibility == Visibility.Hidden)
            {
                Frontend_Viewer.Visibility = Visibility.Visible;
                Option_Viewer.Visibility = Visibility.Hidden;
            }

            if (Frontend_Items.Children.Count > 0)
                return;

            var Cosmetics = Generate.Frontend(Type);
            foreach (var Cosmetic in Cosmetics)
            {
                var NewCosmetic = CreateCosmetic(Cosmetic.Key, Cosmetic.Value.Name, Cosmetic.Value.Icon, Cosmetic.Value.OverrideFrontend);
                Frontend_Items.Children.Add(NewCosmetic);
            }
        }

        public void SetSearchBar(TextBox SearchBar)
        {
            SearchBar.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    Frontend_Viewer.Visibility = Visibility.Hidden;
                    Option_Viewer.Visibility = Visibility.Visible;

                    Option_Items.Children.Clear();

                    string searchTerm = SearchBar.Text.ToLower();
                    var filteredCosmetics = Generate.Frontend(Type).Where(c => c.Value.Name.ToLower().Contains(searchTerm)).Select(c => CreateCosmetic(c.Key, c.Value.Name, c.Value.Icon, c.Value.OverrideFrontend));

                    foreach (var cosmetic in filteredCosmetics)
                    {
                        Option_Items.Children.Add(cosmetic);
                    }
                }
            };
            SearchBar.TextChanged += (sender, e) =>
            {
                if (string.IsNullOrEmpty(SearchBar.Text))
                {
                    Frontend_Viewer.Visibility = Visibility.Visible;
                    Option_Viewer.Visibility = Visibility.Hidden;
                    Option_Items.Children.Clear();
                }
            };
        }

        private Image CreateCosmetic(string CacheKey, string Name, string URL, string Frontend)
        {
            var NewCosmetic = new Image()
            {
                Height = 85,
                Width = 85,
                Margin = new Thickness(10),
                Cursor = Cursors.Hand,
                ToolTip = Name
            };

            if (string.IsNullOrEmpty(Frontend))
                NewCosmetic.LoadImage(URL, invalid: "https://github.com/GalaxySwapperOfficial/Galaxy-Swapper-API/blob/main/In%20Game/Icons/InvalidCosmetic.png?raw=true");
            else
                NewCosmetic.LoadImage(Frontend, invalid: "https://github.com/GalaxySwapperOfficial/Galaxy-Swapper-API/blob/main/In%20Game/Icons/InvalidCosmetic.png?raw=true");

            NewCosmetic.MouseEnter += (sender, e) => AnimateSize((Image)sender, 95, 5);
            NewCosmetic.MouseLeave += (sender, e) => AnimateSize((Image)sender, 85, 10);
            NewCosmetic.MouseLeftButtonDown += (sender, e) =>
            {
                var Options = Generate.Options(CacheKey, Type);
                if (Options.Count == 0)
                    Memory.MainView.SetOverlay(Memory.NoOptionsView);
                else
                    Memory.MainView.SetOverlay(new OptionsView(Name, Options));
            };

            return NewCosmetic;
        }

        private void AnimateSize(Image image, int size, int margin)
        {
            var sizeAnimation = new DoubleAnimation(size, TimeSpan.FromMilliseconds(200))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            var marginAnimation = new ThicknessAnimation(new Thickness(margin), TimeSpan.FromMilliseconds(200))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            image.BeginAnimation(WidthProperty, sizeAnimation);
            image.BeginAnimation(HeightProperty, sizeAnimation);
            image.BeginAnimation(MarginProperty, marginAnimation);
        }
    }
}