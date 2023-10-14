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
        public byte[] Asset;
        public FZenPackageSummary Summary;
        public FBulkDataMapEntry[] BulkDataMap;
        public FExportMapEntry[] ExportMap;
        public FNameEntrySerialized[] Entires;
        public ulong[] Hashes;
        public ulong HashVersion;
        public byte[] RestOfData;
        public Deserializer(byte[] buffer) => Asset = buffer;

        public void Read()
        {
            var reader = new Reader(Asset);

            Summary = new FZenPackageSummary()
            {
                bHasVersioningInfo = reader.Read<uint>(),
                HeaderSize = reader.Read<uint>(),
                Name = reader.Read<FMappedName>(),
                PackageFlags = reader.Read<EPackageFlags>(),
                CookedHeaderSize = reader.Read<uint>(),
                ImportedPublicExportHashesOffset = reader.Read<int>(),
                ImportMapOffset = reader.Read<int>(),
                ExportMapOffset = reader.Read<int>(),
                ExportBundleEntriesOffset = reader.Read<int>(),
                DependencyBundleHeadersOffset = reader.Read<int>(),
                DependencyBundleEntriesOffset = reader.Read<int>(),
                ImportedPackageNamesOffset = reader.Read<int>()
            };

            LoadNameMaps(reader);
            LoadBulkDataMaps(reader);
            LoadExportMap(reader);

            reader.Position = Summary.ImportedPublicExportHashesOffset;
            RestOfData = reader.ReadBytes((int)(reader.Length - reader.Position));
        }

        private void LoadNameMaps(Reader reader)
        {
            int numStrings = reader.Read<int>();
            uint numBuffer = reader.Read<uint>();

            HashVersion = reader.Read<ulong>();
            Hashes = reader.ReadArray<ulong>(numStrings);

            var headers = reader.ReadArray<FSerializedNameHeader>(numStrings);
            Entires = new FNameEntrySerialized[numStrings];
            for (var i = 0; i < numStrings; i++)
            {
                var header = headers[i];
                var length = (int)header.Length;
                var s = header.IsUtf16 ? new string(reader.ReadArray<char>(length)) : Encoding.UTF8.GetString(reader.ReadBytes(length));
                Entires[i] = new FNameEntrySerialized(s);
            }
        }

        private void LoadExportMap(Reader reader)
        {
            ExportMap = new FExportMapEntry[(Summary.ExportBundleEntriesOffset - Summary.ExportMapOffset) / 72];
            reader.Position = Summary.ExportMapOffset;
            for (int i = 0; i < ExportMap.Length; i++)
            {
                long position = reader.Position;
                ExportMap[i].CookedSerialOffset = reader.Read<ulong>();
                ExportMap[i].CookedSerialSize = reader.Read<ulong>();
                ExportMap[i].ObjectName = reader.Read<FMappedName>();
                ExportMap[i].OuterIndex = reader.Read<FPackageObjectIndex>();
                ExportMap[i].ClassIndex = reader.Read<FPackageObjectIndex>();
                ExportMap[i].SuperIndex = reader.Read<FPackageObjectIndex>();
                ExportMap[i].TemplateIndex = reader.Read<FPackageObjectIndex>();
                ExportMap[i].PublicExportHash = reader.Read<ulong>();
                ExportMap[i].ObjectFlags = reader.Read<EObjectFlags>();
                ExportMap[i].FilterFlags = reader.Read<byte>();
                reader.Position = position + 72;
            }
        }

        private void LoadBulkDataMaps(Reader reader)
        {
            var bulkDataMapSize = reader.Read<UInt64>();

            BulkDataMap = new FBulkDataMapEntry[bulkDataMapSize / FBulkDataMapEntry.Size];

            for (int i = 0; i < BulkDataMap.Length; i++)
            {
                BulkDataMap[i].SerialOffset = reader.Read<UInt64>();
                BulkDataMap[i].DuplicateSerialOffset = reader.Read<UInt64>();
                BulkDataMap[i].SerialSize = reader.Read<UInt64>();
                BulkDataMap[i].Flags = reader.Read<uint>();
                BulkDataMap[i].Pad = reader.Read<uint>();
            }
        }

        public void ReplaceEntry(string search, string replace)
        {
            int index = Entires.ToList().FindIndex(entry => entry.Name.Equals(search, StringComparison.OrdinalIgnoreCase));
            if (index < 0)
            {
                Log.Error($"Failed to find index for: {search}");
                return;
            }
            if (replace.Length > 255)
            {
                Log.Error($"{replace} is longer then 255 chars");
                return;
            }

            Entires[index].Name = replace;
            Hashes[index] = CityHash.Hash(Encoding.ASCII.GetBytes(replace.ToLower()));

            Log.Information($"Replaced: {search} to {replace}");

            Adjust(replace.Length - search.Length);
        }

        public void ReplaceEntry(Deserializer deserializer, string search, string replace)
        {
            int index = Entires.ToList().FindIndex(entry => entry.Name.Equals(search, StringComparison.OrdinalIgnoreCase));
            int overrideindex = deserializer.Entires.ToList().FindIndex(entry => entry.Name.Equals(replace, StringComparison.OrdinalIgnoreCase));

            if (index < 0)
            {
                Log.Warning($"Failed to find index for: {search} attaching _C and trying again");
                search = $"{search}_C";
                index = Entires.ToList().FindIndex(entry => entry.Name.Equals(search, StringComparison.OrdinalIgnoreCase));

                if (index < 0)
                {
                    Log.Error($"Failed to find index for: {search}");
                    throw new Exception($"Failed to find index for: {search}");
                }
            }

            if (overrideindex < 0)
            {
                Log.Warning($"Failed to find index for: {replace} attaching _C and trying again");
                replace = $"{replace}_C";
                overrideindex = deserializer.Entires.ToList().FindIndex(entry => entry.Name.Equals(replace, StringComparison.OrdinalIgnoreCase));

                if (overrideindex < 0)
                {
                    Log.Error($"Failed to find index for: {replace}");
                    throw new Exception($"Failed to find index for: {replace}");
                }
            }

            Entires[index].Name = deserializer.Entires[overrideindex].Name;
            Hashes[index] = deserializer.Hashes[overrideindex];

            Log.Information($"Replaced: {search} to {replace} with hashes");

            Adjust(replace.Length - search.Length);
        }

        public void Adjust(int diff)
        {
            Summary.ImportMapOffset += diff;
            Summary.ExportMapOffset += diff;
            Summary.ExportBundleEntriesOffset += diff;
            Summary.DependencyBundleHeadersOffset += diff;
            Summary.DependencyBundleEntriesOffset += diff;
            Summary.ImportedPackageNamesOffset += diff;
            Summary.ImportedPublicExportHashesOffset += diff;
            Summary.HeaderSize += (uint)diff;
            Summary.CookedHeaderSize += (uint)diff;

            Log.Information($"Adjusting asset with for: {diff} difference");
        }
    }
}