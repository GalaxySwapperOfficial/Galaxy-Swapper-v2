using Galaxy_Swapper_v2.Workspace.Swapping.Other;
using Galaxy_Swapper_v2.Workspace.Utilities;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Serilog;
using System.IO;

namespace Galaxy_Swapper_v2.Workspace.ClientSettings
{
    public class ClientSettings
    {
        public byte[] Buffer = null!;

        public ClientSettings(byte[] buffer)
        {
            Buffer = buffer;
        }

        public uint Magic;
        public int Engine;

        public bool Deserialize()
        {
            Log.Information("Deserializing ClientSettings buffer");

            var reader = new Reader(Buffer);

            Magic = reader.Read<uint>();
            Engine = reader.Read<int>();

            var isCompressed = reader.ReadBoolean(true);

            Log.Information("ClientSettings is compressed: {0}", isCompressed);

            if (isCompressed)
            {
                var decompressedSize = reader.Read<int>();
                var compressedSize = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
                byte[] compressedBuffer = reader.ReadBytes(compressedSize);

                Log.Information("Decompressing ClientSettings buffer");

                Buffer = Decompress(compressedBuffer, decompressedSize);
            }
            else
            {
                Buffer = reader.ReadBytes((int)(reader.BaseStream.Length - reader.Position));
            }

            //I will write the full deserilize when I'm back from vacation but due to me being on a time contrant it will be left as this.

            return true;
        }

        public bool Serialize(out byte[] serialized)
        {
            //I will write the full Serialize when I'm back from vacation but due to me being on a time contrant it will be left as this.

            serialized = Buffer;
            return false;
        }

        public bool ModifyFov(int fov)
        {
            byte[] minFov = new byte[] { 0x46, 0x4F, 0x56, 0x4D, 0x69, 0x6E, 0x69, 0x6D, 0x75, 0x6D, 0x00, 0x0E, 0x00, 0x00, 0x00, 0x46, 0x6C, 0x6F, 0x61, 0x74, 0x50, 0x72, 0x6F, 0x70, 0x65, 0x72, 0x74, 0x79, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] maxFov = new byte[] { 0x46, 0x4F, 0x56, 0x4D, 0x61, 0x78, 0x69, 0x6D, 0x75, 0x6D, 0x00, 0x0E, 0x00, 0x00, 0x00, 0x46, 0x6C, 0x6F, 0x61, 0x74, 0x50, 0x72, 0x6F, 0x70, 0x65, 0x72, 0x74, 0x79, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            int minFovPos = Buffer.IndexOfSequence(minFov, 0);
            int maxFovPos = Buffer.IndexOfSequence(maxFov, 0);

            if (minFovPos < 0 || maxFovPos < 0)
            {
                Log.Error("Failed to find fov pos");
                return false;
            }

            var writer = new Writer(Buffer);

            writer.Position = minFovPos;
            writer.Write<float>(fov - 1);

            writer.Position = maxFovPos;
            writer.Write<float>(fov);

            Buffer = writer.ToByteArray(writer.BaseStream.Length);

            return true;
        }

        #region Compression
        private static byte[] Compress(byte[] inputData)
        {
            using (var compressedStream = new MemoryStream())
            {
                using (var deflaterStream = new DeflaterOutputStream(compressedStream))
                {
                    deflaterStream.Write(inputData, 0, inputData.Length);
                }

                return compressedStream.ToArray();
            }
        }
        private static byte[] Decompress(byte[] compressedData, int decompressedSize)
        {
            using (var decompressedStream = new MemoryStream(decompressedSize))
            {
                using (var compressedStream = new MemoryStream(compressedData))
                {
                    using (var inflaterStream = new InflaterInputStream(compressedStream))
                    {
                        inflaterStream.CopyTo(decompressedStream);
                        decompressedStream.Seek(0, SeekOrigin.Begin);

                        return decompressedStream.ToArray();
                    }
                }
            }
        }
        #endregion
    }
}