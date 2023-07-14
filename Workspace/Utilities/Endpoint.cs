using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;

namespace Galaxy_Swapper_v2.Workspace.Utilities
{
    public static class Endpoint
    {
        private static JObject Parse { get; set; } = default!;
        private const string Domain = "https://www.galaxyswapperv2.com/API/Return.php";
        public enum Type
        {
            Version,
            DefaultSwaps,
            News,
            Cosmetics,
            Languages,
            Presence,
            FOV
        }

        public static JToken Read(Type Type)
        {
            if (Parse == null)
                Download();
            if (!Parse.ContainsKey(Type.ToString()))
            {
                Log.Fatal($"Failed to find {Type} in endpoint cache.");
                Message.DisplaySTA("Error", $"Failed to find {Type} in endpoint cache.", MessageBoxButton.OK, solutions: new List<string> { "Disable Windows Defender Firewall", "Disable any anti-virus softwares", "Turn on a VPN" }, close: true);
            }

            if (File.Exists("E:\\Documents\\API\\Galaxy Swapper v2\\1.01\\Cosmetics.json")) //Local API for debugging
                Parse["Cosmetics"] = JObject.Parse(File.ReadAllText("E:\\Documents\\API\\Galaxy Swapper v2\\1.01\\Cosmetics.json"));

            return Parse[Type.ToString()];
        }

        private static void Download()
        {
            using (WebClient WC = new WebClient())
            {
                WC.Headers.Add("version", Global.Version);
                WC.Headers.Add("apiversion", Global.ApiVersion);

                try
                {
                    string Response = WC.DownloadString(Domain);

                    if (!Response.ValidJson())
                    {
                        Log.Fatal("Endpoint from webclient did not return in a valid json format");
                        Message.DisplaySTA("Error", "Endpoint response did not return in a valid JSON format. Contact Wslt about this issue.", MessageBoxButton.OK, solutions: new List<string> { "Disable Windows Defender Firewall", "Disable any anti-virus softwares", "Turn on a VPN" }, close: true);
                    }

                    Parse = JObject.Parse(Response);

                    if (Parse["status"].Value<int>() != 200)
                    {
                        Log.Fatal($"Endpoint did not return with code 200?\n{Parse["message"].Value<string>()}");
                        Message.DisplaySTA("Error", Parse["message"].Value<string>(), MessageBoxButton.OK, solutions: new List<string> { "Disable Windows Defender Firewall", "Disable any anti-virus softwares", "Turn on a VPN" }, close: true);
                    }

                }
                catch (Exception Exception)
                {
                    Log.Fatal(Exception, "Failed to download endpoint from webclient");
                    Message.DisplaySTA("Error", "Webclient caught a exception while downloading response from Endpoint.", MessageBoxButton.OK, solutions: new List<string> { "Disable Windows Defender Firewall", "Disable any anti-virus softwares", "Turn on a VPN" }, close: true);
                }
            }
        }

        public static void Clear() => Parse = null;
    }
}