namespace Galaxy_Swapper_v2.Workspace.Verify.EpicManifestParser.Enums
{
    public enum EManifestMetaVersion : byte
    {
        Original = 0,
        SerialisesBuildId,
        LatestPlusOne,
        Latest = LatestPlusOne - 1
    }
}