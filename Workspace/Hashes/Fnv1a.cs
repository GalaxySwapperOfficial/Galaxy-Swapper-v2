using System;

namespace Galaxy_Swapper_v2.Workspace.Hashes
{
    public static class Fnv1a
    {
        private const uint OffsetBasis = 2166136261;
        private const uint Prime = 16777619;

        public static uint Hash(string content)
        {
            uint hash = OffsetBasis;

            foreach (char c in content)
            {
                hash ^= c;
                hash *= Prime;
            }

            return hash;
        }

        public static string ToString(this uint hash)
        {
            return hash.ToString();
        }

        public static byte[] ToBytes(this uint hash)
        {
            return BitConverter.GetBytes(hash);
        }
    }
}