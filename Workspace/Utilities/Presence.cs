using DiscordRPC;
using DiscordRPC.Message;
using Galaxy_Swapper_v2.Workspace.Properties;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Galaxy_Swapper_v2.Workspace.Utilities
{
    public static class Presence
    {
        private static RichPresence RichPresence;
        public static DiscordRpcClient Client;

        public static string Username = "Anonymous";
        public static string Avatar = "https://www.galaxyswapperv2.com/Icons/InvalidIcon.png";
        public static ulong ID = 0;
        public static int Tag = 0000;
        public static User.Flag Flag = User.Flag.None;
        public static User.PremiumType PremiumType = User.PremiumType.None;

        public static void Initialize()
        {
            var Parse = Endpoint.Read(Endpoint.Type.Presence);

            Client = new DiscordRpcClient(Parse["ApplicationID"].Value<string>());
            Client.Initialize();
            Client.OnReady += delegate (object sender, ReadyMessage e)
            {
                if (!string.IsNullOrEmpty(e.User.Username) && !string.IsNullOrEmpty(e.User.GetAvatarURL(User.AvatarFormat.PNG)))
                {
                    Username = e.User.Username;
                    Tag = e.User.Discriminator;
                    Avatar = e.User.GetAvatarURL(User.AvatarFormat.PNG);
                    ID = e.User.ID;
                    Flag = e.User.Flags;
                    PremiumType = e.User.Premium;
                }
            };

            List<Button> Buttons = new List<Button>();

            foreach (var Button in Parse["Buttons"])
            {
                Buttons.Add(new DiscordRPC.Button { Label = Button["Label"].Value<string>(), Url = Button["URL"].Value<string>() });
            }

            RichPresence = new RichPresence()
            {
                Details = "Dashboard",
                State = Parse["State"].Value<string>(),
                Timestamps = Timestamps.Now,
                Buttons = Buttons.ToArray(),
                Assets = new Assets()
                {
                    LargeImageKey = Parse["LargeImageKey"].Value<string>(),
                    LargeImageText = Parse["LargeImageText"].Value<string>(),
                    SmallImageKey = Parse["SmallImageKey"].Value<string>(),
                    SmallImageText = Parse["SmallImageText"].Value<string>()
                }
            };

            if (Settings.Read(Settings.Type.RichPresence).Value<bool>())
                Client.SetPresence(RichPresence);
        }

        public static void Update(string Details)
        {
            if (Settings.Read(Settings.Type.RichPresence).Value<bool>())
                Client.UpdateDetails(Details);
        }
    }
}