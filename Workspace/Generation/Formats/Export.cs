using CUE4Parse.UE4.IO.Objects;

namespace Galaxy_Swapper_v2.Workspace.Generation.Formats
{
    /// <summary>
    /// All the code below was provided from: https://github.com/GalaxySwapperOfficial/Galaxy-Swapper-v2
    /// You can also find us at https://galaxyswapperv2.com/Guilded
    /// </summary>
    public class Export
    {
        public byte[] Buffer { get; set; }
        public byte[] CompressedBuffer { get; set; }
        public string Ucas { get; set; }
        public string Utoc { get; set; }
        public FIoStoreTocCompressedBlockEntry CompressionBlock { get; set; }
        public FIoOffsetAndLength ChunkOffsetLengths { get; set; }
        public long Offset { get; set; }
        public uint LastPartition { get; set; }
    }
}