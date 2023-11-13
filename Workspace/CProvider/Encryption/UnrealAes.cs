using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using AesProvider = System.Security.Cryptography.Aes;

namespace Galaxy_Swapper_v2.Workspace.CProvider.Encryption
{
    public static class UnrealAes
    {
        public const int ALIGN = 16;
        public const int BLOCK_SIZE = 16 * 8;
        private static AesProvider Provider;

        public static byte[] Encrypt(byte[] plaintext, byte[] AESKey)
        {
            Provider = AesProvider.Create();
            Provider.Key = AESKey;
            Provider.Mode = CipherMode.ECB;
            Provider.Padding = PaddingMode.PKCS7;

            ICryptoTransform encryptor = Provider.CreateEncryptor(Provider.Key, Provider.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(plaintext, 0, plaintext.Length);
                }
                return msEncrypt.ToArray();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Decrypt(this byte[] encrypted, FAesKey key)
        {
            Provider = AesProvider.Create();
            Provider.Mode = CipherMode.ECB;
            Provider.Padding = PaddingMode.None;
            Provider.BlockSize = BLOCK_SIZE;

            return Provider.CreateDecryptor(key.Key, null).TransformFinalBlock(encrypted, 0, encrypted.Length);
        }
    }
}