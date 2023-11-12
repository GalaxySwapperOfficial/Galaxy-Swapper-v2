using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Net;

namespace Galaxy_Swapper_v2.Workspace.Utilities
{
    public static class Endpoint
    {
        private static JObject Parse { get; set; } = default!;
        private const string Domain = "https://www.galaxyswapperv2.com/API/Return.php";

        public enum Type
        {
            Version, News, Cosmetics, Languages, Presence, FOV, Lobby, UEFN, Socials, Stats
        }

        public static JToken Read(Type Type)
        {
            if (Parse == null)
                Download();
            if (!Parse.ContainsKey(Type.ToString()))
                throw new Exception($"Failed to find {Type} in endpoint cache.");
            return Parse[Type.ToString()];
        }

        private static void Download()
        {
            using (var client = new RestClient())
            {
                var request = new RestRequest(new Uri(Domain), Method.Get);
                request.AddHeader("version", Global.Version);
                request.AddHeader("apiversion", Global.ApiVersion);

                var response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception($"Failed to download response from endpoint! Expected: {HttpStatusCode.OK} Received: {response.StatusCode}");

                Parse = JObject.Parse(response.Content);

                if (Parse["status"].Value<int>() != 200)
                    throw new Exception($"Endpoint did not return with code 200! Expected: 200 Received: {Parse["status"].Value<int>()}");
            }
        }

        public static void Clear() => Parse = null;
    }
}
