using Galaxy_Swapper_v2.Workspace.Properties;
using Newtonsoft.Json.Linq;
using System;
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

        public static string[] ReadSolutions(Type type, params string[] keys)
        {
            if (Parse is not null)
            {
                Parse = Endpoint.Read(Endpoint.Type.Languages);
            }

            JToken jToken = keys.Aggregate(
                type switch
                {
                    Type.View => Parse["Views"],
                    Type.Header => Parse["Headers"],
                    Type.Message => Parse["Messages"],
                    _ => throw new Exception($"Failed to find {type}"),
                },
                (current, key) => current[key])["Solutions"];

            string language = Settings.Read(Settings.Type.Language).Value<string>();
            return jToken[language].KeyIsNullOrEmpty() ? ((JArray)jToken["EN"]).ToObject<string[]>() : ((JArray)jToken[language]).ToObject<string[]>();
        }
    }
}
