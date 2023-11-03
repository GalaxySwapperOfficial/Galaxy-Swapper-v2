namespace Galaxy_Swapper_v2.Workspace.Verify.EpicManifestParser.Enums
{
    public enum EChunkDataListVersion : byte
    {
        Original = 0,
        LatestPlusOne,
        Latest = LatestPlusOne - 1
    }
}