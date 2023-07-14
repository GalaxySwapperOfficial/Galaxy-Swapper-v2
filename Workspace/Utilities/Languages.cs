using Galaxy_Swapper_v2.Workspace.Properties;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Galaxy_Swapper_v2.Workspace.Utilities
{
    public static class Languages
    {
        private static JToken Parse { get; set; } = default!;
        public enum Type
        {
            View,
            Header,
            Message
        }

        public static string Read(Type Type, params string[] Keys)
        {
            if (Parse == null)
                Parse = Endpoint.Read(Endpoint.Type.Languages);

            JToken Path;

            switch(Type)
            {
                case Type.View:
                    Path = Parse["Views"];
                    break;
                case Type.Header:
                    Path = Parse["Headers"];
                    break;
                case Type.Message:
                    Path = Parse["Messages"];
                    break;
                default:
                    throw new Exception($"Failed to find {Type}");
            }

            Path = Keys.Aggregate(Path, (current, key) => current[key]);

            string selected = Settings.Read(Settings.Type.Language).Value<string>();
            return Path[selected].KeyIsNullOrEmpty() ? Path["EN"].Value<string>() : Path[selected].Value<string>();
        }

        public static List<string> ReadSolutions(Type Type, params string[] Keys)
        {
            if (Parse == null)
                Parse = Endpoint.Read(Endpoint.Type.Languages);

            JToken Path;

            switch (Type)
            {
                case Type.View:
                    Path = Parse["Views"];
                    break;
                case Type.Header:
                    Path = Parse["Headers"];
                    break;
                case Type.Message:
                    Path = Parse["Messages"];
                    break;
                default:
                    throw new Exception($"Failed to find {Type}");
            }

            Path = Keys.Aggregate(Path, (current, key) => current[key])["Solutions"];

            string selected = Settings.Read(Settings.Type.Language).Value<string>();
            var Array = Path[selected].KeyIsNullOrEmpty() ? (JArray)Path["EN"] : (JArray)Path[selected];

            return Array.ToObject<List<string>>();
        }
    }
}
