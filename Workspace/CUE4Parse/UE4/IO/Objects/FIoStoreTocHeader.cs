using System;
using System.Linq;
using CUE4Parse.UE4.Exceptions;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Readers;
using CUE4Parse.Utils;
using Galaxy_Swapper_v2.Workspace.Swapping.Other;

namespace CUE4Parse.UE4.IO.Objects
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
        Compressed	= (1 << 0),
        Encrypted	= (1 << 1),
        Signed		= (1 << 2),
        Indexed		= (1 << 3),
    }

    public class FIoStoreTocHeader
    {
        public const int SIZE = 144;
        public static byte[] TOC_MAGIC = {0x2D, 0x3D, 0x3D, 0x2D, 0x2D, 0x3D, 0x3D, 0x2D, 0x2D, 0x3D, 0x3D, 0x2D, 0x2D, 0x3D, 0x3D, 0x2D}; // -==--==--==--==-

        public readonly byte[] TocMagic;
        public readonly EIoStoreTocVersion Version;
        private readonly byte _reserved0;
        private readonly ushort _reserved1;
        public readonly uint TocHeaderSize;
        public readonly uint TocEntryCount;
        public readonly uint TocCompressedBlockEntryCount;
        public readonly uint TocCompressedBlockEntrySize;	// For sanity checking
        public readonly uint CompressionMethodNameCount;
        public readonly uint CompressionMethodNameLength;
        public readonly uint CompressionBlockSize;
        public readonly uint DirectoryIndexSize;
        public uint PartitionCount;
        public readonly FIoContainerId ContainerId;
        public readonly FGuid EncryptionKeyGuid;
        public readonly EIoContainerFlags ContainerFlags;
        private readonly byte _reserved3;
        private readonly ushort _reserved4;
        public readonly uint TocChunkPerfectHashSeedsCount;
        public ulong PartitionSize;
        public readonly uint TocChunksWithoutPerfectHashCount;
        private readonly uint _reserved7;
        private readonly ulong[] _reserved8;

        public FIoStoreTocHeader(FArchive Ar)
        {
            TocMagic = Ar.ReadBytes(16);
            if (!TOC_MAGIC.SequenceEqual(TocMagic))
                throw new ParserException(Ar, "Invalid utoc magic");
            Version = Ar.Read<EIoStoreTocVersion>();
            _reserved0 = Ar.Read<byte>();
            _reserved1 = Ar.Read<ushort>();
            TocHeaderSize = Ar.Read<uint>();
            TocEntryCount = Ar.Read<uint>();
            TocCompressedBlockEntryCount = Ar.Read<uint>();
            TocCompressedBlockEntrySize = Ar.Read<uint>();
            CompressionMethodNameCount = Ar.Read<uint>();
            CompressionMethodNameLength = Ar.Read<uint>();
            CompressionBlockSize = Ar.Read<uint>();
            DirectoryIndexSize = Ar.Read<uint>();
            PartitionCount = Ar.Read<uint>();
            ContainerId = Ar.Read<FIoContainerId>();
            EncryptionKeyGuid = Ar.Read<FGuid>();
            ContainerFlags = Ar.Read<EIoContainerFlags>();
            _reserved3 = Ar.Read<byte>();
            _reserved4 = Ar.Read<ushort>();
            TocChunkPerfectHashSeedsCount = Ar.Read<uint>();
            PartitionSize = Ar.Read<ulong>();
            TocChunksWithoutPerfectHashCount = Ar.Read<uint>();
            _reserved7 = Ar.Read<uint>();
            _reserved8 = Ar.ReadArray<ulong>(5);
            Ar.Position = Ar.Position.Align(4);
        }

        public FIoStoreTocHeader(Reader reader)
        {
            TocMagic = reader.ReadBytes(16);

            if (!TOC_MAGIC.SequenceEqual(TocMagic))
                throw new Exception("Invalid utoc magic");

            Version = reader.Read<EIoStoreTocVersion>();
            _reserved0 = reader.Read<byte>();
            _reserved1 = reader.Read<ushort>();
            TocHeaderSize = reader.Read<uint>();
            TocEntryCount = reader.Read<uint>();
            TocCompressedBlockEntryCount = reader.Read<uint>();
            TocCompressedBlockEntrySize = reader.Read<uint>();
            CompressionMethodNameCount = reader.Read<uint>();
            CompressionMethodNameLength = reader.Read<uint>();
            CompressionBlockSize = reader.Read<uint>();
            DirectoryIndexSize = reader.Read<uint>();
            PartitionCount = reader.Read<uint>();
            ContainerId = reader.Read<FIoContainerId>();
            EncryptionKeyGuid = reader.Read<FGuid>();
            ContainerFlags = reader.Read<EIoContainerFlags>();
            _reserved3 = reader.Read<byte>();
            _reserved4 = reader.Read<ushort>();
            TocChunkPerfectHashSeedsCount = reader.Read<uint>();
            PartitionSize = reader.Read<ulong>();
            TocChunksWithoutPerfectHashCount = reader.Read<uint>();
            _reserved7 = reader.Read<uint>();
            _reserved8 = reader.ReadArray<ulong>(5);
        }

        public bool Compare(FIoStoreTocHeader header)
        {
            if (TocHeaderSize != header.TocHeaderSize)
                return false;
            if (TocEntryCount != header.TocEntryCount)
                return false;
            if (TocHeaderSize != header.TocHeaderSize)
                return false;
            if (TocCompressedBlockEntryCount != header.TocCompressedBlockEntryCount)
                return false;
            if (TocCompressedBlockEntrySize != header.TocCompressedBlockEntrySize)
                return false;
            if (CompressionMethodNameCount != header.CompressionMethodNameCount)
                return false;
            if (CompressionMethodNameLength != header.CompressionMethodNameLength)
                return false;
            if (CompressionBlockSize != header.CompressionBlockSize)
                return false;
            if (DirectoryIndexSize != header.DirectoryIndexSize)
                return false;
            if (TocChunkPerfectHashSeedsCount != header.TocChunkPerfectHashSeedsCount)
                return false;

            return true;
        }
    }
}