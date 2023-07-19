using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Galaxy_Swapper_v2.Workspace.Generation.Formats
{
    /// <summary>
    /// All the code below was provided from: https://github.com/GalaxySwapperOfficial/Galaxy-Swapper-v2
    /// You can also find us at https://galaxyswapperv2.com/Guilded
    /// </summary>
    public class Frontend
    {
        public JToken Empty { get; set; }
        public Dictionary<string, Cosmetic> Cosmetics = new Dictionary<string, Cosmetic>();
        public List<Option> Options = new List<Option>();
    }
}