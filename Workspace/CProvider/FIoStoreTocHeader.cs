using Galaxy_Swapper_v2.Workspace.CProvider.Objects;
using Galaxy_Swapper_v2.Workspace.Swapping.Other;

namespace Galaxy_Swapper_v2.Workspace.CProvider
{
    public enum EIoStoreTocVersion : byte
    {
        Invalid = 0,
        Initial,
        DirectoryIndex,
        PartitionSize,
        PerfectHash,
        PerfectHashWithOverflow,
        LatestPlusOne,
        Latest = LatestPlusOne - 1
    }

    public enum EIoContainerFlags : byte
    {
        None,
        Compressed = (1 << 0),
        Encrypted = (1 << 1),
        Signed = (1 << 2),
        Indexed = (1 << 3),
    }

    public class FIoStoreTocHeader
    {
        private readonly byte[] Magic = { 0x2D, 0x3D, 0x3D, 0x2D, 0x2D, 0x3D, 0x3D, 0x2D, 0x2D, 0x3D, 0x3D, 0x2D, 0x2D, 0x3D, 0x3D, 0x2D };

        public readonly byte[] HeaderMagic;
        public readonly EIoStoreTocVersion Version;
        public readonly uint HeaderSize;
        public readonly uint EntryCount;
        public readonly uint CompressedBlockEntryCount;
        public readonly uint CompressedBlockEntrySize;
        public readonly uint CompressionMethodNameCount;
        public readonly uint CompressionMethodNameLength;
        public readonly uint CompressionBlockSize;
        public readonly uint DirectoryIndexSize;
        public readonly uint PartitionCount;
        public readonly FIoContainerId ContainerId;
        public readonly FGuid EncryptionKeyGuid;
        public readonly EIoContainerFlags ContainerFlags;
        public readonly uint ChunkPerfectHashSeedsCount;
        public readonly ulong PartitionSize;
        public readonly uint ChunksWithoutPerfectHashCount;
        public FIoStoreTocHeader(Reader reader)
        {
            HeaderMagic = reader.ReadBytes(Magic.Length);
            Version = reader.Read<EIoStoreTocVersion>();

            //_reserved0 _reserved1 not needed so skip
            reader.Position += sizeof(byte);
            reader.Position += sizeof(ushort);

            HeaderSize = reader.Read<uint>();
            EntryCount = reader.Read<uint>();
            CompressedBlockEntryCount = reader.Read<uint>();
            CompressedBlockEntrySize = reader.Read<uint>();
            CompressionMethodNameCount = reader.Read<uint>();
            CompressionMethodNameLength = reader.Read<uint>();
            CompressionBlockSize = reader.Read<uint>();
            DirectoryIndexSize = reader.Read<uint>();
            PartitionCount = reader.Read<uint>();
            ContainerId = reader.Read<FIoContainerId>();
            EncryptionKeyGuid = reader.Read<FGuid>();
            ContainerFlags = reader.Read<EIoContainerFlags>();

            //_reserved3 _reserved4 not needed so skip
            reader.Position += sizeof(byte);
            reader.Position += sizeof(ushort);

            ChunkPerfectHashSeedsCount = reader.Read<uint>();
            PartitionSize = reader.Read<ulong>();
            ChunksWithoutPerfectHashCount = reader.Read<uint>();

            //_reserved7 _reserved8 not needed so skip
            reader.Position += sizeof(uint);
            reader.Position += 5 * sizeof(ulong);
        }
    }
}