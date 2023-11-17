using Galaxy_Swapper_v2.Workspace.CProvider.Encryption;
using Galaxy_Swapper_v2.Workspace.CProvider.Objects;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;

namespace Galaxy_Swapper_v2.Workspace.Swapping.Providers
{
    public static class AesProvider
    {
        private const string Domain = "https://galaxyswapperv2.com/API/Fortnite/Aes.json";
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
                    return;
                }

                var parse = JsonConvert.DeserializeObject<JObject>(response.Content);

                if (parse["dynamicKeys"] is not null)
                {
                    foreach (var dynamickey in parse["dynamicKeys"])
                    {
                        var pakGuid = new FGuid(dynamickey["guid"].Value<string>());
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
