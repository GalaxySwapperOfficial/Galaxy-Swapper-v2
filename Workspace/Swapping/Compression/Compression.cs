using CUE4Parse.Compression;
using Serilog;

namespace Galaxy_Swapper_v2.Workspace.Swapping.Compression
{
    public static class Compression
    {
        public static byte[] Decompress(byte[] Compressed, int UncompressedLength, CompressionMethod method)
        {
            switch (method)
            {
                case CompressionMethod.Oodle:
                    return Types.Oodle.Decompress(Compressed, UncompressedLength);
                default:
                    return Compressed;
            }
        }
    }
}
