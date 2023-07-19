using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Galaxy_Swapper_v2.Workspace.Generation.Formats
{
    /// <summary>
    /// All the code below was provided from: https://github.com/GalaxySwapperOfficial/Galaxy-Swapper-v2
    /// You can also find us at https://galaxyswapperv2.com/Guilded
    /// </summary>
    public class Option : ICloneable
    {
        public string Name;
        public string ID;
        public string Icon;
        public string OverrideIcon;
        public string Message;
        public string OptionMessage;
        public bool Nsfw = false;
        public JToken Parse;
        public List<Asset> Exports = new List<Asset>();
        public List<Downloadable> Downloadables = new List<Downloadable>();
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}