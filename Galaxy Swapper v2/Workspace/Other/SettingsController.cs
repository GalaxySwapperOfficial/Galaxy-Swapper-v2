using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy_Swapper_v2.Workspace.Other
{
    public static class SettingsController
    {
        public static string ColorsSettingsRaw()
        {
            JObject Json = JObject.FromObject(new
            {
                MHex = "#171a24",
                SHex = "#1e1e2f",
                TextHex = "#ffffff",
                ButtonHex = "#1e202f",
                HHex = "#151521",
                SecTextHex = "#3A3F4B"
            });
            return Json.ToString();
        }
        public static string ConfigSettingsRaw()
        {
            JObject Json = JObject.FromObject(new
            {
                IconSize = 80,
                EmoteWarning = true,
                DiscordRPC = true,
                HideNonePerfectDefaults = true,
            });
            return Json.ToString();
        }
        public static void Initialize()
        {
            if (!Directory.Exists(Global.Appdata + "\\Galaxy Swapper v2"))
                Directory.CreateDirectory(Global.Appdata + "\\Galaxy Swapper v2");
            if (!File.Exists(Global.Appdata + "\\Galaxy Swapper v2\\Colors.json"))
            {
                File.WriteAllText(Global.Appdata + "\\Galaxy Swapper v2\\Colors.json", ColorsSettingsRaw());
            }
            if (!File.Exists(Global.Appdata + "\\Galaxy Swapper v2\\Settings.config"))
            {
                File.WriteAllText(Global.Appdata + "\\Galaxy Swapper v2\\Settings.config", ConfigSettingsRaw());
            }
            if (!File.Exists(Global.Appdata + "\\Galaxy Swapper v2\\Swaps.log"))
            {
                File.WriteAllText(Global.Appdata + "\\Galaxy Swapper v2\\Swaps.log", "[]");
            }

            //If The User Starts The Application Then Closes It The Settings File Can Corrupt So We Will Check Here
            if (File.ReadAllText(Global.Appdata + "\\Galaxy Swapper v2\\Colors.json").ValidateJSON() != true)
            {
                File.WriteAllText(Global.Appdata + "\\Galaxy Swapper v2\\Colors.json", ColorsSettingsRaw());
            }
            if (File.ReadAllText(Global.Appdata + "\\Galaxy Swapper v2\\Settings.config").ValidateJSON() != true)
            {
                File.WriteAllText(Global.Appdata + "\\Galaxy Swapper v2\\Settings.config", ConfigSettingsRaw());
            }
            if (File.ReadAllText(Global.Appdata + "\\Galaxy Swapper v2\\Swaps.log").ValidateJSON() != true)
            {
                File.WriteAllText(Global.Appdata + "\\Galaxy Swapper v2\\Swaps.log", ConfigSettingsRaw());
            }
        }
        public static string SettingsReturn(string JsonValue)
        {
            return JObject.Parse(File.ReadAllText(Global.Appdata + "\\Galaxy Swapper v2\\Colors.json"))[JsonValue].ToString();
        }
        public static string ConfigReturn(string JsonValue)
        {
            return JObject.Parse(File.ReadAllText(Global.Appdata + "\\Galaxy Swapper v2\\Settings.config"))[JsonValue].ToString();
        }
    }
}
