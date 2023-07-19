using CUE4Parse.UE4.IO.Objects;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.IO;

namespace Galaxy_Swapper_v2.Workspace.Properties
{
    /// <summary>
    /// All the code below was provided from: https://github.com/GalaxySwapperOfficial/Galaxy-Swapper-v2
    /// You can also find us at https://galaxyswapperv2.com/Guilded
    /// </summary>
    public static class SwapData
    {
        public static JArray Cache { get; set; } = default!;
        public static readonly string Path = $"{Config.Path}\\SwapData.json";
        public static void Initialize()
        {
            try
            {
                if (Cache != null)
                    return;

                if (!File.Exists(Path))
                {
                    Log.Information($"{Path} Does not exist populating cache with empty array");
                    Cache = new JArray();
                    return;
                }

                string Content = File.ReadAllText(Path);

                if (!Content.ValidArray())
                {
                    Log.Information($"{Path} Is not in a valid json format populating cache with empty array");
                    Cache = new JArray();
                    return;
                }

                Cache = JArray.Parse(Content);

                Log.Information("Successfully initialized swapdata");
            }
            catch (Exception Exception)
            {
                Log.Error(Exception.Message, $"Caught a error while initializing cache loading with empty array");
                Cache = new JArray();
            }
        }

        public static void Add(string ObjectPath, string FilePath, string Utoc, byte[] Buffer, long Offset, FIoStoreTocCompressedBlockEntry CompressedBlock, FIoOffsetAndLength OffsetAndLength)
        {
            var NewCache = new JArray();
            foreach (var Asset in Cache)
            {
                if (Asset["ObjectPath"].Value<string>() == ObjectPath)
                {
                    if (Asset["Offset"].Value<long>() == Offset && Asset["Path"].Value<string>() == FilePath)
                        return;
                    else
                        continue;
                }
                else
                    NewCache.Add(Asset);
            }

            var CompressedBlockObject = JObject.FromObject(new
            {
                Offset = CompressedBlock.Position,
                Buffer = Compression.Compress(CompressedBlock.Buffer),
                Path = Utoc
            });

            var OffsetAndLengthObject = JObject.FromObject(new
            {
                Offset = OffsetAndLength.Position,
                Buffer = Compression.Compress(OffsetAndLength.Buffer),
                Path = Utoc
            });

            var Object = JObject.FromObject(new
            {
                ObjectPath = ObjectPath,
                Buffer = Compression.Compress(Buffer),
                Offset = Offset,
                Path = FilePath,
                CompressionBlock = CompressedBlockObject,
                ChunkOffsetAndLengths = OffsetAndLengthObject
            });

            NewCache.Add(Object);
            Cache = NewCache;

            File.WriteAllText(Path, Cache.ToString());
        }

        public static void Read(string ObjectPath, out byte[] Buffer, out long Offset, out string FilePath, out byte[] CompressionBlockBuffer, out long CompressionBlockOffset, out string CompressionBlockPath, out byte[] ChunkOffsetAndLengthsBuffer, out long ChunkOffsetAndLengthsOffset, out string ChunkOffsetAndLengthsPath)
        {
            Buffer = null;
            Offset = 0;
            CompressionBlockBuffer = null;
            CompressionBlockOffset = 0;
            ChunkOffsetAndLengthsBuffer = null;
            ChunkOffsetAndLengthsOffset = 0;
            FilePath = null;
            CompressionBlockPath = null;
            ChunkOffsetAndLengthsPath = null;

            foreach (var Asset in Cache)
            {
                if (Asset["ObjectPath"].Value<string>() == ObjectPath)
                {
                    Buffer = Compression.Decompress(Asset["Buffer"].Value<string>());
                    Offset = Asset["Offset"].Value<long>();
                    FilePath = Asset["Path"].Value<string>();

                    if (!Asset["CompressionBlock"].KeyIsNullOrEmpty() && !Asset["ChunkOffsetAndLengths"].KeyIsNullOrEmpty())
                    {
                        CompressionBlockBuffer = Compression.Decompress(Asset["CompressionBlock"]["Buffer"].Value<string>());
                        CompressionBlockOffset = Asset["CompressionBlock"]["Offset"].Value<long>();
                        CompressionBlockPath = Asset["CompressionBlock"]["Path"].Value<string>();

                        ChunkOffsetAndLengthsBuffer = Compression.Decompress(Asset["ChunkOffsetAndLengths"]["Buffer"].Value<string>());
                        ChunkOffsetAndLengthsOffset = Asset["ChunkOffsetAndLengths"]["Offset"].Value<long>();
                        ChunkOffsetAndLengthsPath = Asset["ChunkOffsetAndLengths"]["Path"].Value<string>();
                    }
                }
            }

            File.WriteAllText(Path, Cache.ToString());
        }

        public static void Remove(string ObjectPath)
        {
            var NewCache = new JArray();
            foreach (var Asset in Cache)
            {
                if (Asset["ObjectPath"].Value<string>() != ObjectPath)
                    NewCache.Add(Asset);
            }

            Cache = NewCache;

            File.WriteAllText(Path, Cache.ToString());
        }

        public static void Delete()
        {
            var NewCache = new JArray();
            Cache = NewCache;

            File.WriteAllText(Path, Cache.ToString());
        }
    }
}
