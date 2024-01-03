using Galaxy_Swapper_v2.Workspace.Hashes;
using Galaxy_Swapper_v2.Workspace.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using static Galaxy_Swapper_v2.Workspace.Global;

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

        public static void LoadImage(this Image image, string url, string invalid = "https://github.com/GalaxySwapperOfficial/Galaxy-Swapper-API/blob/main/In%20Game/Icons/invalid.png?raw=true")
        {
            if (image is null)
            {
                Log.Error($"LoadImage image is null skipping");
                return;
            }

            var bitmapImage = new BitmapImage();

            if (url is null)
            {
                Log.Error($"LoadImage url Is null loading as invalid");
                bitmapImage = new BitmapImage(new Uri(invalid, UriKind.RelativeOrAbsolute));
                return;
            }

            string name = $"{Fnv1a.Hash(url)}-{image.Width}x{image.Height}.cache";

            try
            {
                if (!ImageCache.ReadCache(name, bitmapImage))
                {
                    bitmapImage.DownloadFailed += IconDownloadFailed;
                    bitmapImage.DownloadCompleted += IconDownloadComplete;
                    bitmapImage = new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"LoadImage caught exception while loading {url} to 'bitmapImage': {ex.Message}");
                bitmapImage = new BitmapImage(new Uri(invalid, UriKind.RelativeOrAbsolute));
            }

            image.Source = bitmapImage;

            void IconDownloadFailed(object sender, ExceptionEventArgs e)
            {
                Log.Error($"IconDownloadFailed event was triggered loading as invalid url");
                bitmapImage = new BitmapImage(new Uri(invalid, UriKind.RelativeOrAbsolute));
                image.Source = bitmapImage;
            }

            void IconDownloadComplete(object sender, EventArgs e)
            {
                ImageCache.Cache(name, bitmapImage);
            }
        }

        public static BitmapImage LoadImageToBitmap(string url, string invalid = "https://github.com/GalaxySwapperOfficial/Galaxy-Swapper-API/blob/main/In%20Game/Icons/invalid.png?raw=true")
        {
            var bitmapImage = new BitmapImage();

            if (url is null)
            {
                Log.Error($"LoadImageToBitmap url Is null loading as invalid");
                bitmapImage = new BitmapImage(new Uri(invalid, UriKind.RelativeOrAbsolute));
                return bitmapImage;
            }

            try
            {
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(url, UriKind.RelativeOrAbsolute);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"LoadImageToBitmap caught exception while loading {url} to 'bitmapImage': {ex.Message}");
                bitmapImage = new BitmapImage(new Uri(invalid, UriKind.RelativeOrAbsolute));
            }

            return bitmapImage;
        }

        public static string Hash(string filePath)
        {
            using SHA256 sHA = SHA256.Create();
            using FileStream inputStream = File.OpenRead(filePath);
            return BitConverter.ToString(sHA.ComputeHash(inputStream)).Replace("-", string.Empty);
        }

        public static TimeSpan GetElaspedAndStop(this Stopwatch stopwatch)
        {
            TimeSpan timespan = stopwatch.Elapsed;
            stopwatch.Stop();
            return timespan;
        }

        public static bool ValidImage(string URL)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = client.Send(new HttpRequestMessage(HttpMethod.Head, URL));

                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (HttpRequestException)
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

        public static void Download(string path, string url, string name = "file")
        {
            var stopwatch = new Stopwatch(); stopwatch.Start();

            using (WebClient WC = new WebClient())
            {
                Log.Information($"Downloading data from: {url} to: {path}");

                try
                {
                    WC.DownloadFile(url, path);
                    WC.Dispose();
                }
                catch (IOException ioException)
                {
                    var driveInfo = new DriveInfo(path);
                    Log.Error(ioException, $"Failed to download {name} drive: {driveInfo.Name} has ran out of space");
                    throw new CustomException($"Failed to download {name}! Drive {driveInfo.Name} has ran out of space!\nMake room in {driveInfo.Name} and try again.");
                }
                catch (Exception Exception)
                {
                    Log.Error(Exception, $"Failed to download {name}");
                    Message.DisplaySTA("Error", $"Webclient caught a exception while downloading {name}!", discord: true, solutions: new[] { "Disable Windows Defender Firewall", "Disable any anti-virus softwares", "Turn on a VPN" });
                    throw new CustomException($"Webclient caught a exception while downloading {name}!");
                }
            }

            Log.Information($"Downloaded {name} to: {path} in {stopwatch.GetElaspedAndStop().ToString("mm':'ss")}");
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

        public static int IndexOfSequenceReverse(this byte[] data, byte[] pattern)
        {
            if (data == null || pattern == null || data.Length == 0 || pattern.Length == 0 || pattern.Length > data.Length)
            {
                return -1; // Invalid input
            }

            for (int i = data.Length - pattern.Length; i >= 0; i--)
            {
                bool match = true;

                for (int j = 0; j < pattern.Length; j++)
                {
                    if (data[i + j] != pattern[j])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    return i; // Pattern found
                }
            }

            return -1; // Pattern not found
        }

        public static long IndexOfSequence(Stream stream, byte[] pattern, long pos = 0)
        {
            long bufferSize = 4096;
            byte[] buffer = new byte[bufferSize];
            int patternLength = pattern.Length;
            int matchIndex = 0;

            stream.Seek(pos, SeekOrigin.Begin);

            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                    break;

                for (int i = 0; i < bytesRead; i++)
                {
                    if (buffer[i] == pattern[matchIndex])
                    {
                        matchIndex++;

                        if (matchIndex == patternLength)
                        {
                            return stream.Position - bytesRead + i + 1 - patternLength;
                        }
                    }
                    else
                    {
                        matchIndex = 0;
                    }
                }
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

        public static string FindFileByExtension(string path, string exten)
        {
            string[] foundFiles = Directory.GetFiles(path, $"*{exten}", SearchOption.AllDirectories);

            if (foundFiles.Length == 0) 
                return null!;
            else
                return foundFiles[0];
        }

        public static bool TryDelete(FileInfo fileInfo)
        {
            try
            {
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }
            }
            catch (Exception Exception)
            {
                Log.Error(Exception, $"Failed to remove {fileInfo.FullName}");
                return false;
            }

            return true;
        }

        public static bool TryDelete(string file)
        {
            try
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
            catch (Exception Exception)
            {
                Log.Error(Exception, $"Failed to remove {file}");
                return false;
            }

            return true;
        }

        public static string ByteToHex(byte[] byteArray) => BitConverter.ToString(byteArray).Replace("-", " ");

        public static long FileLength(this string path) => new FileInfo(path).Length;

        public static byte[] CreateNull(int Length)
        {
            return new byte[Length];
        }

        public static bool FilesInUse(DirectoryInfo directoryInfo)
        {
            foreach (var fileInfo in directoryInfo.GetFiles())
            {
                if (!fileInfo.Exists)
                {
                    continue;
                }
                else if (!CanEdit(fileInfo.FullName))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool FilesInUse(string directory)
        {
            foreach (var fileInfo in new DirectoryInfo(directory).GetFiles())
            {
                if (!fileInfo.Exists)
                {
                    continue;
                }
                else if (!CanEdit(fileInfo.FullName))
                {
                    return true;
                }
            }

            return false;
        }

        public static JObject TryParse(string content)
        {
            try
            {
                return JsonConvert.DeserializeObject<JObject>(content);
            }
            catch (Exception Exception)
            {
                Log.Error(Exception, "Failed to parse Json content");
                return null!;
            }
        }
    }
}