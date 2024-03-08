using System;

namespace Galaxy_Swapper_v2.Workspace
{
    public static class Global
    {
        public const string Version = "1.36";
        public const string ApiVersion = "1.27";
        public static string Discord = "https://galaxyswapperv2.com/Discord.php";
        public static string Website = "https://galaxyswapperv2.com";
        public static string Download = "https://galaxyswapperv2.com/Downloads/InGame.php";
        public static string Key = "https://galaxyswapperv2.com/Downloads/Discord.php";
        public static string FortniteDirectoryTutorial = "https://galaxyswapperv2.com/Videos/FortniteDirectoryInvalid.mp4";
        public static string EpicGamesDirectoryTutorial = "https://galaxyswapperv2.com/Videos/EpicGamesDirectoryInvalid.mp4";
        public const string InvalidPluginIcon = "https://galaxyswapperv2.com/Icons/FallBackPluginImage.png";
        public class CustomException : Exception
        {
            public CustomException(string message) : base(message)
            {
            }

            public CustomException(string message, Exception innerException) : base(message, innerException)
            {
            }
        }

        public class FortniteDirectoryEmptyException : Exception
        {
            public FortniteDirectoryEmptyException(string message) : base(message)
            {
            }

            public FortniteDirectoryEmptyException(string message, Exception innerException) : base(message, innerException)
            {
            }
        }
    }
}
