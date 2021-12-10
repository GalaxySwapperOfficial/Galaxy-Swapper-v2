using CUE4Parse.FileProvider;
using Galaxy_Swapper_v2.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy_Swapper_v2.Workspace
{
    public static class Global
    {
        public static long Offset;
        public static string Pak;
        public static int CompressedLength;
        public static byte[] CompressedBytes;
        public static bool Compressed = false;
        public static string Username = "Username: Unkown";
        public static string ProfilePicture = "https://www.galaxyswapperv2.com/Icons/InvalidIcon.png";

        public static string Appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static void UrlStart(this string url)
        {
            ProcessStartInfo Procc = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C start {url}",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            };
            Process.Start(Procc);
        }
        public static void CloseFortnite()
        {
            var client = new RestClient("https://raw.githubusercontent.com/GalaxySwapperOfficial/Galaxy-Swapper-API/main/In%20Game/Processkills.json");
            var request = new RestRequest(Method.GET);
            foreach (var process in JArray.Parse(client.Execute(request).Content))
            {
                Process[] pname = Process.GetProcessesByName(process["Process"].ToString());
                if (pname.Length == 0)
                    continue;
                else
                {
                    pname[0].Kill();
                    pname[0].WaitForExit();
                }
            }
        }
        public static bool ValidateJSON(this string s)
        {
            try
            {
                JToken.Parse(s);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
