using System.Collections.Generic;

namespace Galaxy_Swapper_v2.Workspace.Structs
{
    public class TextureData
    {
        public byte[] SearchBuffer { get; set; } = default!;
        public long Offset { get; set; } = default!;
        public List<TextureParameter> TextureParameters = new();
    }
}