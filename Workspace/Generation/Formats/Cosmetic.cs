using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Galaxy_Swapper_v2.Workspace.Generation.Formats
{
    /// <summary>
    /// All the code below was provided from: https://github.com/GalaxySwapperOfficial/Galaxy-Swapper-v2
    /// You can also find us at https://galaxyswapperv2.com/Guilded
    /// </summary>
    public class Cosmetic
    {
        public string Name;
        public string ID;
        public string Icon;
        public string OverrideFrontend;
        public string Message;
        public bool Nsfw = false;
        public JToken Parse;
        public List<Option> Options = new List<Option>();
        public List<Downloadable> Downloadables = new List<Downloadable>();
    }
}