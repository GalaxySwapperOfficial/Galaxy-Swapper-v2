using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Galaxy_Swapper_v2.Workspace.Utilities
{
    public static class Misc
    {
        public static SolidColorBrush HexToBrush(this string hex) => new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
        public static Color HexToColor(this string hex) => (Color)ColorConverter.ConvertFromString(hex);

        public static bool ValidJson(this string Content)
        {
            try
            {
                JObject.Parse(Content);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool ValidArray(this string Content)
        {
            try
            {
                JArray.Parse(Content);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void UrlStart(this string url)
        {
            ProcessStartInfo Procc = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C start {url}",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            };
            Process.Start(Procc);
        }

        public static bool Encrypted(this string JSON) => Regex.IsMatch(JSON, @"^[a-zA-Z0-9\+/]*={0,2}$");

        public static byte[] AssetLengthBlock(int Length)
        {
            byte[] Return = new byte[5];
            Buffer.BlockCopy(BitConverter.GetBytes(Length), 0, Return, 0, 4);
            Array.Reverse(Return);
            return Return;
        }

        public static byte[] CompressionBlock(uint Offset, uint CompressedSize, uint UncompressedSize, uint PakIndex, bool IsCompressed)
        {
            var CompressionData = new byte[12];
            Buffer.BlockCopy(BitConverter.GetBytes(Offset), 0, CompressionData, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(PakIndex), 0, CompressionData, 4, 1);
            Buffer.BlockCopy(BitConverter.GetBytes(CompressedSize), 0, CompressionData, 5, 3);
            Buffer.BlockCopy(BitConverter.GetBytes(UncompressedSize), 0, CompressionData, 8, 3);

            if (IsCompressed)
                Buffer.BlockCopy(BitConverter.GetBytes(1), 0, CompressionData, 11, 1);

            return CompressionData;
        }

        public static void LoadImage(this Image Image, string url, string invalid = "https://github.com/GalaxySwapperOfficial/Galaxy-Swapper-API/blob/main/In%20Game/Icons/invalid.png?raw=true")
        {
            var Icon = new BitmapImage();

            Icon.BeginInit();
            Icon.UriSource = new Uri(url, UriKind.RelativeOrAbsolute);
            Icon.CacheOption = BitmapCacheOption.OnLoad;
            Icon.DownloadFailed += IconDownloadFailed;
            Icon.EndInit();

            void IconDownloadFailed(object sender, ExceptionEventArgs e)
            {
                Icon = new BitmapImage();
                Icon.BeginInit();
                Icon.UriSource = new Uri(invalid, UriKind.RelativeOrAbsolute);
                Icon.CacheOption = BitmapCacheOption.OnLoad;
                Icon.DownloadFailed += IconDownloadFailed;
                Icon.EndInit();

                Image.Source = Icon;
            }

            Image.Source = Icon;
        }

        public static bool ValidImage(string URL)
        {
            using (WebClient WC = new WebClient())
            {
                try
                {
                    WC.DownloadString(URL);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static byte[] MatchToByte(byte[] content, int ByeLength)
        {
            byte[] result = new byte[ByeLength];
            Buffer.BlockCopy(content, 0, result, 0, ByeLength);
            return result;
        }

        public static bool KeyIsNullOrEmpty(this JToken token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
                   (token.Type == JTokenType.Null);
        }

        public static bool CanEdit(this string file)
        {
            try
            {
                using (FileStream Filestream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    Filestream.Close();
                    return true;
                }
            }
            catch (IOException)
            {
                return false;
            }
        }

        public static int IndexOfSequence(this byte[] buffer, byte[] pattern, int Star)
        {
            int Start = 0;
            if (Star != 0)
            {
                Start = Star;
            }
            int i = Array.IndexOf(buffer, pattern[0], Start);
            while (i >= 0 && i <= buffer.Length - pattern.Length)
            {
                byte[] segment = new byte[pattern.Length];
                Buffer.BlockCopy(buffer, i, segment, 0, pattern.Length);
                if (segment.SequenceEqual(pattern))
                    return i;
                i = Array.IndexOf(buffer, pattern[0], i + 1);
            }
            return -1;
        }

        public static byte[] HexToByte(this string hexString)
        {
            hexString = hexString.Replace(" ", "");
            byte[] data = new byte[hexString.Length / 2];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }
            return data;
        }

        public static string ByteToHex(byte[] byteArray) => BitConverter.ToString(byteArray).Replace("-", " ");

        public static long FileLength(this string path) => new FileInfo(path).Length;

        public static byte[] CreateNull(int Length)
        {
            return new byte[Length];
        }
    }
}
