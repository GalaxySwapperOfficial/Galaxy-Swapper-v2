using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.IO.Objects;
using CUE4Parse.UE4.Objects.UObject;
using Galaxy_Swapper_v2.Workspace.Swapping.Other;
using Serilog;
using System;
using System.Linq;
using System.Text;

namespace Galaxy_Swapper_v2.Workspace.Swapping.Sterilization
{
    public class Deserializer
    {
        public Structs.FZenPackageSummary Summary;
        public FPackageObjectIndex[] ImportMap;
        public FExportMapEntry[] ExportMap;
        public string[] NameMap;
        public ulong[] NameMapHashes;
        public byte[] RestOfData;
        public int ExtraOfDataSize = 0;
        public byte[] ExtraOfData;
        public int Size;
        public ulong HashVersion = 0;

        public Deserializer(int size)
        {
            Size = size;
        }

        public void Deserialize(byte[] Asset)
        {
            Reader Reader = new Reader(Asset);
            Reader.Position = 0L;

            Summary = Reader.Read<Structs.FZenPackageSummary>();
            NameMap = DeserializeNameBatch(Reader);

            if (Summary.ImportedPublicExportHashesOffset - Reader.Position > 0)
            {
                ExtraOfDataSize = (int)(Summary.ImportedPublicExportHashesOffset - Reader.Position);
                ExtraOfData = Reader.ReadBytes(ExtraOfDataSize);
            }

            Reader.Position = Summary.ImportedPublicExportHashesOffset;
            RestOfData = Reader.ReadBytes((int)(Reader.Length - Reader.Position));

            Reader.Position = Summary.ImportMapOffset;
            ImportMap = Reader.ReadArray<FPackageObjectIndex>((Summary.ExportMapOffset - Summary.ImportMapOffset) / 8);

            Reader.Position = Summary.ExportMapOffset;
            ExportMap = new FExportMapEntry[(Summary.ExportBundleEntriesOffset - Summary.ExportMapOffset) / 72];

            DeserializeExportMap(Reader);
        }

        public string[] DeserializeNameBatch(Reader Reader)
        {
            ulong[] hashes;
            int count = Reader.Read<int>();

            if (count == 0)
            {
                hashes = Array.Empty<ulong>();
                return Array.Empty<string>();
            }

            Reader.Position += sizeof(uint); // numStringBytes
            HashVersion = Reader.Read<ulong>(); // hashVersion

            hashes = Reader.ReadArray<ulong>(count);

            uint[] array = new uint[count];
            string[] array2 = new string[count];

            array = (from x in Reader.ReadArray<FSerializedNameHeader>(count) select x.Length).ToArray();

            for (int i = 0; i < array2.Length; i++)
            {
                byte[] array3 = Reader.ReadBytes((int)array[i]);
                array2[i] = Encoding.ASCII.GetString(array3);
            }

            NameMapHashes = hashes;
            ExtraOfDataSize = (int)Reader.Position;

            return array2;
        }

        public void DeserializeExportMap(Reader Reader)
        {
            for (int i = 0; i < ExportMap.Length; i++)
            {
                long position = Reader.Position;
                ExportMap[i].CookedSerialOffset = Reader.Read<ulong>();
                ExportMap[i].CookedSerialSize = Reader.Read<ulong>();
                ExportMap[i].ObjectName = Reader.Read<FMappedName>();
                ExportMap[i].OuterIndex = Reader.Read<FPackageObjectIndex>();
                ExportMap[i].ClassIndex = Reader.Read<FPackageObjectIndex>();
                ExportMap[i].SuperIndex = Reader.Read<FPackageObjectIndex>();
                ExportMap[i].TemplateIndex = Reader.Read<FPackageObjectIndex>();
                ExportMap[i].PublicExportHash = Reader.Read<ulong>();
                ExportMap[i].ObjectFlags = Reader.Read<EObjectFlags>();
                ExportMap[i].FilterFlags = Reader.Read<byte>();
                Reader.Position = position + 72;
            }
        }

        public void ReplaceNameMap(string Search, string Replace)
        {
            try
            {
                int num = NameMap.ToList().FindIndex((string x) => x.ToLower() == Search.ToLower());
                if (!NameMap.ToList().Contains(Search))
                {
                    Log.Warning($"Failed to replace {Search} to {Replace}");
                    return;
                }
                if (num < 0)
                {
                    Log.Warning($"Failed to replace {Search} to {Replace}");
                    return;
                }
                if (Replace.Length > 255)
                {
                    Log.Error($"{Replace} is longer then 255 chars");
                    return;
                }

                NameMap[num] = Replace;

                Log.Information($"Replaced {Search} to {Replace} namemap");
            }
            catch (Exception Exception)
            {
                Log.Error(Exception, $"Failed to replace {Search} to {Replace} namemap");
            }
        }

        public void ReplaceNameMapAndHashes(Deserializer AssetDeserializer, string Search, string Replace)
        {
            try
            {
                int SearchIndex = this.NameMap.ToList().FindIndex((string x) => x.ToLower() == Search.ToLower());

                if (SearchIndex < 0)
                {
                    Log.Error($"Failed to find {Search} namemap attempting _C ?");

                    SearchIndex = this.NameMap.ToList().FindIndex((string x) => x.ToLower() == $"{Search.ToLower()}_c");
                    Search = $"{Search}_C";

                    if (SearchIndex < 0)
                    {
                        Log.Error($"Failed to find {Search} namemap even with _C");
                        throw new Exception($"Failed to find {Search} namemap!");
                    }
                }

                int ReplaceIndex = AssetDeserializer.NameMap.ToList().FindIndex((string x) => x.ToLower() == Replace.ToLower());
                if (ReplaceIndex < 0)
                {
                    Log.Error($"Failed to find {Replace} namemap attempting _C ?");

                    ReplaceIndex = AssetDeserializer.NameMap.ToList().FindIndex((string x) => x.ToLower() == $"{Replace.ToLower()}_c");
                    Replace = $"{Replace}_C";

                    if (ReplaceIndex < 0)
                    {
                        Log.Error($"Failed to find {Replace} namemap even with _C");
                        throw new Exception($"Failed to find {Replace} namemap!");
                    }
                }

                this.NameMapHashes[SearchIndex] = AssetDeserializer.NameMapHashes[ReplaceIndex];
                this.NameMap[SearchIndex] = AssetDeserializer.NameMap[ReplaceIndex];

                Log.Information($"Replaced {Search} to {Replace} namemap and hashes");
            }
            catch (Exception Exception)
            {
                Log.Error(Exception, $"Failed to replace {Search} to {Replace} namemap and hashes");
                throw new Exception($"Failed to replace {Search} To {Replace} namemap and hashes");
            }
        }
    }
}