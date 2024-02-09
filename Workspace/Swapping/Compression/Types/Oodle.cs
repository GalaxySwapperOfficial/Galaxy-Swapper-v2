﻿using System;
using System.Runtime.InteropServices;

namespace Galaxy_Swapper_v2.Workspace.Swapping.Compression.Types
{
    public static class Oodle
    {
        [DllImport("oo2core_5_win64.dll")]
        private static extern int OodleLZ_Compress(OodleFormat format, byte[] decompressedBuffer, long decompressedSize, byte[] compressedBuffer, OodleCompressionLevel compressionLevel, uint a, uint b, uint c, uint threadModule);

        [DllImport("oo2core_5_win64.dll")]
        private static extern int OodleLZ_Decompress(byte[] buffer, long bufferSize, byte[] outputBuffer, long outputBufferSize, uint a, uint b, ulong c, uint d, uint e, uint f, uint g, uint h, uint i, uint threadModule);

        public static byte[] Compress(byte[] buffer, OodleCompressionLevel CompressionLevel)
        {
            var array = new byte[(uint)buffer.Length + 274U * (((uint)buffer.Length + 262143U) / 262144U)];
            var compressedBytes = new byte[OodleLZ_Compress(OodleFormat.Kraken, buffer, buffer.Length, array, CompressionLevel, 0U, 0U, 0U, 0U)];
            Buffer.BlockCopy(array, 0, compressedBytes, 0, compressedBytes.Length);
            return compressedBytes;
        }

        public static byte[] Decompress(byte[] data, int decompressedSize)
        {
            byte[] decompressedData = new byte[decompressedSize];
            Decompress(data, (uint)data.Length, ref decompressedData, (uint)decompressedSize);
            return decompressedData;
        }

        private enum OodleFormat : uint
        {
            LZH, LZHLW, LZNIB, None, LZB16, LZBLW, LZA, LZNA, Kraken, Mermaid, BitKnit, Selkie, Akkorokamui
        }

        public enum OodleCompressionLevel : ulong
        {
            None, Fastest, Faster, Fast, Normal, Level1, Level2, Level3, Level4, Level5
        }

        private static uint Decompress(byte[] buffer, uint bufferSize, ref byte[] outputBuffer, uint outputBufferSize)
        {
            if (buffer.Length > 0 && bufferSize > 0 && outputBuffer.Length > 0 && outputBufferSize > 0)
                return (uint)OodleLZ_Decompress(buffer, bufferSize, outputBuffer, outputBufferSize, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            return 0;
        }
    }
}
