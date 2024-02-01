namespace Galaxy_Swapper_v2.Workspace.CProvider.Objects
{
    public class GameFile
    {
        public readonly string Path;
        public readonly long Size;
        public readonly uint TocEntryIndex;
        public readonly FIoOffsetAndLength ChunkOffsetLengths;
        public readonly FIoStoreTocHeader IoStoreTocHeader;
        public FIoStoreTocCompressedBlockEntry CompressionBlock;
        public FIoChunkId ChunkId;
        public string Ucas;
        public string LastUcas;
        public string Utoc;
        public int LastPartition;
        public long Offset;
        public byte[] CompressedBuffer;
        public byte[] UncompressedBuffer;
        public bool IsEncrypted;
        public GameFile(string path, uint tocentryindex, FIoOffsetAndLength chunkOffsetLengths, FIoStoreTocHeader ioStoreTocHeader)
        {
            Path = path;
            TocEntryIndex = tocentryindex;
            ChunkOffsetLengths = chunkOffsetLengths;
            Size = (long)ChunkOffsetLengths.Length;
            IoStoreTocHeader = ioStoreTocHeader;
        }

        public GameFile()
        {

        }
    }
}