using Galaxy_Swapper_v2.Workspace.Structs;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Galaxy_Swapper_v2.Workspace.Generation.Formats
{
    public class Cosmetic
    {
        public string Name;
        public string ID;
        public string Icon;
        public string OverrideFrontend;
        public string Message;
        public bool Nsfw = false;
        public bool UseMainUEFN = false;
        public string UEFNTag;
        public int Stats = 0;
        public Generate.Type Type;
        public JToken Parse;
        public List<Option> Options = new List<Option>();
        public List<Downloadable> Downloadables = new List<Downloadable>();
        public List<Social> Socials = new List<Social>();
    }
}