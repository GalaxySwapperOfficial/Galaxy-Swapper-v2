using System;
using System.IO;
using System.IO.Compression;

namespace Galaxy_Swapper_v2.Workspace.Utilities
{
    public static class Compression
    {
        public static string Compress(byte[] Buffer)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                {
                    gzipStream.Write(Buffer, 0, Buffer.Length);
                }
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        public static byte[] Decompress(string Base64)
        {
            byte[] CompressedBuffer = Convert.FromBase64String(Base64);
            using (var memoryStream = new MemoryStream(CompressedBuffer))
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    using (var output = new MemoryStream())
                    {
                        gzipStream.CopyTo(output);
                        return output.ToArray();
                    }
                }
            }
        }
    }
}
