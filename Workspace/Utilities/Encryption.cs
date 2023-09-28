using Serilog;
using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace Galaxy_Swapper_v2.Workspace.Utilities
{
    public static class Encryption
    {
        public static string Compress(this string uncompressedString)
        {
            byte[] compressedBytes;

            using (var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(uncompressedString)))
            {
                using (var compressedStream = new MemoryStream())
                {
                    using (var compressorStream = new DeflateStream(compressedStream, CompressionLevel.Optimal, true))
                    {
                        uncompressedStream.CopyTo(compressorStream);
                    }
                    compressedBytes = compressedStream.ToArray();
                }
            }
            return Encrypt(Convert.ToBase64String(compressedBytes));
        }
        public static string Decompress(this string compressedString)
        {
            byte[] DecompressedBytes = null;
            using (var CompressedStream = new MemoryStream(Convert.FromBase64String(Decrypt(compressedString))))
            {
                using (var DecompressStream = new DeflateStream(CompressedStream, CompressionMode.Decompress))
                {
                    using (var DecompressedStream = new MemoryStream())
                    {
                        DecompressStream.CopyTo(DecompressedStream);
                        DecompressedBytes = DecompressedStream.ToArray();
                    }
                }
            }
            return Encoding.UTF8.GetString(DecompressedBytes);
        }
        static string Encrypt(string plainText)
        {
            string text = "ᣤᣤ";
            string text2 = "졊젙졅졎젡졇졂졗";
            string s = "@1B2c3D5e5F5b7H8";
            int num = (int)text[0];
            int num2 = num;
            num = (num2 ^ 25064);
            int num3 = num;
            num = num3 + 3829;
            int num4 = num;
            int num5 = num4 << 5;
            int num6 = num;
            int num7 = num6 & 65535;
            int num8 = num5 | num7 >> 11;
            num = (num8 & 65535);
            string text3 = text;
            string str = text3.Substring(0, 0);
            int num9 = num;
            string str2 = ((char)(num9 & 65535)).ToString();
            string text4 = text;
            text = str + str2 + text4.Substring(1);
            int num10 = (int)text2[0];
            int num11 = num10;
            num10 = num11 - 51191;
            num10 -= 0;
            string text5 = text2;
            string str3 = text5.Substring(0, 0);
            int num12 = num10;
            string str4 = ((char)(num12 & 65535)).ToString();
            string text6 = text2;
            text2 = str3 + str4 + text6.Substring(1);
            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            DeriveBytes deriveBytes = new Rfc2898DeriveBytes(text, Encoding.ASCII.GetBytes(text2));
            byte[] bytes2 = deriveBytes.GetBytes(32);
            RijndaelManaged rijndaelManaged = new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.Zeros
            };
            ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor(bytes2, Encoding.ASCII.GetBytes(s));
            byte[] inArray;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Stream stream = memoryStream;
                ICryptoTransform transform = cryptoTransform;
                using (CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Write))
                {
                    Stream stream2 = cryptoStream;
                    byte[] buffer = bytes;
                    stream2.Write(buffer, 0, bytes.Length);
                    cryptoStream.FlushFinalBlock();
                    inArray = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(inArray);
        }
        static string Decrypt(string encryptedText)
        {
            string text = "ᣤᣤ";
            string text2 = "졊젙졅졎젡졇졂졗";
            string s = "@1B2c3D5e5F5b7H8";
            int num = (int)text[0];
            int num2 = num;
            num = (num2 ^ 25064);
            int num3 = num;
            num = num3 + 3829;
            int num4 = num;
            int num5 = num4 << 5;
            int num6 = num;
            int num7 = num6 & 65535;
            int num8 = num5 | num7 >> 11;
            num = (num8 & 65535);
            string text3 = text;
            string str = text3.Substring(0, 0);
            int num9 = num;
            string str2 = ((char)(num9 & 65535)).ToString();
            string text4 = text;
            text = str + str2 + text4.Substring(1);
            int num10 = (int)text2[0];
            int num11 = num10;
            num10 = num11 - 51191;
            num10 -= 0;
            string text5 = text2;
            string str3 = text5.Substring(0, 0);
            int num12 = num10;
            string str4 = ((char)(num12 & 65535)).ToString();
            string text6 = text2;
            text2 = str3 + str4 + text6.Substring(1);
            byte[] array = Convert.FromBase64String(encryptedText);
            DeriveBytes deriveBytes = new Rfc2898DeriveBytes(text, Encoding.ASCII.GetBytes(text2));
            byte[] bytes = deriveBytes.GetBytes(32);
            RijndaelManaged rijndaelManaged = new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.None
            };
            ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor(bytes, Encoding.ASCII.GetBytes(s));
            MemoryStream memoryStream = new MemoryStream(array);
            Stream stream = memoryStream;
            ICryptoTransform transform = cryptoTransform;
            CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read);
            byte[] array2 = new byte[array.Length];
            Stream stream2 = cryptoStream;
            byte[] buffer = array2;
            int count = stream2.Read(buffer, 0, array2.Length);
            memoryStream.Close();
            cryptoStream.Close();
            Encoding utf = Encoding.UTF8;
            byte[] bytes2 = array2;
            return utf.GetString(bytes2, 0, count).TrimEnd("\0".ToCharArray());
        }
    }
}