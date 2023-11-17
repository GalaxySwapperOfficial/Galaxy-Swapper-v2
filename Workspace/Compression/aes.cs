using System;
using System.IO;
using System.Security.Cryptography;

namespace Galaxy_Swapper_v2.Workspace.Compression
{
    public static class aes
    {
        public static byte[] Encrypt(string plainText, byte[] key)
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

        public static byte[] Decrypt(byte[] encryptedBytes, byte[] key)
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
                {
                    using (MemoryStream decryptedMemoryStream = new MemoryStream())
                    {
                        cryptoStream.CopyTo(decryptedMemoryStream);
                        return decryptedMemoryStream.ToArray();
                    }
                }
            }
        }
    }
}
