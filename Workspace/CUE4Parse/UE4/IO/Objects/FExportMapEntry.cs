using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Readers;
using CUE4Parse.UE4.Versions;

namespace CUE4Parse.UE4.IO.Objects
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

        public FExportMapEntry(FArchive Ar)
        {
            var start = Ar.Position;
            CookedSerialOffset = Ar.Read<ulong>();
            CookedSerialSize = Ar.Read<ulong>();
            ObjectName = Ar.Read<FMappedName>();
            OuterIndex = Ar.Read<FPackageObjectIndex>();
            ClassIndex = Ar.Read<FPackageObjectIndex>();
            SuperIndex = Ar.Read<FPackageObjectIndex>();
            TemplateIndex = Ar.Read<FPackageObjectIndex>();
            if (Ar.Game >= EGame.GAME_UE5_0)
            {
                GlobalImportIndex = new FPackageObjectIndex(FPackageObjectIndex.Invalid);
                PublicExportHash = Ar.Read<ulong>();
            }
            else
            {
                GlobalImportIndex = Ar.Read<FPackageObjectIndex>();
                PublicExportHash = 0;
            }

            ObjectFlags = Ar.Read<EObjectFlags>();
            FilterFlags = Ar.Read<byte>();
            Ar.Position = start + Size;
        }
    }
}