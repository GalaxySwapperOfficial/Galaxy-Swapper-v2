using Galaxy_Swapper_v2.Workspace.Components;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;

namespace Galaxy_Swapper_v2.Workspace.Properties
{
    public static class Account
    {
        public static readonly string Path = $"{App.Config}\\Login.encrypted";
        public static bool Valid()
        {
            try
            {
                if (!File.Exists(Path))
                    return false;

                string Content = File.ReadAllText(Path);

                if (string.IsNullOrEmpty(Content))
                    return false;

                Content = Content.Decompress();

                if (!Content.ValidJson())
                    return false;

                var Parse = JObject.Parse(Content);

                if (Parse["Username"].Value<string>() != Environment.UserName)
                    return false;

                DateTime CurrentDate = DateTime.Now;

                foreach (string Day in Parse["Days"])
                {
                    if (Day == CurrentDate.ToString("dd/MM/yyyy"))
                        return true;
                }

                Log.Warning("Key expired");
                File.Delete(Path);
                return false;
            }
            catch (Exception Exception)
            {
                Log.Error(Exception, "Error while validating key");
                return false;
            }
        }

        private const string Domain = "https://galaxyswapperv2.com/Key/Valid.php";
        public static bool Activate(string Activation)
        {
            var stopwatch = new Stopwatch(); stopwatch.Start();

            using (var client = new RestClient())
            {
                var request = new RestRequest(new Uri(Domain), Method.Get);
                request.AddHeader("version", Global.Version);
                request.AddHeader("apiversion", Global.Version);
                request.AddHeader("activation", Activation);
                request.AddHeader("auth", "galaxyswapperv2");

                Log.Information($"Sending {request.Method} request to {Domain}");
                var response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Log.Fatal($"Failed to download response from endpoint! Expected: {HttpStatusCode.OK} Received: {response.StatusCode}");
                    Message.DisplaySTA("Error", "Webclient caught a exception while downloading response from Endpoint.", MessageBoxButton.OK, solutions: new List<string> { "Disable Windows Defender Firewall", "Disable any anti-virus softwares", "Turn on a VPN" }, close: true);
                }

                Log.Information($"Finished {request.Method} request in {stopwatch.GetElaspedAndStop().ToString("mm':'ss")} received {response.Content.Length}");

                var parse = JsonConvert.DeserializeObject<JObject>(response.Content);
                
                switch (parse["status"].Value<int>())
                {
                    case 200:
                        if (!Create(parse["days"].Value<int>()))
                            return false;

                        Message.Display(Languages.Read(Languages.Type.Header, "Info"), string.Format(Languages.Read(Languages.Type.Message, "LoginSuccess"), parse["days"].Value<int>()), MessageBoxButton.OK);
                        return true;
                    case 409:
                        Message.Display(Languages.Read(Languages.Type.Header, "Warning"), Languages.Read(Languages.Type.Message, "LoginInvalid"), MessageBoxButton.OK);
                        return false;
                    default:
                        Message.Display(Languages.Read(Languages.Type.Header, "Error"), parse["message"].Value<string>(), MessageBoxButton.OK, solutions: new () { "Disable Windows Defender Firewall", "Disable any anti-virus softwares", "Turn on a VPN" });
                        return false;
                }
            }
        }

        private static bool Create(int Days)
        {
            try
            {
                if (File.Exists(Path))
                    File.Delete(Path);

                DateTime CurrentDate = DateTime.Now;

                var Array = new JArray
                {
                    CurrentDate.ToString("dd/MM/yyyy")
                };

                for (int i = 1; i < Days; i++)
                {
                    Array.Add(CurrentDate.AddDays(i).ToString("dd/MM/yyyy"));
                }

                var Object = JObject.FromObject(new
                {
                    Username = Environment.UserName,
                    Days = Array
                });

                string Content = Object.ToString(Newtonsoft.Json.Formatting.None).Compress();

                File.WriteAllText(Path, Content);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}