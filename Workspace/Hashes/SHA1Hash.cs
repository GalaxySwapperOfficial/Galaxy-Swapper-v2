using System;
using System.IO;
using System.Security.Cryptography;

namespace Galaxy_Swapper_v2.Workspace.Hashes
{
    public class SHA1Hash
    {
        public static string HashByteArray(byte[] input)
        {
            return Convert.ToHexString(SHA1.Create().ComputeHash(input));
        }

        public static string HashFile(string path)
        {
            if (!File.Exists(path))
            {
                return string.Empty;
            }
            return Convert.ToHexString(SHA1.Create().ComputeHash(File.ReadAllBytes(path)));
        }

        public static string HashFileStream(FileStream fs)
        {
            if (!File.Exists(fs.Name))
            {
                return string.Empty;
            }
            return Convert.ToHexString(SHA1.Create().ComputeHash(fs));
        }

        public static string HashStream(Stream fs)
        {
            return Convert.ToHexString(SHA1.Create().ComputeHash(fs));
        }
    }
}