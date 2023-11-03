using System;
using System.Text;

namespace Galaxy_Swapper_v2.Workspace.Verify.EpicManifestParser.Objects
{
    public readonly struct FStringMemory
    {
        public Memory<byte> Memory { get; }
        public bool IsUnicode { get; }
        public Span<byte> GetSpan() => Memory.Span;
        public bool IsEmpty() => Memory.IsEmpty;
        public Encoding GetEncoding() => IsUnicode ? Encoding.Unicode : Encoding.UTF8;

        public FStringMemory(Memory<byte> memory, bool isUnicode)
        {
            Memory = memory;
            IsUnicode = isUnicode;
        }

        public override string ToString() => GetEncoding().GetString(GetSpan());
    }
}