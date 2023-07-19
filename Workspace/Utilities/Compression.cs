using System;
using System.IO;
using System.IO.Compression;

namespace Galaxy_Swapper_v2.Workspace.Utilities
{
    public static class Compression
    {
        public static string Compress(byte[] Buffer)
        {
            byte[] CompressedBuffer;
            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                {
                    gzipStream.Write(Buffer, 0, Buffer.Length);
                }
                CompressedBuffer = memoryStream.ToArray();
            }
            return Convert.ToBase64String(CompressedBuffer);
        }

        public static byte[] Decompress(string Base64)
        {
            byte[] CompressedBuffer = Convert.FromBase64String(Base64);
            byte[] DecompressedBuffer;
            using (var memoryStream = new MemoryStream(CompressedBuffer))
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    using (var output = new MemoryStream())
                    {
                        gzipStream.CopyTo(output);
                        DecompressedBuffer = output.ToArray();
                    }
                }
            }
            return DecompressedBuffer;
        }
    }
}