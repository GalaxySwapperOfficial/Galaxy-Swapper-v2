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
        public static readonly string Path = $"{Config.Path}\\Plugins";
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

            var Parse = JObject.Parse(Content);

            if (Parse["Icon"].KeyIsNullOrEmpty())
            {
                Log.Error($"{name} does not contain Icon?");
                Message.Display("Error", $"{name} does not contain Icon!", System.Windows.MessageBoxButton.OK);
                return false;
            }
            if (Parse["Swapicon"].KeyIsNullOrEmpty())
            {
                Log.Error($"{name} does not contain Swapicon?");
                Message.Display("Error", $"{name} does not contain Swapicon!", System.Windows.MessageBoxButton.OK);
                return false;
            }

            if (!Misc.ValidImage(Parse["Icon"].Value<string>()))
            {
                Log.Error($"{name} Icon is a invalid url");
                Message.Display("Error", $"{name} Icon is a invalid url!", System.Windows.MessageBoxButton.OK);
                return false;
            }
            if (!Misc.ValidImage(Parse["Swapicon"].Value<string>()))
            {
                Log.Error($"{name} Swapicon is a invalid url");
                Message.Display("Error", $"{name} Swapicon is a invalid url!", System.Windows.MessageBoxButton.OK);
                return false;
            }
            if (!Parse["OverrideIcon"].KeyIsNullOrEmpty() && !Misc.ValidImage(Parse["OverrideIcon"].Value<string>()))
            {
                Log.Error($"{name} OverrideIcon is a invalid url");
                Message.Display("Error", $"{name} OverrideIcon is a invalid url!", System.Windows.MessageBoxButton.OK);
                return false;
            }

            if (Parse["Downloadables"] != null)
            {
                foreach (var downloadable in Parse["Downloadables"])
                {
                    int Index = (Parse["Downloadables"] as JArray).IndexOf(downloadable) + 1;
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
            }

            foreach (var Asset in Parse["Assets"])
            {
                int Index = (Parse["Assets"] as JArray).IndexOf(Asset) + 1;
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

            Log.Information($"Encrypting {name}");
            Content = Compression.Compress(Encoding.ASCII.GetBytes(Content));

            File.WriteAllText($"{Path}\\{new Random().Next(100000000, 999999999)}-{new Random().Next(100000000, 999999999)}.encrypted", Content);
            Log.Information($"Wrote {name} to {Path}");
            return true;
        }

        public static List<string> List() => Directory.GetFiles(Path).ToList();
    }
}
