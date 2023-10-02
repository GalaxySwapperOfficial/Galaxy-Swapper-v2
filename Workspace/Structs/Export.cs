using CUE4Parse.UE4.IO.Objects;

namespace Galaxy_Swapper_v2.Workspace.Structs
{
    public class Export
    {
        public byte[] Buffer { get; set; }
        public FIoStoreTocCompressedBlockEntry CompressionBlock { get; set; }
        public FIoOffsetAndLength ChunkOffsetLengths { get; set; }
        public string Ucas { get; set; }
        public string Utoc { get; set; }
        public uint LastPartition { get; set; }
    }
}
