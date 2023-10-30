using Ionic.Zlib;
using System.IO;

namespace Galaxy_Swapper_v2.Workspace.Compression
{
    public static class zlib
    {
        public static byte[] Compress(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (ZlibStream zls = new ZlibStream(ms, CompressionMode.Compress, CompressionLevel.Level9))
                {
                    zls.Write(buffer, 0, buffer.Length);
                }
                return ms.ToArray();
            }
        }

        public static byte[] Decompress(byte[] compressedData, int uncompressedsize)
        {
            byte[] uncompresed = new byte[uncompressedsize];
            using (var memoryStream = new MemoryStream(compressedData))
            {
                var zlib = new ZlibStream(memoryStream, CompressionMode.Decompress);
                zlib.Read(uncompresed, 0, uncompressedsize);
                zlib.Dispose();
            }

            return uncompresed;
        }
    }
}
