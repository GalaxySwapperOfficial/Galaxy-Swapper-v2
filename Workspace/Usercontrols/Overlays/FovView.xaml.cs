using Galaxy_Swapper_v2.Workspace.Properties;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Galaxy_Swapper_v2.Workspace.Usercontrols.Overlays
{
    public partial class FovView : UserControl
    {
        public FovView()
        {
            InitializeComponent();
        }

        private void FovView_Loaded(object sender, RoutedEventArgs e)
        {
            Convert.Content = Languages.Read(Languages.Type.View, "SwapView", "Convert");
            Revert.Content = Languages.Read(Languages.Type.View, "SwapView", "Revert");

            if (Settings.Read(Settings.Type.CloseFortnite).Value<bool>())
                EpicGamesLauncher.Close();

            var Parse = Endpoint.Read(Endpoint.Type.FOV);

            if (Parse["Message"] != null && !string.IsNullOrEmpty(Parse["Message"].Value<string>()) && Parse["Message"].Value<string>().ToLower() != "false")
                Message.Display(Languages.Read(Languages.Type.Header, "Warning"), Parse["Message"].Value<string>(), MessageBoxButton.OK);

            Slider.Maximum = Parse["Maximum"].Value<int>();
            Slider.Minimum = Parse["Minimum"].Value<int>();

            foreach (var Preview in Parse["Previews"])
            {
                var preview = new Image() { Name = $"Preview{Preview["Amount"].Value<string>()}", Visibility = Visibility.Hidden, Stretch = System.Windows.Media.Stretch.Fill };
                preview.LoadImage(Preview["URL"].Value<string>());
                Previews.Children.Add(preview);

                Log.Information($"Loaded icon for: {Preview["Amount"].Value<string>()}");
            }

            Slider.Value = 80;
            Presence.Update("FOV Changer");
        }

        private void Close_Click(object sender, MouseButtonEventArgs e) => Memory.MainView.RemoveOverlay();
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Slider != null && Amount != null)
            {
                Slider.Value = Math.Round(e.NewValue / Slider.TickFrequency) * Slider.TickFrequency;
                Amount.Text = string.Format(Languages.Read(Languages.Type.View, "FovView", "Amount"), Slider.Value);

                Previews.Children.OfType<Image>().ToList().ForEach(img => img.Visibility = Visibility.Hidden);
                var image = Previews.Children.OfType<Image>().FirstOrDefault(img => img.Name == $"Preview{Slider.Value}");
                if (image != null)
                    image.Visibility = Visibility.Visible;
            }
        }

        private float GetSliderValue()
        {
            double SliderValue = 120; //Set as 120 so if there are errors it's still stretched
            this.Dispatcher.Invoke(() =>
            {
                SliderValue = Slider.Value;
            });
            return (float)SliderValue;
        }

        private void Convert_Click(object sender, RoutedEventArgs e) => Memory.MainView.SetOverlay(new AuthenticateView(GetSliderValue(), true));

        private void Revert_Click(object sender, RoutedEventArgs e) => Memory.MainView.SetOverlay(new AuthenticateView(80, false));
    }
}