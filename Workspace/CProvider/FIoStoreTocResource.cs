using Galaxy_Swapper_v2.Workspace.CProvider.Objects;
using Galaxy_Swapper_v2.Workspace.Swapping.Other;
using Serilog;
using System;

namespace Galaxy_Swapper_v2.Workspace.CProvider
{
    public class FIoStoreTocResource
    {
        public readonly CompressionMethod[] CompressionMethods;
        public readonly byte[]? DirectoryIndexBuffer;
        public readonly FIoStoreTocCompressedBlockEntry[] CompressionBlocks;
        public readonly FIoOffsetAndLength[] ChunkOffsetLengths;
        public FIoStoreTocResource(Reader reader, FIoStoreTocHeader header)
        {
            //We don't need chunkids so skip var chunkIds = reader.ReadArray<FIoChunkId>((int)header.EntryCount);
            reader.Position += 12 * header.EntryCount;

            ChunkOffsetLengths = new FIoOffsetAndLength[header.EntryCount];
            for (int i = 0; i < header.EntryCount; i++)
            {
                ChunkOffsetLengths[i] = new FIoOffsetAndLength(reader);
            }

            uint perfectHashSeedsCount = 0;
            uint chunksWithoutPerfectHashCount = 0;
            if (header.Version >= EIoStoreTocVersion.PerfectHashWithOverflow)
            {
                perfectHashSeedsCount = header.ChunkPerfectHashSeedsCount;
                chunksWithoutPerfectHashCount = header.ChunksWithoutPerfectHashCount;
            }
            else if (header.Version >= EIoStoreTocVersion.PerfectHash)
            {
                perfectHashSeedsCount = header.ChunkPerfectHashSeedsCount;
            }

            if (perfectHashSeedsCount > 0)
            {
                //We don't need ChunkPerfectHashSeeds so skip ChunkPerfectHashSeeds = reader.ReadArray<int>((int)perfectHashSeedsCount);
                reader.Position += sizeof(int) * perfectHashSeedsCount;
            }
            if (chunksWithoutPerfectHashCount > 0)
            {
                //We don't need ChunkIndicesWithoutPerfectHash so skip ChunkIndicesWithoutPerfectHash = reader.ReadArray<int>((int)chunksWithoutPerfectHashCount);
                reader.Position += sizeof(int) * chunksWithoutPerfectHashCount;
            }

            CompressionBlocks = new FIoStoreTocCompressedBlockEntry[header.CompressedBlockEntryCount];
            for (int i = 0; i < header.CompressedBlockEntryCount; i++)
            {
                CompressionBlocks[i] = new FIoStoreTocCompressedBlockEntry(reader);
            }

            unsafe
            {
                var bufferSize = (int)(header.CompressionMethodNameLength * header.CompressionMethodNameCount);
                var buffer = stackalloc byte[bufferSize];
                reader.Serialize(buffer, bufferSize);
                CompressionMethods = new CompressionMethod[header.CompressionMethodNameCount + 1];
                CompressionMethods[0] = CompressionMethod.None;
                for (var i = 0; i < header.CompressionMethodNameCount; i++)
                {
                    var name = new string((sbyte*)buffer + i * header.CompressionMethodNameLength, 0, (int)header.CompressionMethodNameLength).TrimEnd('\0');
                    if (string.IsNullOrEmpty(name))
                        continue;
                    if (!Enum.TryParse(name, true, out CompressionMethod method))
                    {
                        Log.Warning($"Unknown compression method '{name}' in {reader.Name}");
                        method = CompressionMethod.Unknown;
                    }

                    CompressionMethods[i + 1] = method;
                }
            }

            if (header.ContainerFlags.HasFlag(EIoContainerFlags.Signed))
            {
                //FSHAHash.SIZE = 20
                int hashSize = reader.Read<int>();
                reader.Position += hashSize + hashSize + 20 * header.CompressedBlockEntryCount;
            }

            if (header.Version >= EIoStoreTocVersion.DirectoryIndex && header.ContainerFlags.HasFlag(EIoContainerFlags.Indexed) && header.DirectoryIndexSize > 0)
            {
                DirectoryIndexBuffer = reader.ReadBytes((int)header.DirectoryIndexSize);
            }

            /* We don't need ChunkMetas so skip
            ChunkMetas = new FIoStoreTocEntryMeta[header.EntryCount];
            for (int i = 0; i < header.EntryCount; i++)
            {
                ChunkMetas[i] = new FIoStoreTocEntryMeta(reader);
            }
            */

            reader.Position += 33 * header.EntryCount;
        }
    }
}