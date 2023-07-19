using CUE4Parse.Compression;
using Serilog;

namespace Galaxy_Swapper_v2.Workspace.Swapping.Compression
{
    /// <summary>
    /// All the code below was provided from: https://github.com/GalaxySwapperOfficial/Galaxy-Swapper-v2
    /// You can also find us at https://galaxyswapperv2.com/Guilded
    /// </summary>
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
