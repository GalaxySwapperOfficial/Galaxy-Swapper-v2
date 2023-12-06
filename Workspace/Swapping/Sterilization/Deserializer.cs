using Galaxy_Swapper_v2.Workspace.CProvider.Objects;
using Galaxy_Swapper_v2.Workspace.Hashes;
using Galaxy_Swapper_v2.Workspace.Swapping.Other;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
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
        public byte[] Pad;
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
            LoadPad(reader);
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

        private void LoadPad(Reader reader)
        {
            var padSize = reader.Read<ulong>();
            Pad = reader.ReadBytes((int)padSize);
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

        public void ReplaceMaterialOverrideArray(byte[] searchBuffer, int offset, Dictionary<string, int> materials, int materialOverrideFlags = 31)
        {
            int arrayPos = offset;

            if (searchBuffer?.Length > 0)
            {
                arrayPos = RestOfData.IndexOfSequence(searchBuffer, 0);
                if (arrayPos < 0)
                {
                    Log.Error($"Failed to find 'MaterialOverrides' array");
                    return;
                }
            }

            var reader = new Reader(RestOfData, arrayPos);

            //Orignal 'MaterialOverrides' data
            int materialOverridesCount = reader.Read<int>();
            int materialOverridesSize = sizeof(int) + 3 + 20;

            if (materialOverridesCount > 1)
            {
                materialOverridesSize += 26 * (materialOverridesCount - 1);
            }

            byte[] materialOverridesBuffer = reader.ReadBytes(materialOverridesSize - sizeof(int));

            //26 is the size of the new array objects
            var writer = new Writer(new byte[materialOverridesSize + 26 * (materialOverridesCount + materials.Count)]);

            writer.Write<int>(materialOverridesCount + materials.Count);
            writer.WriteBytes(materialOverridesBuffer);

            //Key = /Game/Characters/Player/Female/Medium/Bodies/F_Med_Soldier_01/Skins/TV_20/Materials/F_MED_Commando_Body_TV20.F_MED_Commando_Body_TV20 Value = MaterialOverrideIndex
            foreach (var material in materials)
            {
                string entryPath = material.Key.SubstringBefore('.');
                string entryName = material.Key.SubstringAfter('.');

                writer.WriteBytes(new byte[] { 0, 5, (byte)material.Value, 0, 0, 0, (byte)Entires.Count(), 0, 0, 0, 0, 0, 0, 0, (byte)(Entires.Count() + 1), 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
                AddEntry(entryPath, entryName);
            }

            byte[] newMaterialOverridesBuffer = writer.ToByteArray(writer.Position);
            var restOfDataArray = new List<byte>(RestOfData);

            //Insert our new 'MaterialOverrides' array
            restOfDataArray.RemoveRange(arrayPos, materialOverridesSize);
            restOfDataArray.InsertRange(arrayPos, newMaterialOverridesBuffer);

            //Insert our new 'MaterialOverrideFlags'
            restOfDataArray.RemoveRange(arrayPos + newMaterialOverridesBuffer.Length, sizeof(int));
            restOfDataArray.InsertRange(arrayPos + newMaterialOverridesBuffer.Length, BitConverter.GetBytes(materialOverrideFlags));

            RestOfData = restOfDataArray.ToArray();
            ExportMap[^1].CookedSerialSize += (ulong)(newMaterialOverridesBuffer.Length - materialOverridesSize);
        }

        public void AddEntry(params string[] nameMaps)
        {
            Entires = Entires.Concat(nameMaps.Select(nameMap => new FNameEntrySerialized(nameMap))).ToArray();
            Hashes = Hashes.Concat(nameMaps.Select(nameMap => CityHash.Hash(Encoding.ASCII.GetBytes(nameMap.ToLower())))).ToArray();

            Adjust(nameMaps.Sum(nameMap => nameMap.Length) + 10 * nameMaps.Length);
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