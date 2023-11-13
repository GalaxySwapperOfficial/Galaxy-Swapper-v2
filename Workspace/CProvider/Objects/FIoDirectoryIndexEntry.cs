using System.Runtime.InteropServices;

namespace Galaxy_Swapper_v2.Workspace.CProvider.Objects
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct FIoDirectoryIndexEntry
    {
        public readonly uint Name;
        public readonly uint FirstChildEntry;
        public readonly uint NextSiblingEntry;
        public readonly uint FirstFileEntry;
    }
}