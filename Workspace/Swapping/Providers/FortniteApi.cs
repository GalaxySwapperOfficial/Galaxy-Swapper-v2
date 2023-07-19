using CUE4Parse.Encryption.Aes;
using CUE4Parse.UE4.Objects.Core.Misc;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;

namespace Galaxy_Swapper_v2.Workspace.Swapping.Providers
{
    /// <summary>
    /// All the code below was provided from: https://github.com/GalaxySwapperOfficial/Galaxy-Swapper-v2
    /// You can also find us at https://galaxyswapperv2.com/Guilded
    /// </summary>
    public static class FortniteApi
    {
        public static Dictionary<FGuid, FAesKey> Keys = new Dictionary<FGuid, FAesKey>();
        private const string Domain = "https://fortnite-api.com/v2/aes";
        private const string Dummy = "0x0000000000000000000000000000000000000000000000000000000000000000";
        public static void Download()
        {
            if (Keys.Count != 0)
                return;

            using (WebClient WC = new WebClient())
            {
                try
                {
                    string Response = WC.DownloadString(Domain);

                    if (string.IsNullOrEmpty(Response) || !Response.ValidJson())
                    {
                        Log.Warning($"{Domain} has returned with a unknown format. Loading mainkey with dummy key");
                        Keys.Add(new FGuid(), new FAesKey(Dummy));
                        return;
                    }

                    var Parse = JObject.Parse(Response);

                    if (Parse["data"]["mainKey"].KeyIsNullOrEmpty())
                    {
                        Log.Warning("Main key is null. Loading mainkey with dummy key");
                        Keys.Add(new FGuid(), new FAesKey(Dummy));
                    }
                    else
                    {
                        Log.Information($"Main key loading as {Parse["data"]["mainKey"].Value<string>()}");
                        Keys.Add(new FGuid(), new FAesKey(FormatAES(Parse["data"]["mainKey"].Value<string>())));
                    }

                    foreach (var dynamicKey in Parse["data"]["dynamicKeys"])
                    {
                        var pakGuid = new FGuid(dynamicKey["pakGuid"].Value<string>());
                        var key = new FAesKey(FormatAES(dynamicKey["key"].Value<string>()));

                        if (Keys.ContainsKey(pakGuid))
                            continue;

                        Log.Information($"Loading {dynamicKey["pakGuid"].Value<string>()} with key: {dynamicKey["key"].Value<string>()}");
                        Keys.Add(pakGuid, key);
                    }
                }
                catch (Exception Exception)
                {
                    Log.Warning(Exception, "Failed to download response from {Domain} loading with dummy key! {e.Message}");
                    Keys.Add(new FGuid(), new FAesKey(Dummy));
                }
            }
        }

        private static string FormatAES(string key) => key.StartsWith("0x") ? key : $"0x{key}";
    }
}
