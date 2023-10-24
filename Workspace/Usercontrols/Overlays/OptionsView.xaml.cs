using Galaxy_Swapper_v2.Workspace.Generation.Formats;
using Galaxy_Swapper_v2.Workspace.Utilities;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Galaxy_Swapper_v2.Workspace.Usercontrols.Overlays
{
    /// <summary>
    /// Interaction logic for OptionsView.xaml
    /// </summary>
    public partial class OptionsView : UserControl
    {
        private List<Option> Options;
        private string DisplayName = "TBD";
        public OptionsView(string displayname, List<Option> options)
        {
            InitializeComponent();
            Options = options;
            DisplayName = displayname;
        }

        private void Close_Click(object sender, MouseButtonEventArgs e) => Memory.MainView.RemoveOverlay();
        private void OptionsView_Loaded(object sender, RoutedEventArgs e)
        {
            Header.Text = Languages.Read(Languages.Type.View, "OptionsView", "Header");
            Tip.Text = Languages.Read(Languages.Type.View, "OptionsView", "Tip");

            foreach (var Option in Options)
            {
                var NewOption = CreateCosmetic(Option);
                Options_Items.Children.Add(NewOption);
            }

            Presence.Update($"{DisplayName} (Options)");
        }

        private Image CreateCosmetic(Option Option)
        {
            var NewCosmetic = new Image() { Height = 85, Width = 85, Margin = new Thickness(10), Cursor = Cursors.Hand, ToolTip = Option.Name };
            NewCosmetic.LoadImage(Option.Icon);
            NewCosmetic.MouseEnter += Cosmetic_MouseEnter;
            NewCosmetic.MouseLeave += Cosmetic_MouseLeave;
            NewCosmetic.MouseLeftButtonDown += delegate
            {
                Memory.MainView.SetOverlay(new SwapView(Option));
            };

            return NewCosmetic;
        }

        private void Cosmetic_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Image)sender).Margin = new Thickness(5);
            ((Image)sender).Height += 10;
            ((Image)sender).Width += 10;
        }

        private void Cosmetic_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Image)sender).Margin = new Thickness(10);
            ((Image)sender).Height -= 10;
            ((Image)sender).Width -= 10;
        }
    }
}