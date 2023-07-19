using System;
using System.IO;

namespace Galaxy_Swapper_v2.Workspace.Properties
{
    /// <summary>
    /// All the code below was provided from: https://github.com/GalaxySwapperOfficial/Galaxy-Swapper-v2
    /// You can also find us at https://galaxyswapperv2.com/Guilded
    /// </summary>
    public static class Config
    {
        public static readonly string Path = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Galaxy-Swapper-v2-Config";
        public static readonly string[] Directories = { Path, $"{Path}\\DLLS", $"{Path}\\Plugins", $"{Path}\\LOGS" };
        public static void Initialize()
        {
            if (File.Exists($"{Path}\\Key.config")) //Old Config Folder
                Directory.Delete(Path, true);

            foreach (string Dir in Directories)
            {
                Directory.CreateDirectory(Dir);
            }
        }
    }
}