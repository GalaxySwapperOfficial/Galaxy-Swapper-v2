using Galaxy_Swapper_v2.Workspace.Compression;
using Galaxy_Swapper_v2.Workspace.Hashes;
using Galaxy_Swapper_v2.Workspace.Swapping.Other;
using Serilog;
using System.IO;
using System.Security.Cryptography;
using System.Text;

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

        public static bool Decompress(Reader reader, FileInfo fileInfo, out string decompressed, Type type)
        {
            decompressed = null!;
            bool haskey = false;

            switch (type)
            {
                case Type.Aes:
                    haskey = true;
                    break;
            }

            byte[] key = null!;
            if (haskey)
            {
                ulong keyhash = reader.Read<ulong>();
                int keylength = reader.Read<int>();
                key = reader.ReadBytes(keylength);

                if (CityHash.Hash(key) != keyhash)
                {
                    Log.Warning($"{fileInfo.Name} encryption key hash was not as expected plugin will be skipped");
                    return false;
                }
            }

            ulong pluginhash = reader.Read<ulong>();
            int pluginlength = reader.Read<int>();
            byte[] compressedbuffer = reader.ReadBytes(pluginlength);

            if (CityHash.Hash(compressedbuffer) != pluginhash)
            {
                Log.Warning($"{fileInfo.Name} buffer hash was not as expected plugin will be skipped");
                return false;
            }

            switch (type)
            {
                case Type.None:
                    decompressed = Encoding.ASCII.GetString(compressedbuffer);
                    break;
                case Type.Aes:
                    decompressed = aes.Decrypt(compressedbuffer, key);
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
