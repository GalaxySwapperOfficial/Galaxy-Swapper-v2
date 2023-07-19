using CUE4Parse.UE4.Objects.Core.Serialization;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse.UE4.Readers;
using CUE4Parse.UE4.Versions;
using Serilog;

namespace CUE4Parse.UE4.IO.Objects
{
    public enum EZenPackageVersion : uint
    {
        Initial,
        DataResourceTable,
        ImportedPackageNames,

        LatestPlusOne,
        Latest = LatestPlusOne - 1
    }

    public struct FZenPackageVersioningInfo
    {
        public EZenPackageVersion ZenVersion;
        public FPackageFileVersion PackageVersion;
        public int LicenseeVersion;
        public FCustomVersionContainer CustomVersions;

        public FZenPackageVersioningInfo(FArchive Ar)
        {
            ZenVersion = Ar.Read<EZenPackageVersion>();
            PackageVersion = Ar.Read<FPackageFileVersion>();
            LicenseeVersion = Ar.Read<int>();
            CustomVersions = new FCustomVersionContainer(Ar);
        }
    }

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
        public int GraphDataOffset;
        public int DependencyBundleHeadersOffset;
        public int DependencyBundleEntriesOffset;
        public int ImportedPackageNamesOffset;

        public FZenPackageSummary(FArchive Ar)
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

            if (Ar.Game >= EGame.GAME_UE5_2)
            {
                DependencyBundleHeadersOffset = Ar.Read<int>();
                DependencyBundleEntriesOffset = Ar.Read<int>();

                ImportedPackageNamesOffset = Ar.Read<int>();
            }
            else
            {
                GraphDataOffset = Ar.Read<int>();
            }
        }
    }
}
