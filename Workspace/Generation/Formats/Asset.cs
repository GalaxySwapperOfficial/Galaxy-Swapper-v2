using Newtonsoft.Json.Linq;
using System;

namespace Galaxy_Swapper_v2.Workspace.Generation.Formats
{
    /// <summary>
    /// All the code below was provided from: https://github.com/GalaxySwapperOfficial/Galaxy-Swapper-v2
    /// You can also find us at https://galaxyswapperv2.com/Guilded
    /// </summary>
    public class Asset : ICloneable
    {
        public string Object;
        public string OverrideObject;
        public string OverrideBuffer;
        public JToken Swaps;
        public Export Export { get; set; } = default!;
        public Export OverrideExport { get; set; } = default!;
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}