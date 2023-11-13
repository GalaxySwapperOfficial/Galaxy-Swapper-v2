using System;
using System.Runtime.InteropServices;

namespace Galaxy_Swapper_v2.Workspace.CProvider.Objects
{
    public enum EIoChunkType : byte
    {
        Invalid,
        InstallManifest,
        ExportBundleData,
        BulkData,
        OptionalBulkData,
        MemoryMappedBulkData,
        LoaderGlobalMeta,
        LoaderInitialLoadMeta,
        LoaderGlobalNames,
        LoaderGlobalNameHashes,
        ContainerHeader
    }

    public enum EIoChunkType5 : byte
    {
        Invalid = 0,
        ExportBundleData = 1,
        BulkData = 2,
        OptionalBulkData = 3,
        MemoryMappedBulkData = 4,
        ScriptObjects = 5,
        ContainerHeader = 6,
        ExternalFile = 7,
        ShaderCodeLibrary = 8,
        ShaderCode = 9,
        PackageStoreEntry = 10,
        DerivedData = 11,
        EditorDerivedData = 12
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct FIoChunkId : IEquatable<FIoChunkId>
    {
        public readonly ulong ChunkId;
        private readonly ushort _chunkIndex;
        private readonly byte _padding;
        public readonly byte ChunkType;

        public FIoChunkId(ulong chunkId, ushort chunkIndex, byte chunkType)
        {
            ChunkId = chunkId;
            _chunkIndex = (ushort)((chunkIndex & 0xFF) << 8 | (chunkIndex & 0xFF00) >> 8); // NETWORK_ORDER16
            ChunkType = chunkType;
            _padding = 0;
        }

        public FIoChunkId(ulong chunkId, ushort chunkIndex, EIoChunkType chunkType) : this(chunkId, chunkIndex, (byte)chunkType) { }
        public FIoChunkId(ulong chunkId, ushort chunkIndex, EIoChunkType5 chunkType) : this(chunkId, chunkIndex, (byte)chunkType) { }

        public static bool operator ==(FIoChunkId left, FIoChunkId right) => left.Equals(right);

        public static bool operator !=(FIoChunkId left, FIoChunkId right) => !left.Equals(right);

        public bool Equals(FIoChunkId other) => ChunkId == other.ChunkId && ChunkType == other.ChunkType;

        public override bool Equals(object? obj) => obj is FIoChunkId other && Equals(other);

        public override string ToString() => $"0x{ChunkId:X8} | {ChunkType}";
    }
}