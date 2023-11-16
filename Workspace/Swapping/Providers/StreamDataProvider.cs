using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Galaxy_Swapper_v2.Workspace.Hashes;
using Galaxy_Swapper_v2.Workspace.Swapping.Other;

namespace Galaxy_Swapper_v2.Workspace.Swapping.Providers
{
    public static class StreamDataProvider
    {
        private const string Domain = "https://galaxyswapperv2.com/API/StreamData/{0}.chunk";
        public enum CompressionType
        {
            None = 0,
            Aes = 1,
            Zlib = 2,
            Oodle = 3,
            GZip = 4
        }

        public static byte[] Download(string path)
        {
            var stopwatch = new Stopwatch(); stopwatch.Start();
            string chunkName = Convert.ToHexString(MD5.Hash(path));
            string url = string.Format(Domain, chunkName);

            using (var client = new RestClient())
            {
                var request = new RestRequest(new Uri(url), Method.Get);

                Log.Information($"Sending {request.Method} request to {url}");

                var response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK || response.RawBytes is null || response.RawBytes.Length == 0)
                {
                    Log.Fatal($"Failed to download response from StreamData! Expected: {HttpStatusCode.OK} Received: {response.StatusCode}");
                    Message.DisplaySTA("Error", "Webclient caught a exception while downloading StreamData.", discord: true, solutions: new[] { "Disable Windows Defender Firewall", "Disable any anti-virus softwares", "Turn on a VPN" });
                    return null!;
                }

                var reader = new Reader(response.RawBytes);

                reader.BaseStream.Position += 4;



                Log.Information($"Finished {request.Method} request in {stopwatch.GetElaspedAndStop().ToString("mm':'ss")} received {response.Content.Length}");
                return null!;
            }
        }
    }
}