using DiscordRPC;
using DiscordRPC.Message;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy_Swapper_v2.Workspace.Other
{
    public static class RPC
    {
        public static RichPresence Richprec;
        public static DiscordRpcClient Client { get; private set; }
        public static void StartRPC()
        {
            JObject parse = JObject.Parse(Endpoint.APIReturn(Endpoint.Endpoints.Rpc, null));
            Client = new DiscordRpcClient(parse["RPCKey"].ToString());
            Client.Initialize();
            Client.OnReady += delegate (object sender, ReadyMessage e)
            {
                Global.Username = e.User.Username;
                Global.ProfilePicture = e.User.GetAvatarURL(User.AvatarFormat.PNG);
            };
            Richprec = new RichPresence()
            {
                Details = "Dashboard",
                State = parse["Details"].ToString(),
                Timestamps = Timestamps.Now,

                Assets = new Assets()
                {
                    LargeImageKey = parse["LargeImageKey"].ToString(),
                    LargeImageText = parse["LargeImageText"].ToString(),
                    SmallImageKey = parse["SmallImageKey"].ToString(),
                    SmallImageText = parse["SmallImageText"].ToString()
                }
            };
            if (bool.Parse(SettingsController.ConfigReturn("DiscordRPC")) == false)
                return;
            else
                Client.SetPresence(Richprec);
        }
        public static void Update(string Details)
        {
            if (bool.Parse(SettingsController.ConfigReturn("DiscordRPC")) == false)
            {
                return;
            }
            Richprec.Details = Details;
            Client.SetPresence(Richprec);
        }
    }
}
