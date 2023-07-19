using System;

namespace Galaxy_Swapper_v2.Workspace
{
    public static class Global
    {
        public const string Version = "1.11";
        public const string ApiVersion = "1.06";
        public static string Discord { get; set; } = default!;
        public static string Website { get; set; } = default!;
        public static string Download { get; set; } = default!;
        public static string Key { get; set; } = default!;

        public class CustomException : Exception
        {
            public CustomException(string message) : base(message)
            {
            }

            public CustomException(string message, Exception innerException) : base(message, innerException)
            {
            }
        }
    }
}
