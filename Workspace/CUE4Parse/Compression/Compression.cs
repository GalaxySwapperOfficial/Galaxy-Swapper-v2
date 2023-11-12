using CUE4Parse.UE4.Exceptions;
using CUE4Parse.UE4.Readers;
using Ionic.Zlib;
using K4os.Compression.LZ4;
using System;
using System.IO;

namespace CUE4Parse.Compression
{
    public static class Compression
    {
        public const int LOADING_COMPRESSION_CHUNK_SIZE = 131072;

        public static byte[] Decompress(byte[] compressed, int uncompressedSize, CompressionMethod method, FArchive? reader = null)
        {
            byte[] uncompressed = new byte[uncompressedSize];
            Decompress(compressed, 0, compressed.Length, uncompressed, 0, uncompressed.Length, method, reader);
            return uncompressed;
        }

        public static void Decompress(byte[] compressed, byte[] dst, CompressionMethod method, FArchive? reader = null) =>
            Decompress(compressed, 0, compressed.Length, dst, 0, dst.Length, method, reader);

        public static void Decompress(byte[] compressed, int compressedOffset, int compressedSize, byte[] uncompressed, int uncompressedOffset, int uncompressedSize, CompressionMethod method, FArchive? reader = null)
        {
            using var srcStream = new MemoryStream(compressed, compressedOffset, compressedSize, false) { Position = 0 };
            switch (method)
            {
                case CompressionMethod.None:
                    Buffer.BlockCopy(compressed, compressedOffset, uncompressed, uncompressedOffset, compressedSize);
                    return;
                case CompressionMethod.Zlib:
                    new ZlibStream(srcStream, CompressionMode.Decompress).Read(uncompressed, uncompressedOffset, uncompressedSize);
                    return;
                case CompressionMethod.Gzip:
                    new GZipStream(srcStream, CompressionMode.Decompress).Read(uncompressed, uncompressedOffset, uncompressedSize);
                    return;
                case CompressionMethod.Oodle:
                    Oodle.Decompress(compressed, compressedOffset, compressedSize, uncompressed, uncompressedOffset, uncompressedSize, reader);
                    return;
                case CompressionMethod.LZ4:
                    var uncompressedBuffer = new byte[uncompressedSize + uncompressedSize / 255 + 16];
                    int result = LZ4Codec.Decode(compressed, compressedOffset, compressedSize, uncompressedBuffer, 0, uncompressedBuffer.Length);
                    Buffer.BlockCopy(uncompressedBuffer, 0, uncompressed, uncompressedOffset, uncompressedSize);
                    if (result != uncompressedSize) throw new FileLoadException($"Failed to decompress LZ4 data (Expected: {uncompressedSize}, Result: {result})");
                    return;
                default:
                    throw reader != null ? new UnknownCompressionMethodException(reader, $"Compression method \"{method}\" is unknown") : new UnknownCompressionMethodException($"Compression method \"{method}\" is unknown");
            }
        }
    }
}
