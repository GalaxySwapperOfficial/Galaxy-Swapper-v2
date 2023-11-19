using Galaxy_Swapper_v2.Workspace.Structs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Galaxy_Swapper_v2.Workspace.Generation.Formats
{
    public class Option : ICloneable
    {
        public string Name;
        public string ID;
        public string Icon;
        public string OverrideIcon;
        public string Message;
        public string OptionMessage;
        public bool Nsfw = false;
        public bool UseMainUEFN = false;
        public bool UEFNFormat = false;
        public bool IsPlugin = false;
        public string UEFNTag;
        public Cosmetic Cosmetic;
        public JToken Parse;
        public List<Asset> Exports = new List<Asset>();
        public List<Downloadable> Downloadables = new List<Downloadable>();
        public List<Social> Socials = new List<Social>();

        public object Clone() => new Option() { Name = Name, ID = ID, Icon = Icon, OptionMessage = OptionMessage, Parse = Parse };
        public object Clone(bool all) => new Option() { Name = Name, ID = ID, Icon = Icon, OverrideIcon = OverrideIcon, Message = Message, OptionMessage = OptionMessage, Nsfw = Nsfw, UseMainUEFN = UseMainUEFN, UEFNFormat = UEFNFormat, IsPlugin = IsPlugin, UEFNTag = UEFNTag, Cosmetic = Cosmetic, Parse = Parse, Downloadables = Downloadables, Socials = Socials, Exports = Exports };
    }
}