using Galaxy_Swapper_v2.Workspace.Swapping.Other;

namespace Galaxy_Swapper_v2.Workspace.CProvider.Objects
{
    public readonly struct FIoChunkId
    {
        public readonly ulong ChunkId;
        public readonly ushort _chunkIndex;
        public readonly byte _padding;
        public readonly byte ChunkType;
        public readonly long Position;
        public FIoChunkId(Reader reader)
        {
            Position = reader.Position;
            ChunkId = reader.Read<ulong>();
            _chunkIndex = reader.Read<ushort>();
            _padding = reader.Read<byte>();
            ChunkType = reader.Read<byte>();
        }
    }
}