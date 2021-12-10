using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;

namespace Galaxy_Swapper_v2.Workspace.Other
{
    public static class Oodle
    {
        public static byte[] Compress(byte[] buffer)
        {
            uint Uint;
            Uint = (uint)Oodle.OodleLZ_Compress(OodleFormat.Kraken, buffer, buffer.Length, new byte[(int)(uint)buffer.Length + 274U * (((uint)buffer.Length + 262143U) / 262144U)], OodleCompressionLevel.Level5, 0U, 0U, 0U, 0);
            return OodleCompress(buffer, buffer.Length, OodleFormat.Kraken, OodleCompressionLevel.Level5, Uint);
        }
        [DllImport("oo2core_8_win64.dll")]
        public static extern int OodleLZ_Compress(OodleFormat format, byte[]? decompressedBuffer, long decompressedSize, byte[] compressedBuffer, OodleCompressionLevel compressionLevel, uint a, uint b, uint c, uint threadModule);
        public static byte[] OodleCompress(byte[]? decompressedBuffer, int decompressedSize, OodleFormat format, OodleCompressionLevel compressionLevel, uint a)
        {
            var array = new byte[(uint)decompressedSize + 274U * (((uint)decompressedSize + 262143U) / 262144U)];
            var compressedBytes = new byte[a + (uint)OodleLZ_Compress(format, decompressedBuffer, decompressedSize, array, compressionLevel, 0U, 0U, 0U, 0U) - (int)a];
            Buffer.BlockCopy(array, 0, compressedBytes, 0, OodleLZ_Compress(format, decompressedBuffer, decompressedSize, array, compressionLevel, 0U, 0U, 0U, 0U));
            return compressedBytes;
        }
        public enum OodleFormat : uint
        {
            LZH,
            LZHLW,
            LZNIB,
            None,
            LZB16,
            LZBLW,
            LZA,
            LZNA,
            Kraken,
            Mermaid,
            BitKnit,
            Selkie,
            Akkorokamui
        }
        public enum OodleCompressionLevel : ulong
        {
            None,
            Fastest,
            Faster,
            Fast,
            Normal,
            Level1,
            Level2,
            Level3,
            Level4,
            Level5
        }
    }
}