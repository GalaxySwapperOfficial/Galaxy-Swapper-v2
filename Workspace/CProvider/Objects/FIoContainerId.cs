using System.Runtime.InteropServices;

namespace Galaxy_Swapper_v2.Workspace.CProvider.Objects
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct FIoContainerId
    {
        public readonly ulong Id;
        public override string ToString()
        {
            return Id.ToString();
        }
    }
}