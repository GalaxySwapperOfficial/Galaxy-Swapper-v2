using System.Runtime.CompilerServices;

namespace Galaxy_Swapper_v2.Workspace.Utilities
{
    public static class AlignUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Align(this long ptr, int alignment)
        {
            return ptr + alignment - 1 & ~(alignment - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Align(this int ptr, int alignment)
        {
            return ptr + alignment - 1 & ~(alignment - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Align(this uint ptr, int alignment)
        {
            return ptr + alignment - 1 & ~(alignment - 1);
        }
    }
}