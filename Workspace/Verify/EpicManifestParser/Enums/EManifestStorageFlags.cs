namespace Galaxy_Swapper_v2.Workspace.Verify.EpicManifestParser.Enums
{
    public enum EManifestStorageFlags : byte
    {
        None = 0,
        Compressed = 1,
        Encrypted = 1 << 1,
    }
}