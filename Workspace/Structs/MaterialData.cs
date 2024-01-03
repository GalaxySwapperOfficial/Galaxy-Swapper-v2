using System.Collections.Generic;

namespace Galaxy_Swapper_v2.Workspace.Structs
{
    public class MaterialData
    {
        public byte[] SearchBuffer { get; set; } = default!;
        public long Offset { get; set; } = default!;
        public MaterialOverrideFlagsData MaterialOverrideFlags { get; set; } = default!;
        public List<OverrideMaterial> Materials = new();
    }
}