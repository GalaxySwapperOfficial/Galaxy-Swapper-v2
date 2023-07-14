using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Galaxy_Swapper_v2.Workspace.Generation.Formats
{
    public class Frontend
    {
        public JToken Empty { get; set; }
        public Dictionary<string, Cosmetic> Cosmetics = new Dictionary<string, Cosmetic>();
        public List<Option> Options = new List<Option>();
    }
}