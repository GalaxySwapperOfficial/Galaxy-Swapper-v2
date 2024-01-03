using Galaxy_Swapper_v2.Workspace.Compression;
using Galaxy_Swapper_v2.Workspace.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

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
            Socials,
            Stats
        }

        public static JToken Read(Type type)
        {
            if (Parse == null)
                Download();
            if (!Parse.ContainsKey(type.ToString().ToLower()))
            {
                Log.Fatal($"Failed to find {type} in endpoint cache.");
                Message.DisplaySTA("Error", $"Failed to find {type} in endpoint cache.", exit: true, solutions: new[] { "Disable Windows Defender Firewall", "Disable any anti-virus softwares", "Turn on a VPN" });
            }

            if (Settings.Read(Settings.Type.IsDev).Value<bool>())
            {
                //Parse["version"] = JObject.Parse(File.ReadAllText("D:\\Galaxy Swapper v2\\Backend\\API\\Version.json"));
                Parse["uefn"] = JObject.Parse(File.ReadAllText("D:\\Galaxy Swapper v2\\Backend\\API\\1.07\\UEFN.json"));
                Parse["cosmetics"] = JObject.Parse(File.ReadAllText("D:\\Galaxy Swapper v2\\Backend\\API\\1.13\\Cosmetics.json"));
            }

            return Parse[type.ToString().ToLower()];
        }

        private static void Download()
        {
            var stopwatch = new Stopwatch(); stopwatch.Start();

            using (var client = new RestClient())
            {
                var request = new RestRequest(new Uri(Domain), Method.Get);
                request.AddHeader("version", Global.Version);
                request.AddHeader("apiversion", Global.ApiVersion);

                Log.Information($"Sending {request.Method} request to {Domain}");

                var response = client.Execute(request);

                try
                {
                    if (response.StatusCode != HttpStatusCode.OK) new Exception($"response did not return as status code OK");


                    Parse = JsonConvert.DeserializeObject<JObject>(response.Content);

                    if (Parse["iscompressed"] is not null && Parse["iscompressed"].Value<bool>())
                    {
                        Log.Information("Decompressing response");

                        try
                        {
                            byte[] decompressedBuffer = gzip.Decompress(Parse["compressedbuffer"].Value<string>());
                            Parse = JObject.Parse(Encoding.ASCII.GetString(decompressedBuffer));
                        }
                        catch (Exception Exception)
                        {
                            Log.Fatal(Exception, "Failed to decompress response from endpoint");
                            Message.DisplaySTA("Error", "Failed to decompress response from endpoint!", discord: true, solutions: new[] { "Disable Windows Defender Firewall", "Disable any anti-virus softwares", "Turn on a VPN" }, exit: true);
                        }
                    }

                    Log.Information($"Finished {request.Method} request in {stopwatch.GetElaspedAndStop().ToString("mm':'ss")} received {response.Content.Length}");
                }
                catch (Exception Exception)
                {
                    Log.Fatal(Exception, $"Failed to download response from endpoint! Expected: {HttpStatusCode.OK} Received: {response.StatusCode}");
                    Message.DisplaySTA("Error", "Webclient caught a exception while downloading response from Endpoint.", discord: true, solutions: new[] { "Disable Windows Defender Firewall", "Disable any anti-virus softwares", "Turn on a VPN" }, exit: true);
                }
            }
        }

        public static void Clear() => Parse = null;
    }
}