using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System.IO;
using System.Windows;

namespace Galaxy_Swapper_v2.Workspace.Plugins
{
    public static class Validate
    {
        public static bool IsValid(FileInfo fileInfo, out JObject parse)
        {
            parse = null!;

            string content = File.ReadAllText(fileInfo.FullName);

            if (fileInfo.Extension.SubstringAfter('.').ToUpper() == "PLUGIN" && !content.Contains("Object")) //Most likely a packed plugin
            {
                var exported = Plugin.Export(fileInfo);
                if (exported is null)
                {
                    Log.Information($"{fileInfo.Name} failed to unpack plugin file");
                    Message.Display("Error", $"Failed to unpack {fileInfo.Name}.", MessageBoxButton.OK);
                    return false;
                }
                parse = exported.Parse;
            }
            else
            {
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
            }

            if (parse["Icon"].KeyIsNullOrEmpty())
            {
                Log.Error($"{fileInfo.Name} does not contain 'Icon' value");
                Message.Display("Error", $"{fileInfo.Name} does not contain 'Icon' value!", MessageBoxButton.OK);
                return false;
            }

            if (!Misc.ValidImage(parse["Icon"].Value<string>()))
            {
                Log.Error($"{fileInfo.Name} 'Icon' url is invalid and will be set to fallback");
                parse["Icon"] = Global.InvalidPluginIcon;
            }

            if (!parse["FrontendIcon"].KeyIsNullOrEmpty() && !Misc.ValidImage(parse["FrontendIcon"].Value<string>()))
            {
                Log.Error($"{fileInfo.Name} 'FrontendIcon' url is invalid and will be set to fallback");
                parse["FrontendIcon"] = Global.InvalidPluginIcon;
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

            if (parse["Socials"] is not null)
            {
                foreach (var social in parse["Socials"])
                {
                    int index = (parse["Socials"] as JArray).IndexOf(social) + 1;

                    if (social["type"].KeyIsNullOrEmpty())
                    {
                        Log.Error($"{fileInfo.Name} 'Socials' array {index} 'type' is null or empty");
                        Message.Display("Error", $"{fileInfo.Name} 'Socials' array {index} 'type' is null or empty", MessageBoxButton.OK);
                        return false;
                    }
                }
            }

            if (parse["Type"].KeyIsNullOrEmpty())
            {
                return None(ref fileInfo, ref parse);
            }

            switch (parse["Type"].Value<string>())
            {
                case "UEFN_Character":
                    return UEFN_Character(ref fileInfo, ref parse);
                default:
                    return None(ref fileInfo, ref parse);
            }
        }

        #region PluginTypes
        public static bool None(ref FileInfo fileInfo, ref JObject parse)
        {
            if (parse["Swapicon"].KeyIsNullOrEmpty())
            {
                Log.Error($"{fileInfo.Name} does not contain 'Swapicon' value");
                Message.Display("Error", $"{fileInfo.Name} does not contain 'Swapicon' value!", MessageBoxButton.OK);
                return false;
            }

            if (!Misc.ValidImage(parse["Swapicon"].Value<string>()))
            {
                Log.Error($"{fileInfo.Name} 'Swapicon' url is invalid and will be set to fallback");
                parse["Swapicon"] = Global.InvalidPluginIcon;
            }

            foreach (var asset in parse["Assets"])
            {
                int index = (parse["Assets"] as JArray).IndexOf(asset) + 1;

                if (asset["AssetPath"].KeyIsNullOrEmpty())
                {
                    Log.Error($"{fileInfo.Name} assets array {index} does not contain 'AssetPath' value");
                    Message.Display("Error", $"{fileInfo.Name} assets array {index} does not contain 'AssetPath' value", MessageBoxButton.OK);
                    return false;
                }

                if (asset["Swaps"] is not null)
                {
                    foreach (var swap in asset["Swaps"])
                    {
                        int sindex = (asset["Swaps"] as JArray).IndexOf(swap) + 1;
                        string[] objects = { "search", "replace", "type" };

                        foreach (string obj in objects)
                        {
                            if (swap[obj].KeyIsNullOrEmpty())
                            {
                                Log.Error($"{fileInfo.Name} assets array {index} swaps array {sindex} does not contain '{obj}' value");
                                Message.Display("Error", $"{fileInfo.Name} assets array {index} swaps array {sindex} does not contain '{obj}' value", MessageBoxButton.OK);
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        public static bool UEFN_Character(ref FileInfo fileInfo, ref JObject parse)
        {
            if (parse["AssetPathTo"].KeyIsNullOrEmpty())
            {
                Log.Error($"{fileInfo.Name} does not contain 'AssetPathTo' value");
                Message.Display("Error", $"{fileInfo.Name} does not contain 'AssetPathTo' value", MessageBoxButton.OK);
                return false;
            }

            if (parse["Swaps"] is not null)
            {
                foreach (var swap in parse["Swaps"])
                {
                    int sindex = (parse["Swaps"] as JArray).IndexOf(swap) + 1;
                    string[] objects = { "search", "replace", "type" };

                    foreach (string obj in objects)
                    {
                        if (swap[obj].KeyIsNullOrEmpty())
                        {
                            Log.Error($"{fileInfo.Name} 'Swaps' array {sindex} does not contain '{obj}' value");
                            Message.Display("Error", $"{fileInfo.Name} 'Swaps' array {sindex} does not contain '{obj}' value", MessageBoxButton.OK);
                            return false;
                        }
                    }
                }
            }

            return true;
        }
        #endregion
    }
}
