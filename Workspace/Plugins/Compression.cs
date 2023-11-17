using Galaxy_Swapper_v2.Workspace.Compression;
using Galaxy_Swapper_v2.Workspace.Hashes;
using Galaxy_Swapper_v2.Workspace.Swapping.Compression.Types;
using Galaxy_Swapper_v2.Workspace.Swapping.Other;
using Serilog;
using System;
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
            Zlib = 2,
            Oodle = 3,
            GZip = 4
        }

        public static bool Compress(out byte[] buffer, out byte[] key, out int uncompressedsize, string content, Type type)
        {
            buffer = null!;
            uncompressedsize = 0;
            key = null!;

            byte[] uncompressed = Encoding.ASCII.GetBytes(content);
            try
            {
                switch (type)
                {
                    case Type.None:
                        buffer = uncompressed;
                        break;
                    case Type.Aes:
                        key = GenerateKey(16);
                        buffer = aes.Encrypt(content, key);
                        break;
                    case Type.Zlib:
                        buffer = zlib.Compress(uncompressed);
                        uncompressedsize = uncompressed.Length;
                        break;
                    case Type.Oodle:
                        buffer = Oodle.Compress(uncompressed, Oodle.OodleCompressionLevel.Level5);
                        uncompressedsize = uncompressed.Length;
                        break;
                    case Type.GZip:
                        buffer = gzip.Compress(uncompressed);
                        break;
                }
            }
            catch (Exception Exception)
            {
                Log.Error(Exception, $"Failed to compress plugin file with type {type}");
                return false;
            }

            return true;
        }

        public static bool Decompress(Reader reader, FileInfo fileInfo, out string decompressed, Type type)
        {
            decompressed = null!;
            bool haskey = false;
            bool hasuncompressedsize = false;

            //Check for key or uncompressed size
            switch (type)
            {
                case Type.Aes:
                    haskey = true;
                    break;
                case Type.Zlib:
                    hasuncompressedsize = true;
                    break;
                case Type.Oodle:
                    hasuncompressedsize = true;
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

            int uncompressedsize = 0;
            if (hasuncompressedsize)
            {
                uncompressedsize = reader.Read<int>();
            }

            ulong pluginhash = reader.Read<ulong>();
            int pluginlength = reader.Read<int>();
            byte[] compressedbuffer = reader.ReadBytes(pluginlength);

            if (CityHash.Hash(compressedbuffer) != pluginhash)
            {
                Log.Warning($"{fileInfo.Name} buffer hash was not as expected plugin will be skipped");
                return false;
            }

            try
            {
                switch (type)
                {
                    case Type.None:
                        decompressed = Encoding.ASCII.GetString(compressedbuffer);
                        break;
                    case Type.Aes:
                        decompressed = Encoding.ASCII.GetString(aes.Decrypt(compressedbuffer, key));
                        break;
                    case Type.Zlib:
                        decompressed = Encoding.ASCII.GetString(zlib.Decompress(compressedbuffer, uncompressedsize));
                        break;
                    case Type.Oodle:
                        decompressed = Encoding.ASCII.GetString(Oodle.Decompress(compressedbuffer, uncompressedsize));
                        break;
                    case Type.GZip:
                        decompressed = Encoding.ASCII.GetString(gzip.Decompress(compressedbuffer));
                        break;
                }
            }
            catch (Exception Exception)
            {
                Log.Error(Exception, $"Failed to decompress {fileInfo.Name} plugin with type {type}");
                return false;
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
