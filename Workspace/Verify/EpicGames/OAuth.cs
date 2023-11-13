using Galaxy_Swapper_v2.Workspace.Usercontrols.Overlays;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Diagnostics;
using System.Net;

namespace Galaxy_Swapper_v2.Workspace.Verify.EpicGames
{
    public class OAuth
    {
        private const string Domain = "https://account-public-service-prod03.ol.epicgames.com/account/api/oauth/token";
        private const string Authorization = "basic MzQ0NmNkNzI2OTRjNGE0NDg1ZDgxYjc3YWRiYjIxNDE6OTIwOWQ0YTVlMjVhNDU3ZmI5YjA3NDg5ZDMxM2I0MWE=";
        public string Access_Token = null!;
        public bool Download(VerifyView verifyView)
        {
            var stopwatch = new Stopwatch(); stopwatch.Start();

            using (var client = new RestClient())
            {
                var request = new RestRequest(new Uri(Domain), Method.Post);
                request.AddHeader("Authorization", Authorization);
                request.AddParameter("grant_type", "client_credentials");

                verifyView.Output($"Sending {request.Method} request to {Domain}", VerifyView.Type.Info);

                var response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }

                Access_Token = JsonConvert.DeserializeObject<JObject>(response.Content)["access_token"].Value<string>(); ;
                verifyView.Output($"Finished {request.Method} request in {stopwatch.GetElaspedAndStop().ToString("mm':'ss")} received {response.Content.Length}", VerifyView.Type.Info);

                return true;
            }
        }
    }
}