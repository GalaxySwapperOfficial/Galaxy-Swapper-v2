using System.Runtime.InteropServices;

namespace Galaxy_Swapper_v2.Workspace.CProvider.Objects
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