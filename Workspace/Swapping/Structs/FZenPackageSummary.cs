using CUE4Parse.UE4.IO.Objects;
using CUE4Parse.UE4.Objects.UObject;
using Galaxy_Swapper_v2.Workspace.Swapping.Other;

namespace Galaxy_Swapper_v2.Workspace.Swapping.Structs
{
    /// <summary>
    /// All the code below was provided from: https://github.com/GalaxySwapperOfficial/Galaxy-Swapper-v2
    /// You can also find us at https://galaxyswapperv2.com/Guilded
    /// </summary>
    public struct FZenPackageSummary
    {
        public uint bHasVersioningInfo;
        public uint HeaderSize;
        public FMappedName Name;
        public EPackageFlags PackageFlags;
        public uint CookedHeaderSize;
        public int ImportedPublicExportHashesOffset;
        public int ImportMapOffset;
        public int ExportMapOffset;
        public int ExportBundleEntriesOffset;
        public int DependencyBundleHeadersOffset;
        public int DependencyBundleEntriesOffset;
        public int ImportedPackageNamesOffset;

        public FZenPackageSummary(Reader Ar)
        {
            bHasVersioningInfo = Ar.Read<uint>();
            HeaderSize = Ar.Read<uint>();
            Name = Ar.Read<FMappedName>();
            PackageFlags = Ar.Read<EPackageFlags>();
            CookedHeaderSize = Ar.Read<uint>();
            ImportedPublicExportHashesOffset = Ar.Read<int>();
            ImportMapOffset = Ar.Read<int>();
            ExportMapOffset = Ar.Read<int>();
            ExportBundleEntriesOffset = Ar.Read<int>();
            DependencyBundleHeadersOffset = Ar.Read<int>();
            DependencyBundleEntriesOffset = Ar.Read<int>();
            ImportedPackageNamesOffset = Ar.Read<int>();
        }
    }
}