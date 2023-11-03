using System.Collections.Generic;
using System.Text.Json;

namespace Galaxy_Swapper_v2.Workspace.Verify.EpicManifestParser.Objects
{
    public class FileManifest
    {
        private readonly Manifest _manifest;
        public string Name { get; }
        public string Hash { get; internal set; }
        public List<FileChunkPart> ChunkParts { get; internal set; }
        public List<string> InstallTags { get; internal set; }

        internal FileManifest(Manifest manifest, string name = null)
        {
            _manifest = manifest;
            Name = name;
            Hash = null;
            ChunkParts = null;
            InstallTags = null;
        }

        internal FileManifest(ref Utf8JsonReader reader, Manifest manifest) : this(manifest)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                return;
            }

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    reader.Skip();
                    continue;
                }

                switch (reader.GetString())
                {
                    case "Filename":
                        {
                            reader.Read();
                            Name = reader.GetString();
                            break;
                        }
                    case "FileHash":
                        {
                            reader.Read();
                            Hash = Utilities.StringBlobToHexString(reader.ValueSpan);
                            break;
                        }
                    case "FileChunkParts":
                        {
                            reader.Read();

                            if (reader.TokenType != JsonTokenType.StartArray)
                            {
                                break;
                            }

                            ChunkParts = new List<FileChunkPart>();
                            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                            {
                                ChunkParts.Add(new FileChunkPart(ref reader));
                            }

                            break;
                        }
                    case "InstallTags":
                        {
                            reader.Read();

                            if (reader.TokenType != JsonTokenType.StartArray)
                            {
                                break;
                            }

                            InstallTags = new List<string>(1); // wasn't ever bigger than 1 ¯\_(ツ)_/¯
                            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                            {
                                InstallTags.Add(reader.GetString());
                            }

                            break;
                        }
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}