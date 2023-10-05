using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Galaxy_Swapper_v2.Workspace.Properties
{
    public static class Plugins
    {
        public static readonly string Path = $"{App.Config}\\Plugins";
        public static bool Add(string file)
        {
            string name = System.IO.Path.GetFileName(file);
            if (!File.Exists(file))
            {
                Message.Display("Error", $"{file} does not exist?", System.Windows.MessageBoxButton.OK);
                return false;
            }

            string Content = File.ReadAllText(file);

            if (Content.Encrypted())
            {
                Log.Information($"Decrypting {name}");
                Content = Content.Decompress();
            }

            if (!Content.ValidJson())
            {
                Log.Error($"{name} is not in a valid JSON format!");
                Message.Display("Error", $"{name} is not in a valid JSON format!", System.Windows.MessageBoxButton.OK);
                return false;
            }

            var parse = JObject.Parse(Content);

            if (parse["Icon"].KeyIsNullOrEmpty())
            {
                Log.Error($"{name} does not contain 'Icon' value!");
                Message.Display("Error", $"{name} does not contain 'Icon' value! This plugin will not be imported.", System.Windows.MessageBoxButton.OK);
                return false;
            }

            string icon = parse["Icon"].Value<string>();

            if (!Misc.ValidImage(icon))
            {
                Log.Error($"{name} has a invalid url for 'Icon'.\n{icon}");
                Message.Display("Error", $"{name} has a invalid url for 'Icon'. This plugin will not be imported.", System.Windows.MessageBoxButton.OK);
                return false;
            }

            if (!parse["FrontendIcon"].KeyIsNullOrEmpty() && !Misc.ValidImage(parse["FrontendIcon"].Value<string>()))
            {
                Log.Error($"{name} has a invalid url for 'FrontendIcon'.\n{parse["FrontendIcon"].Value<string>()}");
                Message.Display("Error", $"{name} has a invalid url for 'FrontendIcon'. This plugin will not be imported.", System.Windows.MessageBoxButton.OK);
                return false;
            }

            if (parse["Downloadables"] != null)
            {
                foreach (var downloadable in parse["Downloadables"])
                {
                    int Index = (parse["Downloadables"] as JArray).IndexOf(downloadable) + 1;

                    if (downloadable["zip"].KeyIsNullOrEmpty())
                    {
                        string[] types = { "pak", "sig", "ucas", "utoc" };
                        foreach (string type in types)
                        {
                            if (downloadable[type].KeyIsNullOrEmpty())
                            {
                                Log.Error($"Downloadables array {Index} does not contain {type}");
                                Message.Display("Error", $"Downloadables array {Index} does not contain {type}!", System.Windows.MessageBoxButton.OK);
                                return false;
                            }
                            if (!Misc.ValidImage(downloadable[type].Value<string>()))
                            {
                                Log.Error($"Downloadables array {Index} {type} does not contain a valid URL");
                                Message.Display("Error", $"Downloadables array {Index} {type} does not contain a valid URL!", System.Windows.MessageBoxButton.OK);
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if (!Misc.ValidImage(downloadable["zip"].Value<string>()))
                        {
                            Log.Error($"Downloadables array {Index} zip does not contain a valid URL");
                            Message.Display("Error", $"Downloadables array {Index} zip does not contain a valid URL!", System.Windows.MessageBoxButton.OK);
                            return false;
                        }
                    }
                }
            }

            if (parse["Type"].KeyIsNullOrEmpty() || parse["Type"].Value<string>() == "default")
            {
                Log.Information($"Reading {name} as type default.");
                if (!ValidateDefault()) return false;
            }
            else
            {
                switch (parse["Type"].Value<string>())
                {
                    case "UEFN_Character":
                        Log.Information($"Reading {name} as type UEFN_Character.");
                        if (!ValidateUEFN_Character()) return false;
                        break;
                }
            }

            Log.Information($"Compressing {name}");
            Content = Compression.Compress(Encoding.ASCII.GetBytes(Content));

            File.WriteAllText($"{Path}\\{new Random().Next(100000000, 999999999)}-{new Random().Next(100000000, 999999999)}.encrypted", Content);
            Log.Information($"Wrote {name} to {Path}");

            bool ValidateDefault()
            {
                foreach (var Asset in parse["Assets"])
                {
                    int Index = (parse["Assets"] as JArray).IndexOf(Asset) + 1;
                    if (Asset["AssetPath"].KeyIsNullOrEmpty())
                    {
                        Log.Error($"{name} AssetPath does not exist in assets array {Index}");
                        Message.Display("Error", $"{name} AssetPath does not exist in assets array {Index}", System.Windows.MessageBoxButton.OK);
                        return false;
                    }

                    if (!Asset["AssetPathTo"].KeyIsNullOrEmpty() && Asset["AssetPathTo"].Value<string>().ToLower() == "/game/")
                    {
                        Log.Error($"{name} AssetPathTo in assets array {Index} must be a valid asset.");
                        Message.Display("Error", $"{name} AssetPathTo in assets array {Index} must be a valid asset.\nIf you are trying to invalid it set it as a empty characterpart.", System.Windows.MessageBoxButton.OK);
                        return false;
                    }

                    if (Asset["Swaps"] != null)
                    {
                        foreach (var Swap in Asset["Swaps"])
                        {
                            int SIndex = (Asset["Swaps"] as JArray).IndexOf(Swap) + 1;

                            string[] Objects = { "search", "replace", "type" };
                            foreach (string Object in Objects)
                            {
                                if (Swap[Object] == null)
                                {
                                    Log.Error($"{name} {Object} does not exist in assets array {Index} swaps array {SIndex}");
                                    Message.Display("Error", $"{name} {Object} does not exist in assets array {Index} swaps array {SIndex}", System.Windows.MessageBoxButton.OK);
                                    return false;
                                }
                            }
                        }
                    }
                }

                return true;
            }

            bool ValidateUEFN_Character()
            {
                if (parse["AssetPathTo"].KeyIsNullOrEmpty())
                {
                    Log.Error($"{name} does not contain 'AssetPathTo'! Please set it to a female body characterpart.");
                    Message.Display("Error", $"{name} Does not contain 'AssetPathTo'! Please set it to a female body characterpart.", System.Windows.MessageBoxButton.OK);
                    return false;
                }

                if (parse["Swaps"] == null)
                {
                    Log.Warning($"{name} does not contain a swaps array? Weird.");
                }

                return true;
            }

            return true;
        }

        public static List<string> List() => Directory.GetFiles(Path).ToList();
    }
}
