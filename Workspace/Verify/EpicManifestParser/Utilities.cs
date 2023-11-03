using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Galaxy_Swapper_v2.Workspace.Verify.EpicManifestParser
{
    internal static unsafe class Utilities
    {
        private const byte _zeroChar = (byte)'0';

        public static T StringBlobTo<T>(ReadOnlySpan<byte> stringBlob) where T : unmanaged
        {
            var bpBuffer = stackalloc byte[sizeof(T)];
            var pBuffer = bpBuffer;

            fixed (byte* bBlob = &stringBlob.GetPinnableReference())
            {
                var i = 0;

                while (i < stringBlob.Length)
                {
                    var b1 = bBlob[i++] - _zeroChar;
                    var b2 = bBlob[i++] - _zeroChar;
                    var b3 = bBlob[i++] - _zeroChar;
                    *pBuffer++ = (byte)(b1 * 100 + b2 * 10 + b3);
                }
            }

            return *(T*)bpBuffer;
        }

        public static string StringBlobToHexString(ReadOnlySpan<byte> stringBlob, bool reversed = false)
        {
            var length = stringBlob.Length / 3;
            var bpBuffer = stackalloc byte[length];
            var pBuffer = bpBuffer;

            fixed (byte* bBlob = &stringBlob.GetPinnableReference())
            {
                if (reversed)
                {
                    var i = stringBlob.Length - 1;

                    while (i > 0)
                    {
                        var b3 = bBlob[i--] - _zeroChar;
                        var b2 = bBlob[i--] - _zeroChar;
                        var b1 = bBlob[i--] - _zeroChar;
                        *pBuffer++ = (byte)(b1 * 100 + b2 * 10 + b3);
                    }
                }
                else
                {
                    var i = 0;

                    while (i < stringBlob.Length)
                    {
                        var b1 = bBlob[i++] - _zeroChar;
                        var b2 = bBlob[i++] - _zeroChar;
                        var b3 = bBlob[i++] - _zeroChar;
                        *pBuffer++ = (byte)(b1 * 100 + b2 * 10 + b3);
                    }
                }
            }

            var lookupP = _lookup32UnsafeP;
            var result = new string(char.MinValue, length * 2);

            fixed (char* resultP = result)
            {
                var resultP2 = (uint*)resultP;

                for (var i = 0; i < length; i++)
                {
                    resultP2[i] = lookupP[bpBuffer[i]];
                }
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetByte(ReadOnlySpan<byte> stringBlob)
        {
            fixed (byte* pBlob = &stringBlob.GetPinnableReference())
            {
                var b1 = pBlob[0] - _zeroChar;
                var b2 = pBlob[1] - _zeroChar;
                var b3 = pBlob[2] - _zeroChar;

                return (byte)(b1 * 100 + b2 * 10 + b3);
            }
        }

        private static readonly uint[] _lookup32Unsafe = CreateLookup32Unsafe();
        private static readonly uint* _lookup32UnsafeP = (uint*)GCHandle.Alloc(_lookup32Unsafe, GCHandleType.Pinned).AddrOfPinnedObject();

        private static uint[] CreateLookup32Unsafe()
        {
            var result = new uint[256];

            for (var i = 0; i < 256; i++)
            {
                var s = i.ToString("X2");

                if (BitConverter.IsLittleEndian)
                {
                    result[i] = s[0] + ((uint)s[1] << 16);
                }
                else
                {
                    result[i] = s[1] + ((uint)s[0] << 16);
                }
            }

            return result;
        }
    }
}