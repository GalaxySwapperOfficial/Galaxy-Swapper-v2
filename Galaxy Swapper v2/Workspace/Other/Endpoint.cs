using Galaxy_Swapper_v2.Workspace.Encryption;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Galaxy_Swapper_v2.Workspace.Other
{
    public static class Endpoint
    {
        public enum Endpoints
        {
            Status,
            Languages,
            Rpc,
            Skins,
            SpecialSkins,
            Backblings,
            Pickaxes,
            Emotes,
            Wraps,
            Misc,
            Plugins,
            IDSwapper
        }
        public static string APIReturn(Endpoints e, string JsonValue)
        {
            string Endpoint = "https://raw.githubusercontent.com/GalaxySwapperOfficial/Galaxy-Swapper-API/main/In%20Game/";
            switch (e)
            {
                case Endpoints.Status:
                    Endpoint += "Status.json";
                    break;
                case Endpoints.Languages:
                    Endpoint += "Languages.json";
                    break;
                case Endpoints.Rpc:
                    Endpoint += "RPC.json";
                    break;
                case Endpoints.Plugins:
                    Endpoint += "Plugins.json";
                    break;
                case Endpoints.IDSwapper:
                    Endpoint += "ID-Swapper.json";
                    break;
                case Endpoints.Skins:
                    Endpoint += "Cosmetics/Skins.json";
                    if (File.Exists(@"C:\Users\white\Music\Galaxy Swapper v2\API\Skins.json"))
                    {
                        return File.ReadAllText(@"C:\Users\white\Music\Galaxy Swapper v2\API\Skins.json");
                    }
                    break;
                case Endpoints.SpecialSkins:
                    Endpoint += "Cosmetics/SpecialSkins.json";
                    if (File.Exists(@"C:\Users\white\Music\Galaxy Swapper v2\API\SpecialSkins.json"))
                    {
                        return File.ReadAllText(@"C:\Users\white\Music\Galaxy Swapper v2\API\SpecialSkins.json");
                    }
                    break;
                case Endpoints.Backblings:
                    Endpoint += "Cosmetics/Backblings.json";
                    if (File.Exists(@"C:\Users\white\Music\Galaxy Swapper v2\API\Backblings.json"))
                    {
                        return File.ReadAllText(@"C:\Users\white\Music\Galaxy Swapper v2\API\Backblings.json");
                    }
                    break;
                case Endpoints.Pickaxes:
                    Endpoint += "Cosmetics/Pickaxes.json";
                    if (File.Exists(@"C:\Users\white\Music\Galaxy Swapper v2\API\Pickaxes.json"))
                    {
                        return File.ReadAllText(@"C:\Users\white\Music\Galaxy Swapper v2\API\Pickaxes.json");
                    }
                    break;
                case Endpoints.Emotes:
                    Endpoint += "Cosmetics/Emotes.json";
                    if (File.Exists(@"C:\Users\white\Music\Galaxy Swapper v2\API\Emotes.json"))
                    {
                        return File.ReadAllText(@"C:\Users\white\Music\Galaxy Swapper v2\API\Emotes.json");
                    }
                    break;
                case Endpoints.Wraps:
                    Endpoint += "Cosmetics/Wraps.json";
                    break;
                case Endpoints.Misc:
                    Endpoint += "Cosmetics/Misc.json";
                    break;
            }
            var client = new RestClient(Endpoint);
            var request = new RestRequest(Method.GET);

            string ReturnContent = client.Execute(request).Content;
            //If Its Encrypted Then We Will Decrypt it! Encryption is not provided here.
            if (Regex.IsMatch(ReturnContent, @"^[a-zA-Z0-9\+/]*={0,2}$"))
            {
                ReturnContent = Compression.Decompress(ReturnContent);
            }
            if (JsonValue == null)
                return ReturnContent;
            else
                return JObject.Parse(ReturnContent)[JsonValue].ToString();
        }
    }
}
