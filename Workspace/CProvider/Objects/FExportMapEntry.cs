namespace Galaxy_Swapper_v2.Workspace.CProvider.Objects
{
    public struct FExportMapEntry
    {
        public const int Size = 72;

        public ulong CookedSerialOffset;
        public ulong CookedSerialSize;
        public FMappedName ObjectName;
        public FPackageObjectIndex OuterIndex;
        public FPackageObjectIndex ClassIndex;
        public FPackageObjectIndex SuperIndex;
        public FPackageObjectIndex TemplateIndex;
        public FPackageObjectIndex GlobalImportIndex;
        public ulong PublicExportHash;
        public EObjectFlags ObjectFlags;
        public byte FilterFlags; // EExportFilterFlags: client/server flags
    }
}