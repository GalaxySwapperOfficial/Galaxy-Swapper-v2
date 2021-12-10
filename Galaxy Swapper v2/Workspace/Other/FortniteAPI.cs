using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy_Swapper_v2.Workspace.Other
{
    public static class FortniteAPI
    {
        public enum Endpoints
        {
            News,
            Aes
        }
        public static string APIReturn(Endpoints e, string JsonValue)
        {
            string Endpoint = "https://fortnite-api.com/v2/";
            switch (e)
            {
                case Endpoints.News:
                    Endpoint += "news";
                    break;
                case Endpoints.Aes:
                    Endpoint += "aes";
                    break;
            }
            var client = new RestClient(Endpoint);
            var request = new RestRequest(Method.GET);

            string ReturnContent = client.Execute(request).Content;
            if (JsonValue == null)
                return ReturnContent;
            else
                return JObject.Parse(ReturnContent)[JsonValue].ToString();
        }
    }
}
