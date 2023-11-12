using DiscordRPC;
using DiscordRPC.Message;
using Galaxy_Swapper_v2.Workspace.Properties;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Linq;

namespace Galaxy_Swapper_v2.Workspace.Utilities
{
    public static class Presence
    {
        private static RichPresence RichPresence;
        public static DiscordRpcClient Client;
        public static User User;

        public static void Initialize()
        {
            var Parse = Endpoint.Read(Endpoint.Type.Presence);

            Client = new DiscordRpcClient(Parse["ApplicationID"].Value<string>());
            Client.Initialize();
            Client.OnReady += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.User.Username) && !string.IsNullOrEmpty(e.User.GetAvatarURL(User.AvatarFormat.PNG)))
                {
                    User = e.User;

                    if (new ulong[] { 1106620371307864085, 852567052899844096, 594557172784955392, 598091052817186835, 200364085508964354 }.Contains(User.ID))
                    {
                        Log.Error("User is banned!");
                        Environment.Exit(0);
                    }
                }
            };

            RichPresence = new RichPresence()
            {
                Details = "Dashboard",
                State = Parse["State"].Value<string>(),
                Timestamps = Timestamps.Now,
                Buttons = Parse["Buttons"].Select(Button => new DiscordRPC.Button { Label = Button["Label"].Value<string>(), Url = Button["URL"].Value<string>() }).ToArray(),
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
