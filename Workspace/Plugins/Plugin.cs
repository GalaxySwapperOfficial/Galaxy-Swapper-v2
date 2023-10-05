using Galaxy_Swapper_v2.Workspace.Swapping.Other;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy_Swapper_v2.Workspace.Plugins
{
    public static class Plugin
    {
        public static readonly string Path = $"{App.Config}\\Plugins";
        public static void Import(FileInfo fileInfo, JObject parse)
        {
            var writer = new Writer(new byte[fileInfo.Length + 60000]);

            writer.Write(1); //1 = encrypted in case I want to add other formats later on

            //Import directory
            byte[] importpath = Encoding.ASCII.GetBytes(fileInfo.FullName);
            writer.Write(importpath.Length);
            writer.WriteBytes(importpath);

            //Encryption key
            byte[] key = GenerateKey(16);

            writer.Write(CityHash.Hash(key));
            writer.Write(key.Length); //Will always be 16 but If I do other formats it may change.
            writer.WriteBytes(key);

            //Plugin buffer
            byte[] encrypted = Encrypt(parse.ToString(Newtonsoft.Json.Formatting.None), key);

            writer.Write(CityHash.Hash(encrypted));
            writer.Write(encrypted.Length);
            writer.WriteBytes(encrypted);

            string output = $"{Path}\\{System.IO.Path.GetRandomFileName()}.plugin";
            File.WriteAllBytes(output, writer.ToByteArray(writer.Position));

            Log.Information($"Plugin wrote to: {output}");
        }

        public static bool Export(FileInfo fileInfo)
        {
            var reader = new Reader(File.ReadAllBytes(fileInfo.FullName));

            reader.BaseStream.Position += sizeof(int); //encryption/compression format currently not used

            int importpathlength = reader.Read<int>();
            string importpath = reader.ReadStrings(importpathlength);

            ulong keyhash = reader.Read<ulong>();
            int keylength = reader.Read<int>();
            byte[] key = reader.ReadBytes(keylength);

            if (CityHash.Hash(key) != keyhash)
            {
                Log.Warning($"{fileInfo.Name} encryption key hash was not as expected plugin will be skipped");
                return false;
            }

            ulong pluginhash = reader.Read<ulong>();
            int pluginlength = reader.Read<int>();
            byte[] pluginbuffer = reader.ReadBytes(pluginlength);

            if (CityHash.Hash(pluginbuffer) != pluginhash)
            {
                Log.Warning($"{fileInfo.Name} buffer hash was not as expected plugin will be skipped");
                return false;
            }

            Log.Information(Decrypt(pluginbuffer, key));

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

        private static byte[] Encrypt(string plainText, byte[] key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    memoryStream.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(plainText);
                    }

                    return memoryStream.ToArray();
                }
            }
        }

        private static string Decrypt(byte[] encryptedBytes, byte[] key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                byte[] iv = new byte[16];
                Array.Copy(encryptedBytes, 0, iv, 0, 16);
                aesAlg.IV = iv;

                using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                using (MemoryStream memoryStream = new MemoryStream(encryptedBytes, 16, encryptedBytes.Length - 16))
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                using (StreamReader streamReader = new StreamReader(cryptoStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }
}
