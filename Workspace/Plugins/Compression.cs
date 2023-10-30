using Galaxy_Swapper_v2.Workspace.Compression;
using Galaxy_Swapper_v2.Workspace.Swapping.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy_Swapper_v2.Workspace.Plugins
{
    public static class Compression
    {
        public enum Type
        {
            None = 0,
            Aes = 1,
            Oodle
        }

        public static bool Compress(out byte[] buffer, out byte[] key, string content, Type type)
        {
            buffer = null!;
            key = GenerateKey(16);

            switch (type)
            {
                case Type.None:
                    buffer = Encoding.ASCII.GetBytes(content);
                    break;
                case Type.Aes:
                    buffer = aes.Encrypt(content, key);
                    break;
            }

            return true;
        }

        public static bool Decompress(byte[] buffer, byte[] key, out string decompressed, Type type)
        {
            decompressed = null!;
            switch (type)
            {
                case Type.None:
                    decompressed = Encoding.ASCII.GetString(buffer);
                    break;
                case Type.Aes:
                    decompressed = aes.Decrypt(buffer, key);
                    break;
            }

            return true;
        }

        private static byte[] GenerateKey(int keyLengthInBytes)
        {
            using (RNGCryptoServiceProvider rngCrypto = new RNGCryptoServiceProvider())
            {
                byte[] randomKey = new byte[keyLengthInBytes];
                rngCrypto.GetBytes(randomKey);
                return randomKey;
            }
        }
    }
}
