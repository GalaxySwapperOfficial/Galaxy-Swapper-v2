using CUE4Parse.Encryption.Aes;
using CUE4Parse.UE4.Objects.Core.Misc;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Galaxy_Swapper_v2.Workspace.Swapping.Providers
{
    public static class AesProvider
    {
        private const string Domain = "https://fortnite-api.com/v2/aes";
        private const string Dummy = "0x0000000000000000000000000000000000000000000000000000000000000000";
        public static Dictionary<FGuid, FAesKey> Keys = new Dictionary<FGuid, FAesKey>();
        public static void Initialize()
        {
            if (Keys.Count != 0)
            {
                Log.Warning("Keys were already initialized");
                return;
            }

            using (RestClient client = new())
            {
                var request = new RestRequest(new Uri(Domain), Method.Get);

                Log.Information($"Sending {request.Method} request to {Domain}");

                var response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Log.Error($"Failed to request aes keys from: {Domain} Expected: {HttpStatusCode.OK} Received: {response.StatusCode}");
                    Keys.Add(default, new(Dummy));
                    return;
                }
                if (string.IsNullOrEmpty(response.Content))
                {
                    Log.Error($"Response from: {Domain} responded with empty content");
                    Keys.Add(default, new(Dummy));
                    return;
                }

                var parse = JsonConvert.DeserializeObject<JObject>(response.Content);

                Keys.Add(default, parse["data"]["mainKey"].KeyIsNullOrEmpty() ? new(Dummy) : new(Format(parse["data"]["mainKey"].Value<string>())));
                Log.Information($"Loaded mainkey as: {Keys.First().Value.KeyString}");

                if (parse["data"]["dynamicKeys"] is not null)
                {
                    foreach (var dynamickey in parse["data"]["dynamicKeys"])
                    {
                        var pakGuid = new FGuid(dynamickey["pakGuid"].Value<string>());
                        if (!dynamickey["key"].KeyIsNullOrEmpty() && !Keys.ContainsKey(pakGuid))
                        {
                            string key = Format(dynamickey["key"].Value<string>());
                            Keys.Add(pakGuid, new(key));

                            Log.Information($"Added {pakGuid} to keys array with value {key} as dynamic key");
                        }
                    }
                }
                else Log.Warning("dynamicKeys array is null! No dynamic keys will be loaded");
            }
        }

        private static string Format(string key) => !key.StartsWith("0x") ? "0x" + key : key;
    }
}
