using Galaxy_Swapper_v2.Workspace.CProvider.Objects;
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
        public GameFile Export { get; set; } = default!;
        public GameFile OverrideExport { get; set; } = default!;
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}