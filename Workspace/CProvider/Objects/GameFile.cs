namespace Galaxy_Swapper_v2.Workspace.CProvider.Objects
{
    public class GameFile
    {
        public readonly string Path;
        public readonly long Size;
        public readonly uint TocEntryIndex;
        public readonly FIoOffsetAndLength ChunkOffsetLengths;


        public FIoStoreTocCompressedBlockEntry CompressionBlock;
        public string Ucas;
        public string LastUcas;
        public string Utoc;
        public long Offset;
        public byte[] CompressedBuffer;
        public byte[] UncompressedBuffer;
        public bool IsEncrypted;
        public GameFile(string path, uint tocentryindex, FIoOffsetAndLength chunkOffsetLengths)
        {
            Path = path;
            TocEntryIndex = tocentryindex;
            ChunkOffsetLengths = chunkOffsetLengths;
            Size = (long)ChunkOffsetLengths.Length;
        }

        public GameFile()
        {

        }
    }
}