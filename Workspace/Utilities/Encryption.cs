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
            return uncompressedString;
        }

        public static string Decompress(this string compressedString)
        {
            return compressedString;
        }
    }
}