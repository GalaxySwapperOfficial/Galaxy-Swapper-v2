using CUE4Parse.UE4.Exceptions;
using CUE4Parse.UE4.Readers;
using CUE4Parse.Utils;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Serilog;

using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace CUE4Parse.Compression
{
    public class OodleException : ParserException
    {
        public OodleException(string? message = null, Exception? innerException = null) : base(message, innerException) { }
        public OodleException(FArchive reader, string? message = null, Exception? innerException = null) : base(reader, message, innerException) { }
    }

    public static class Oodle
    {
        public unsafe delegate long OodleDecompress(byte* bufferPtr, long bufferSize, byte* outputPtr, long outputSize, int a, int b, int c, long d, long e, long f, long g, long h, long i, int threadModule);
        public static OodleDecompress DecompressFunc;

        static unsafe Oodle()
        {
            DecompressFunc = OodleLZ_Decompress;
        }

        public static unsafe void Decompress(byte[] compressed, int compressedOffset, int compressedSize, byte[] uncompressed, int uncompressedOffset, int uncompressedSize, FArchive? reader = null)
        {
            long decodedSize;

            fixed (byte* compressedPtr = compressed, uncompressedPtr = uncompressed)
            {
                decodedSize = DecompressFunc(compressedPtr + compressedOffset, compressedSize, uncompressedPtr + uncompressedOffset, uncompressedSize, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3);
            }

            if (decodedSize <= 0)
            {
                if (reader != null) throw new OodleException(reader, $"Oodle decompression failed with result {decodedSize}");
                throw new OodleException($"Oodle decompression failed with result {decodedSize}");
            }
        }

        [DllImport("oo2core_5_win64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe long OodleLZ_Decompress(byte* buffer, long bufferSize, byte* output, long outputBufferSize, int a, int b, int c, long d, long e, long f, long g, long h, long i, int threadModule);
    }
}