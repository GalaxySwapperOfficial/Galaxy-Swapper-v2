using CUE4Parse.Encryption.Aes;
using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Versions;
using Galaxy_Swapper_v2.Properties;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Windows.Forms;

namespace Galaxy_Swapper_v2.Workspace.Other
{
    public static class Cue4parse
    {
        public static string Aeskey;
        public static DefaultFileProvider provider = new DefaultFileProvider(Settings.Default.FortniteInstall, SearchOption.TopDirectoryOnly, true);
        public static byte[] ExportAsset(string file, string Asset)
        {
            if (!File.Exists(Settings.Default.FortniteInstall + "\\global.ucas"))
                MessageBox.Show("Global.ucas Is Missing? Verify Fortnite And Try Again.", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            provider.Initialize(file);
            provider.Versions.Game = EGame.GAME_UE5_0;
            if (Aeskey == null)
            {
                JObject parse = JObject.Parse(FortniteAPI.APIReturn(FortniteAPI.Endpoints.Aes, null));
                if (parse["data"]["mainKey"] == null)
                    Aeskey = "0x0000000000000000000000000000000000000000000000000000000000000000";
                else
                    Aeskey = $"0x{parse["data"]["mainKey"]}";
            }
            provider.SubmitKey(new FGuid(), new FAesKey(Aeskey));
            byte[] Exported = provider.SaveAsset(Asset);
            provider.Dispose();
            GC.Collect();
            return Exported;
        }
    }
}
