using Galaxy_Swapper_v2.Workspace.ClientSettings;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Galaxy_Swapper_v2.Workspace.Usercontrols.Overlays
{
    public partial class AuthenticateView : UserControl
    {
        public ClientSettingsData ClientSettings = null!;
        public float FovAmount = 0;
        public bool IsConvert = false;
        public AuthenticateView(float fovAmount, bool isConvert)
        {
            InitializeComponent();
            FovAmount = fovAmount;
            IsConvert = isConvert;
        }

        private void AuthenticateView_Loaded(object sender, RoutedEventArgs e) => Presence.Update("FOV Changer");

        private void AuthorizationCode_Click(object sender, MouseButtonEventArgs e) => Endpoint.Read(Endpoint.Type.FOV)["AuthorizationCode"].Value<string>().UrlStart();
        private void LoginTutorial_Click(object sender, MouseButtonEventArgs e) => Endpoint.Read(Endpoint.Type.FOV)["LoginTutorial"].Value<string>().UrlStart();
        private void Github_Click(object sender, MouseButtonEventArgs e) => Endpoint.Read(Endpoint.Type.FOV)["Github"].Value<string>().UrlStart();
        private void Discord_Click(object sender, MouseButtonEventArgs e) => Global.Discord.UrlStart();
        private void Close_Click(object sender, MouseButtonEventArgs e) => Memory.MainView.RemoveOverlay();

        private void Password_Focus(object sender, RoutedEventArgs e)
        {
            if (AuthorizationCodeBox.Password == "00000000000000000000000000000000")
            {
                AuthorizationCodeBox.Password = string.Empty;
            }
        }

        private void Password_UnFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(AuthorizationCodeBox.Password))
            {
                AuthorizationCodeBox.Password = "00000000000000000000000000000000";
            }
        }

        private void Login_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            string authorizationCode = AuthorizationCodeBox.Password;

            if (authorizationCode.ValidJson())
            {
                var parse = JObject.Parse(authorizationCode);
                if (parse["authorizationCode"] is null)
                {
                    Message.DisplaySTA("Error", "The authorization code you entered appears to be invalid or empty! Please watch the tutorial under 'How do I get my authorization code?'.", discord: true);
                    return;
                }
                else authorizationCode = parse["authorizationCode"].Value<string>();
            }

            if (string.IsNullOrEmpty(authorizationCode) || authorizationCode == "00000000000000000000000000000000")
            {
                Message.DisplaySTA("Error", "The authorization code you entered appears to be invalid or empty!\nPlease watch the tutorial and try again.", discord: true, links: new[] { Endpoint.Read(Endpoint.Type.FOV)["LoginTutorial"].Value<string>() } );
                return;
            }

            var clientSettingsData = new ClientSettingsData();
            
            if (!clientSettingsData.Authenticate(authorizationCode) || !clientSettingsData.Download())
                return;

            if (!clientSettingsData.Deserialize())
                return;

            if (!clientSettingsData.ModifyFov(FovAmount))
                return;

            if (!clientSettingsData.Upload(clientSettingsData.Buffer))
                return;

            var timeSpan = stopWatch.GetElaspedAndStop();

            if (IsConvert)
            {
                if (timeSpan.Minutes > 0)
                    Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"), string.Format(Languages.Read(Languages.Type.View, "SwapView", "ConvertedMinutes"), timeSpan.Minutes), MessageBoxButton.OK);
                else
                    Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"), string.Format(Languages.Read(Languages.Type.View, "SwapView", "Converted"), timeSpan.Seconds), MessageBoxButton.OK);
            }
            else
            {
                if (timeSpan.Minutes > 0)
                    Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"), string.Format(Languages.Read(Languages.Type.View, "SwapView", "RevertedMinutes"), timeSpan.Minutes), MessageBoxButton.OK);
                else
                    Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"), string.Format(Languages.Read(Languages.Type.View, "SwapView", "Reverted"), timeSpan.Seconds), MessageBoxButton.OK);
            }

            Close_Click(null!, null!);
        }
    }
}