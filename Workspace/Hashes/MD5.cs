using System.Text;

namespace Galaxy_Swapper_v2.Workspace.Hashes
{
    public static class MD5
    {
        public static byte[] Hash(byte[] buffer)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                return md5.ComputeHash(buffer);
            }
        }

        public static byte[] Hash(string content)
        {
            return Hash(Encoding.ASCII.GetBytes(content));
        }
    }
}