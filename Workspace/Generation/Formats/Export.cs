using CUE4Parse.UE4.IO.Objects;

namespace Galaxy_Swapper_v2.Workspace.Generation.Formats
{
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