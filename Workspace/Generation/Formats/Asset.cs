using Newtonsoft.Json.Linq;
using System;

namespace Galaxy_Swapper_v2.Workspace.Generation.Formats
{
    public class Asset : ICloneable
    {
        public string Object;
        public string OverrideObject;
        public string OverrideBuffer;
        public JToken Swaps;
        public Structs.Export Export { get; set; } = default!;
        public Structs.Export OverrideExport { get; set; } = default!;
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}