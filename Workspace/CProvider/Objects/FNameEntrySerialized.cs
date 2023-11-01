using System;
using System.Runtime.InteropServices;

namespace Galaxy_Swapper_v2.Workspace.CProvider.Objects
{
    public struct FNameEntrySerialized
    {
        public string? Name;
        public ulong hashVersion;
#if NAME_HASHES
        public readonly ushort NonCasePreservingHash;
        public readonly ushort CasePreservingHash;
#endif

        public FNameEntrySerialized(string name, ulong HashVersion = 0)
        {
            Name = name;
            hashVersion = HashVersion;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = Size)]
    public readonly struct FSerializedNameHeader : IEquatable<FSerializedNameHeader>
    {
        public const int Size = 2;

        private readonly byte _data0;
        private readonly byte _data1;

        public bool IsUtf16 => (_data0 & 0x80u) != 0;
        public uint Length => ((_data0 & 0x7Fu) << 8) + _data1;

        public bool Equals(FSerializedNameHeader other)
        {
            return _data0 == other._data0 && _data1 == other._data1;
        }

        public override bool Equals(object? obj)
        {
            return obj is FSerializedNameHeader other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_data0, _data1);
        }

        public static bool operator ==(FSerializedNameHeader left, FSerializedNameHeader right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FSerializedNameHeader left, FSerializedNameHeader right)
        {
            return !left.Equals(right);
        }
    }
}