using CUE4Parse.UE4.IO.Objects;
using Galaxy_Swapper_v2.Workspace.Swapping.Other;
using System.Linq;
using System.Text;

namespace Galaxy_Swapper_v2.Workspace.Swapping.Sterilization
{
    public class Serializer
    {
        private Deserializer Deserializer { get; set; }
        public Serializer(Deserializer deserializer)
        {
            Deserializer = deserializer;
        }

        public byte[] Serialize()
        {
            Writer writer = new Writer(Enumerable.Repeat((byte)0, Deserializer.RestOfData.Length + 65536).ToArray());
            writer.Write(Deserializer.Summary);

            SerializeNameBatch(writer, Deserializer.NameMap);

            if (Deserializer.ExtraOfData != null)
                writer.WriteBytes(Deserializer.ExtraOfData);

            Deserializer.Summary.ImportedPublicExportHashesOffset = (int)writer.Position;
            writer.WriteBytes(Deserializer.RestOfData);

            long position = writer.Position;
            int diff = (int)(position - Deserializer.Size);

            Deserializer.Summary.ImportMapOffset += diff;
            Deserializer.Summary.ExportMapOffset += diff;
            Deserializer.Summary.ExportBundleEntriesOffset += diff;
            Deserializer.Summary.DependencyBundleHeadersOffset += diff;
            Deserializer.Summary.DependencyBundleEntriesOffset += diff;
            Deserializer.Summary.ImportedPackageNamesOffset += diff;

            writer.Position = Deserializer.Summary.ExportMapOffset;
            SerializeExportMap(writer);

            writer.Position = 0L;

            Deserializer.Summary.HeaderSize += (uint)diff;
            Deserializer.Summary.CookedHeaderSize += (uint)diff;

            writer.Write(Deserializer.Summary);

            return writer.ToByteArray(position);
        }


        private void SerializeExportMap(Writer Ar)
        {
            FExportMapEntry[] ExportMap = Deserializer.ExportMap;
            for (int i = 0; i < ExportMap.Length; i++)
            {
                FExportMapEntry fExportMapEntry = ExportMap[i];

                Ar.Write(fExportMapEntry.CookedSerialOffset);
                Ar.Write(fExportMapEntry.CookedSerialSize);
                Ar.Write(fExportMapEntry.ObjectName._nameIndex);
                Ar.Write(fExportMapEntry.ObjectName.ExtraIndex);
                Ar.Write(fExportMapEntry.OuterIndex);
                Ar.Write(fExportMapEntry.ClassIndex);
                Ar.Write(fExportMapEntry.SuperIndex);
                Ar.Write(fExportMapEntry.TemplateIndex);
                Ar.Write(fExportMapEntry.PublicExportHash);
                Ar.Write(fExportMapEntry.ObjectFlags);
                Ar.WriteBytes(new byte[] { fExportMapEntry.FilterFlags, 0, 0, 0 });
            }
        }

        public void SerializeNameBatch(Writer Ar, string[] names)
        {
            Ar.Write(names.Length); //Amount of strings

            if (names.Length != 0)
            {
                Ar.Write(names.Sum(x => x.Length)); //Lengths of strings
                Ar.Write(Deserializer.HashVersion); //Hash version (It's always null but yk)

                foreach (string text in names) //Write all string hashes
                {
                    Ar.Write(CityHash.Hash(Encoding.ASCII.GetBytes(text.ToLower())));
                }

                foreach (string text in names) //Write each strings length
                {
                    Ar.WriteBytes(new byte[] { 0, (byte)text.Length });
                }

                foreach (string text in names) //Write the string itself
                {
                    Ar.WriteBytes(Encoding.ASCII.GetBytes(text));
                }
            }
        }
    }
}