using System.Runtime.InteropServices;

namespace CUE4Parse.UE4.IO.Objects
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FBulkDataMapEntry
    {
        public const uint Size = 32;

        public ulong SerialOffset;
        public ulong DuplicateSerialOffset;
        public ulong SerialSize;
        public uint Flags;
        public uint Pad;
    }
}