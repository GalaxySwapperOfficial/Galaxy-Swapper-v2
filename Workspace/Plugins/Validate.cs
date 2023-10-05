using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Formats.Asn1;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Galaxy_Swapper_v2.Workspace.Plugins
{
    public static class Validate
    {
        public static bool IsValid(FileInfo fileInfo, out JObject parse)
        {
            parse = null!;

            string content = File.ReadAllText(fileInfo.FullName);

            if (content.Encrypted())
            {
                Log.Information($"Decrypting {fileInfo.Name}");
                content = content.Decompress();
            }

            if (!content.ValidJson())
            {
                Log.Information($"{fileInfo.Name} is not in a valid JSON format");
                Message.Display("Error", $"{fileInfo.Name} is not in a valid JSON format!", MessageBoxButton.OK);
                return false;
            }

            parse = JObject.Parse(content);

            if (parse["Icon"].KeyIsNullOrEmpty())
            {
                Log.Error($"{fileInfo.Name} does not contain 'Icon' value");
                Message.Display("Error", $"{fileInfo.Name} does not contain 'Icon' value!", System.Windows.MessageBoxButton.OK);
                return false;
            }

            if (!Misc.ValidImage(parse["Icon"].Value<string>()))
            {
                Log.Error($"{fileInfo.Name} 'Icon' url is invalid");
                Message.Display("Error", $"{fileInfo.Name} 'Icon' url is invalid", MessageBoxButton.OK);
                return false;
            }

            if (!parse["FrontendIcon"].KeyIsNullOrEmpty() && !Misc.ValidImage(parse["FrontendIcon"].Value<string>()))
            {
                Log.Error($"{fileInfo.Name} 'FrontendIcon' url is invalid");
                Message.Display("Error", $"{fileInfo.Name} 'FrontendIcon' url is invalid", MessageBoxButton.OK);
                return false;
            }

            if (parse["Downloadables"] is not null)
            {
                foreach (var downloadable in parse["Downloadables"])
                {
                    int index = (parse["Downloadables"] as JArray).IndexOf(downloadable) + 1;

                    if (downloadable["zip"].KeyIsNullOrEmpty())
                    {
                        foreach (string type in new[] { "ucas", "utoc", "pak", "sig" })
                        {
                            if (downloadable[type].KeyIsNullOrEmpty())
                            {
                                Log.Error($"{fileInfo.Name} 'Downloadables' array {index} does not contain '{type}'");
                                Message.Display("Error", $"{fileInfo.Name} 'Downloadables' array {index} does not contain '{type}'", MessageBoxButton.OK);
                                return false;
                            }
                            else if (!Misc.ValidImage(downloadable[type].Value<string>()))
                            {
                                Log.Error($"{fileInfo.Name} 'Downloadables' array {index} '{type}' is a invalid url");
                                Message.Display("Error", $"{fileInfo.Name} 'Downloadables' array {index} '{type}' is a invalid url", MessageBoxButton.OK);
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if (!Misc.ValidImage(downloadable["zip"].Value<string>()))
                        {
                            Log.Error($"{fileInfo.Name} 'Downloadables' array {index} 'zip' is a invalid url");
                            Message.Display("Error", $"{fileInfo.Name} 'Downloadables' array {index} 'zip' is a invalid url", MessageBoxButton.OK);
                            return false;
                        }
                    }
                }
            }

            if (parse["Type"].KeyIsNullOrEmpty())
            {
                return None();
            }

            switch (parse["Type"].Value<string>())
            {
                case "UEFN_Character":
                    return UEFN_Character();
                default:
                    return None();
            }
        }

        #region PluginTypes
        public static bool None()
        {
            return true;
        }

        public static bool UEFN_Character()
        {
            return true;
        }
        #endregion
    }
}
