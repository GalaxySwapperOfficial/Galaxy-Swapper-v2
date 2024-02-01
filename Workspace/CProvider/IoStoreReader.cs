using Galaxy_Swapper_v2.Workspace.Compression;
using Galaxy_Swapper_v2.Workspace.CProvider.Encryption;
using Galaxy_Swapper_v2.Workspace.CProvider.Objects;
using Galaxy_Swapper_v2.Workspace.Swapping.Compression.Types;
using Galaxy_Swapper_v2.Workspace.Swapping.Other;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Galaxy_Swapper_v2.Workspace.CProvider
{
    public class IoStoreReader
    {
        public FileInfo IoFileInfo;
        public FIoStoreTocHeader IoStoreTocHeader;
        public FIoStoreTocResource FIoStoreTocResource;
        public Dictionary<string, GameFile> Files;
        public DirectoryInfo PakDirectoryInfo;
        public DefaultFileProvider DefaultFileProvider;

        public string MountPoint;
        public Dictionary<int, string> Partitions;
        public IoStoreReader(DefaultFileProvider defaultfileprovider, FileInfo iofileInfo, DirectoryInfo pakDirectoryInfo)
        {
            IoFileInfo = iofileInfo;
            PakDirectoryInfo = pakDirectoryInfo;
            DefaultFileProvider = defaultfileprovider;
        }

        public bool Initialize()
        {
            var reader = new Reader(IoFileInfo.FullName);

            try
            {
                IoStoreTocHeader = new FIoStoreTocHeader(reader);
            }
            catch (Exception Exception)
            {
                Log.Error(Exception, $"Failed to initialize {IoFileInfo.Name} FIoStoreTocHeader");
                reader.Close();
                return false;
            }

            if (IoStoreTocHeader.CompressionMethodNameCount == 0)
            {
                Log.Warning($"{IoFileInfo.Name} CompressionMethodNameCount is 0 and will be skipped");
                reader.Close();
                return false;
            }

            try
            {
                FIoStoreTocResource = new FIoStoreTocResource(reader, IoStoreTocHeader);
            }
            catch (Exception Exception)
            {
                Log.Error(Exception, $"Failed to initialize {IoFileInfo.Name} IO header");
                reader.Close();
                return false;
            }

            if (FIoStoreTocResource.DirectoryIndexBuffer is null)
            {
                Log.Error($"{IoFileInfo.Name} DirectoryIndexBuffer is null");
                reader.Close();
                return false;
            }

            reader.Dispose();

            if (IoStoreTocHeader.ContainerFlags.HasFlag(EIoContainerFlags.Encrypted))
            {
                if (DefaultFileProvider.Keys.ContainsKey(IoStoreTocHeader.EncryptionKeyGuid))
                {
                    reader = new Reader(UnrealAes.Decrypt(FIoStoreTocResource.DirectoryIndexBuffer, DefaultFileProvider.Keys[IoStoreTocHeader.EncryptionKeyGuid]));
                }
                else
                {
                    Log.Warning($"Encryption key for {IoFileInfo.Name} has not been provided");
                    reader.Close();
                    return false;
                }
            }
            else
            {
                reader = new Reader(FIoStoreTocResource.DirectoryIndexBuffer);
            }

            if (!ReadMountPoint(reader))
            {
                reader.Close();
                return false;
            }

            var directoryEntries = reader.ReadArray<FIoDirectoryIndexEntry>();
            var fileEntries = reader.ReadArray<FIoFileIndexEntry>();
            var stringTable = reader.ReadArray(reader.ReadFString);
            Files = new Dictionary<string, GameFile>(fileEntries.Length, StringComparer.OrdinalIgnoreCase);

            ReadIndex(MountPoint, 0U, ref directoryEntries, ref fileEntries, ref stringTable);

            reader.Close();

            Partitions = new() { { 0, $"{IoFileInfo.Name.SubstringBefore('.')}" } };

            if (IoStoreTocHeader.PartitionCount > 1)
            {
                for (int i = 1; i <= IoStoreTocHeader.PartitionCount; i++)
                {
                    string partition = $"{IoFileInfo.Name.SubstringBefore('.')}_s{i}";

                    if (File.Exists($"{PakDirectoryInfo.FullName}\\{partition}.ucas"))
                    {
                        Partitions.Add(i, partition);
                    }
                    else break;
                }
            }

            Log.Information($"Successfully mounted {IoFileInfo.Name} with {IoStoreTocHeader.EntryCount} game files");

            return true;
        }

        private bool ReadMountPoint(Reader reader)
        {
            try
            {
                MountPoint = reader.ReadFString();
            }
            catch (Exception Exception)
            {
                Log.Warning(Exception, $"{IoFileInfo.Name} aes key is incorrect and can't mount");
                return false;
            }

            bool badMountPoint = !MountPoint.StartsWith("../../..") && !MountPoint.StartsWith(",./../..");


            MountPoint = MountPoint.SubstringAfter("../../..").SubstringAfter(",./../..");

            if (MountPoint[0] != '/' || ((MountPoint.Length > 1) && (MountPoint[1] == '.')))
                badMountPoint = true;

            if (badMountPoint)
            {
                Log.Warning($"{IoFileInfo.Name} has strange mount point {MountPoint}, mounting to root");
                MountPoint = "/";
            }

            MountPoint = MountPoint.Substring(1);
            return true;
        }

        private bool ReadIndex(string directoryname, uint dir, ref FIoDirectoryIndexEntry[] directoryEntries, ref FIoFileIndexEntry[] fileEntries, ref string[] stringTable)
        {
            const uint invalidHandle = uint.MaxValue;

            while (dir != invalidHandle)
            {
                ref var dirEntry = ref directoryEntries[dir];
                var subDirectoryName = dirEntry.Name == invalidHandle ? directoryname : $"{directoryname}{stringTable[dirEntry.Name]}/";
                var file = dirEntry.FirstFileEntry;
                while (file != invalidHandle)
                {
                    ref var fileEntry = ref fileEntries[file];

                    var path = string.Concat(subDirectoryName, stringTable[fileEntry.Name]);
                    var gamefile = new GameFile(path, fileEntry.UserData, FIoStoreTocResource.ChunkOffsetLengths[fileEntry.UserData], IoStoreTocHeader);

                    Files[path] = gamefile;
                    file = fileEntry.NextFileEntry;
                }

                ReadIndex(subDirectoryName, dirEntry.FirstChildEntry, ref directoryEntries, ref fileEntries, ref stringTable);
                dir = dirEntry.NextSiblingEntry;
            }
            return true;
        }

        public bool Export(GameFile gamefile, string path, long offset)
        {
            var compressionBlockSize = IoStoreTocHeader.CompressionBlockSize;
            var firstBlockIndex = (int)(offset / compressionBlockSize);
            var compressionBlock = FIoStoreTocResource.CompressionBlocks[firstBlockIndex];

            var partitionIndex = (int)((ulong)compressionBlock.Offset / IoStoreTocHeader.PartitionSize);
            var partitionOffset = (long)((ulong)compressionBlock.Offset % IoStoreTocHeader.PartitionSize);
            var rawsize = compressionBlock.CompressedSize.Align(UnrealAes.ALIGN);

            //Set our gamefile data for swapping
            gamefile.Offset = partitionOffset;
            gamefile.Ucas = Partitions[partitionIndex];
            gamefile.LastUcas = Partitions.Last().Value;
            gamefile.LastPartition = Partitions.Last().Key;
            gamefile.Utoc = IoFileInfo.Name.SubstringBefore('.');
            gamefile.CompressionBlock = compressionBlock;
            gamefile.ChunkId = FIoStoreTocResource.ChunkIds[gamefile.TocEntryIndex];

            var reader = new Reader($"{PakDirectoryInfo.FullName}\\{gamefile.Ucas}.ucas");

            byte[] compressedbuffer = new byte[rawsize];
            byte[] uncompressedbuffer = new byte[compressionBlock.UncompressedSize];

            reader.Position = gamefile.Offset;
            compressedbuffer = reader.ReadBytes((int)rawsize);

            reader.Close();

            if (IoStoreTocHeader.ContainerFlags.HasFlag(EIoContainerFlags.Encrypted))
            {
                gamefile.IsEncrypted = true;
                if (DefaultFileProvider.Keys.ContainsKey(IoStoreTocHeader.EncryptionKeyGuid))
                {
                    compressedbuffer = UnrealAes.Decrypt(compressedbuffer, DefaultFileProvider.Keys[IoStoreTocHeader.EncryptionKeyGuid]);
                }
            }

            if (compressionBlock.CompressionMethodIndex == 0)
            {
                uncompressedbuffer = compressedbuffer;
            }
            else
            {
                var compressionmethod = FIoStoreTocResource.CompressionMethods[compressionBlock.CompressionMethodIndex];

                try
                {
                    switch (compressionmethod)
                    {
                        case CompressionMethod.None:
                            uncompressedbuffer = compressedbuffer;
                            break;
                        case CompressionMethod.Oodle:
                            uncompressedbuffer = Oodle.Decompress(compressedbuffer, uncompressedbuffer.Length);
                            break;
                        case CompressionMethod.Zlib:
                            uncompressedbuffer = zlib.Decompress(compressedbuffer, uncompressedbuffer.Length);
                            break;
                        case CompressionMethod.Gzip:
                            uncompressedbuffer = gzip.Decompress(compressedbuffer);
                            break;
                    }
                }
                catch (Exception Exception)
                {
                    Log.Error(Exception, $"Failed to decompress {gamefile.Path} with compression method {compressionmethod}");
                    return false;
                }
            }

            gamefile.CompressedBuffer = compressedbuffer;
            gamefile.UncompressedBuffer = uncompressedbuffer;

            return true;
        }
    }
}