using Galaxy_Swapper_v2.Workspace.Components;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;

namespace Galaxy_Swapper_v2.Workspace.Properties
{
    /// <summary>
    /// All the code below was provided from: https://github.com/GalaxySwapperOfficial/Galaxy-Swapper-v2
    /// You can also find us at https://galaxyswapperv2.com/Guilded
    /// </summary>
    public static class Account
    {
        public static readonly string Path = $"{Config.Path}\\LoginGithub.json";
        public static bool Valid()
        {
            try
            {
                if (!File.Exists(Path))
                    return false;

                string Content = File.ReadAllText(Path);

                if (!Content.ValidJson())
                    return false;

                var Parse = JObject.Parse(Content);

                if (Parse["Username"].Value<string>() != Environment.UserName)
                    return false;

                DateTime CurrentDate = DateTime.Now;

                foreach (string Day in Parse["Days"])
                {
                    if (Day == CurrentDate.ToString("dd/MM/yyyy"))
                        return true;
                }

                Log.Warning("Key expired");
                File.Delete(Path);
                return false;
            }
            catch (Exception Exception)
            {
                Log.Error(Exception, "Error while validating key");
                return false;
            }
        }

        private const string Domain = "https://galaxyswapperv2.com/Key/Valid.php";
        public static bool Activate(string Activation)
        {
            using (WebClient WC = new WebClient())
            {
                WC.Headers.Add("version", Global.Version);
                WC.Headers.Add("apiversion", Global.ApiVersion);
                WC.Headers.Add("activation", Activation);
                WC.Headers.Add("auth", "galaxyswapperv2");

                try
                {
                    string Response = WC.DownloadString(Domain);

                    if (!Response.ValidJson())
                    {
                        new CMessageboxControl("Error", $"Endpoint response did not return in a valid JSON format. Contact Wslt about this issue.", System.Windows.MessageBoxButton.OK, new List<string>() { Global.Discord }).ShowDialog();
                        return false;
                    }

                    var Parse = JObject.Parse(Response);

                    if (Parse["status"].Value<int>() == 200)
                    {
                        Message.Display(Languages.Read(Languages.Type.Header, "Info"), string.Format(Languages.Read(Languages.Type.Message, "LoginSuccess"), Parse["days"].Value<int>()), MessageBoxButton.OK);

                        if (!Create(Parse["days"].Value<int>()))
                            return false;

                        return true;
                    }
                    else if (Parse["status"].Value<int>() == 409)
                    {
                        if (Message.Display(Languages.Read(Languages.Type.Header, "Warning"), Languages.Read(Languages.Type.Message, "LoginInvalid"), MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
                            Global.Key.UrlStart();

                        return false;
                    }
                    else
                    {
                        new CMessageboxControl("Error", Parse["message"].Value<string>(), System.Windows.MessageBoxButton.OK, new List<string>() { Global.Discord }, new List<string>() { "Disable Windows Defender Firewall", "Disable any anti-virus softwares", "Turn on a VPN" }).ShowDialog();
                        return false;
                    }
                }
                catch
                {
                    new CMessageboxControl("Error", "Webclient caught a exception while downloading response from Endpoint.", System.Windows.MessageBoxButton.OK, new List<string>() { Global.Discord }, new List<string>() { "Disable Windows Defender Firewall", "Disable any anti-virus softwares", "Turn on a VPN" }).ShowDialog();
                    return false;
                }
            }
        }

        private static bool Create(int Days)
        {
            try
            {
                if (File.Exists(Path))
                    File.Delete(Path);

                DateTime CurrentDate = DateTime.Now;

                var Array = new JArray
                {
                    CurrentDate.ToString("dd/MM/yyyy")
                };

                for (int i = 1; i < Days; i++)
                {
                    Array.Add(CurrentDate.AddDays(i).ToString("dd/MM/yyyy"));
                }

                var Object = JObject.FromObject(new
                {
                    Username = Environment.UserName,
                    Days = Array
                });

                string Content = Object.ToString(Newtonsoft.Json.Formatting.None);

                File.WriteAllText(Path, Content);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}