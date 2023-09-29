using CUE4Parse.Compression;

namespace Galaxy_Swapper_v2.Workspace.Swapping.Compression
{
    public static class Compression
    {
        public static byte[] Compress(byte[] buffer ,CompressionMethod method)
        {
            switch (method)
            {
                case CompressionMethod.Oodle:
                    return Types.Oodle.Compress(buffer, Types.Oodle.OodleCompressionLevel.Level5);
                default:
                    return buffer;
            }
        }

        public static byte[] Decompress(byte[] compressed, int decompressedsize, CompressionMethod method)
        {
            switch (method)
            {
                case CompressionMethod.Oodle:
                    return Types.Oodle.Decompress(compressed, decompressedsize);
                default:
                    return compressed;
            }
        }
    }
}
