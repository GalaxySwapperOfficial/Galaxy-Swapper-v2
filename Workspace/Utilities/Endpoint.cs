using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            News,
            Cosmetics,
            Languages,
            Presence,
            FOV,
            Lobby,
            UEFN,
            Socials
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

            if (File.Exists("D:\\Galaxy Swapper v2\\Backend\\API\\1.13\\Cosmetics.json")) //Local API for debugging
                Parse["Cosmetics"] = JObject.Parse(File.ReadAllText("D:\\Galaxy Swapper v2\\Backend\\API\\1.13\\Cosmetics.json"));
            if (File.Exists("D:\\Galaxy Swapper v2\\Backend\\API\\1.15\\UEFN.json")) //Local API for debugging
                Parse["UEFN"] = JObject.Parse(File.ReadAllText("D:\\Galaxy Swapper v2\\Backend\\API\\1.15\\UEFN.json"));
            if (File.Exists("D:\\Galaxy Swapper v2\\Backend\\API\\1.17\\Lobby.json")) //Local API for debugging
                Parse["Lobby"] = JObject.Parse(File.ReadAllText("D:\\Galaxy Swapper v2\\Backend\\API\\1.17\\Lobby.json"));

            return Parse[Type.ToString()];
        }

        private static void Download()
        {
            var stopwatch = new Stopwatch(); stopwatch.Start();

            using (var client = new RestClient())
            {
                var request = new RestRequest(new Uri(Domain));
                request.AddHeader("version", Global.Version);
                request.AddHeader("apiversion", Global.ApiVersion);

                Log.Information($"Sending request to {Domain}");

                var response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Log.Fatal($"Failed to download response from endpoint! Expected: {HttpStatusCode.OK} Received: {response.StatusCode}");
                    Message.DisplaySTA("Error", "Webclient caught a exception while downloading response from Endpoint.", MessageBoxButton.OK, solutions: new List<string> { "Disable Windows Defender Firewall", "Disable any anti-virus softwares", "Turn on a VPN" }, close: true);
                }

                Parse = JsonConvert.DeserializeObject<JObject>(response.Content);

                if (Parse["status"].Value<int>() != 200)
                {
                    Log.Fatal($"Endpoint did not return with code 200! Expected: 200 Received: {Parse["status"].Value<int>()}");
                    Message.DisplaySTA("Error", Parse["message"].Value<string>(), MessageBoxButton.OK, solutions: new List<string> { "Disable Windows Defender Firewall", "Disable any anti-virus softwares", "Turn on a VPN" }, close: true);
                }

                Log.Information($"Finished GET request in {stopwatch.GetElaspedAndStop().ToString("mm':'ss")} received {response.Content.Length}");
            }
        }

        public static void Clear() => Parse = null;
    }
}