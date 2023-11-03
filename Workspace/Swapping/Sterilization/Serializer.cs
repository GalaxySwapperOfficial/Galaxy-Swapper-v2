using Galaxy_Swapper_v2.Workspace.CProvider.Objects;
using Galaxy_Swapper_v2.Workspace.Swapping.Other;
using System.Linq;
using System.Text;

namespace Galaxy_Swapper_v2.Workspace.Swapping.Sterilization
{
    public class Serializer
    {
        public Deserializer Deserializer;
        public Serializer(Deserializer deserializer) => Deserializer = deserializer;

        public byte[] Write()
        {
            var writer = new Writer(Enumerable.Repeat((byte)0, Deserializer.RestOfData.Length + 65536).ToArray());

            writer.Write(Deserializer.Summary.bHasVersioningInfo);
            writer.Write(Deserializer.Summary.HeaderSize);
            writer.Write(Deserializer.Summary.Name);
            writer.Write(Deserializer.Summary.PackageFlags);
            writer.Write(Deserializer.Summary.CookedHeaderSize);
            writer.Write(Deserializer.Summary.ImportedPublicExportHashesOffset);
            writer.Write(Deserializer.Summary.ImportMapOffset);
            writer.Write(Deserializer.Summary.ExportMapOffset);
            writer.Write(Deserializer.Summary.ExportBundleEntriesOffset);
            writer.Write(Deserializer.Summary.DependencyBundleHeadersOffset);
            writer.Write(Deserializer.Summary.DependencyBundleEntriesOffset);
            writer.Write(Deserializer.Summary.ImportedPackageNamesOffset);

            WriteNameMaps(writer);
            WritePad(writer);
            WriteBulkData(writer);

            writer.WriteBytes(Deserializer.RestOfData);
            long finalsize = writer.Position;

            WriteExportMap(writer);

            return writer.ToByteArray(finalsize);
        }

        public void WriteNameMaps(Writer writer)
        {
            writer.Write(Deserializer.Entires.Length);

            if (Deserializer.Entires.Length == 0)
                return;

            writer.Write(Deserializer.Entires.Sum(x => x.Name.Length));
            writer.Write(Deserializer.HashVersion);

            foreach (ulong hash in Deserializer.Hashes)
            {
                writer.Write(hash);
            }

            foreach (var entry in Deserializer.Entires)
            {
                writer.WriteBytes(new byte[] { 0, (byte)entry.Name.Length });
            }

            foreach (var entry in Deserializer.Entires)
            {
                writer.WriteBytes(Encoding.ASCII.GetBytes(entry.Name));
            }
        }

        public void WritePad(Writer writer)
        {
            writer.Write<ulong>((ulong)Deserializer.Pad.Length);
            writer.WriteBytes(Deserializer.Pad);
        }

        public void WriteBulkData(Writer writer)
        {
            writer.Write((ulong)Deserializer.BulkDataMap.Length * FBulkDataMapEntry.Size);
            foreach (var bulk in Deserializer.BulkDataMap)
            {
                writer.Write(bulk.SerialOffset);
                writer.Write(bulk.DuplicateSerialOffset);
                writer.Write(bulk.SerialSize);
                writer.Write(bulk.Flags);
                writer.Write(bulk.Pad);
            }
        }

        public void WriteExportMap(Writer writer)
        {
            writer.Position = Deserializer.Summary.ExportMapOffset;

            FExportMapEntry[] ExportMap = Deserializer.ExportMap;
            for (int i = 0; i < ExportMap.Length; i++)
            {
                FExportMapEntry fExportMapEntry = ExportMap[i];

                writer.Write(fExportMapEntry.CookedSerialOffset);
                writer.Write(fExportMapEntry.CookedSerialSize);
                writer.Write(fExportMapEntry.ObjectName._nameIndex);
                writer.Write(fExportMapEntry.ObjectName.ExtraIndex);
                writer.Write(fExportMapEntry.OuterIndex);
                writer.Write(fExportMapEntry.ClassIndex);
                writer.Write(fExportMapEntry.SuperIndex);
                writer.Write(fExportMapEntry.TemplateIndex);
                writer.Write(fExportMapEntry.PublicExportHash);
                writer.Write(fExportMapEntry.ObjectFlags);
                writer.WriteBytes(new byte[] { fExportMapEntry.FilterFlags, 0, 0, 0 });
            }
        }
    }
}