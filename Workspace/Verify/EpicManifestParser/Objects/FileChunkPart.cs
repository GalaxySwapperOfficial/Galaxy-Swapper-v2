using Galaxy_Swapper_v2.Workspace.Swapping.Other;
using GenericReader;
using System;
using System.Text.Json;

namespace Galaxy_Swapper_v2.Workspace.Verify.EpicManifestParser.Objects
{
    public class FileChunkPart
    {
        public string Guid { get; }
        public int Size { get; }
        public int Offset { get; }

        internal FileChunkPart(ref Utf8JsonReader reader)
        {
            Guid = null;
            Size = Offset = 0;

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                return;
            }

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    continue;
                }

                switch (reader.GetString())
                {
                    case "Guid":
                        {
                            reader.Read();
                            Guid = reader.GetString();
                            break;
                        }
                    case "Size":
                        {
                            reader.Read();
                            Size = Utilities.StringBlobTo<int>(reader.ValueSpan);
                            break;
                        }
                    case "Offset":
                        {
                            reader.Read();
                            Offset = Utilities.StringBlobTo<int>(reader.ValueSpan);
                            break;
                        }
                }
            }
        }

        internal FileChunkPart(IGenericReader reader)
        {
            reader.Position += 4;
            var hex = reader.ReadBytes(16);
            var guidA = BitConverter.ToUInt32(hex, 00);
            var guidB = BitConverter.ToUInt32(hex, 04);
            var guidC = BitConverter.ToUInt32(hex, 08);
            var guidD = BitConverter.ToUInt32(hex, 12);
            Guid = $"{guidA:X8}{guidB:X8}{guidC:X8}{guidD:X8}";
            Offset = reader.Read<int>();
            Size = reader.Read<int>();
        }

        public override string ToString()
        {
            return $"S:{Size}, O:{Offset} | {Guid}";
        }
    }
}