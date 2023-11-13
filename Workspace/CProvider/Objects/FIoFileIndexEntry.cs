using System.Runtime.InteropServices;

namespace Galaxy_Swapper_v2.Workspace.CProvider.Objects
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct FIoFileIndexEntry
    {
        public readonly uint Name;
        public readonly uint NextFileEntry;
        public readonly uint UserData;
    }
}