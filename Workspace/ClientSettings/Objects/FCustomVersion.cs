using Galaxy_Swapper_v2.Workspace.CProvider.Objects;
using Galaxy_Swapper_v2.Workspace.Swapping.Other;

namespace Galaxy_Swapper_v2.Workspace.ClientSettings.Objects
{
    public class FCustomVersion
    {
        public FGuid Key;
        public int Version;
        public FCustomVersion()
        {

        }

        public FCustomVersion(Reader reader)
        {
            Key = reader.Read<FGuid>();
            Version = reader.Read<int>();
        }

        public void Write(Writer writer)
        {
            writer.Write<FGuid>(Key);
            writer.Write<int>(Version);
        }
    }
}