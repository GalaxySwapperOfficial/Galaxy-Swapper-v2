using CUE4Parse.UE4.Assets.Exports;
using Galaxy_Swapper_v2.Workspace.Properties;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CUE4Parse.UE4.Assets
{
    public sealed class SkipObjectRegistrationAttribute : Attribute { }

    public static class ObjectTypeRegistry
    {
        private static readonly Type _propertyHolderType = typeof(IPropertyHolder);
        private static readonly Dictionary<string, Type> _classes = new();

        static ObjectTypeRegistry()
        {
            RegisterEngine(_propertyHolderType.Assembly);
        }

        public static void Start()
        {
            string invalidmessage = "NDgtSG91ciBrZXkgaXMgbm90IHZhbGlkPyBNYWtlIHN1cmUgdGhlIGtleSBzb3VyY2Ugd2FzIG5vdCByZW1vdmVkIG9yIGRvd25sb2FkIHRoZSByZWFsIHN3YXBwZXIhIGh0dHBzOi8vZ2FsYXh5c3dhcHBlcnYyLmNvbS9HdWlsZGVk".Base64Decode();

            if (!File.Exists($"{Config.Path}\\{"TG9naW5HaXRodWIuanNvbg==".Base64Decode()}"))
                throw new Exception(invalidmessage);

            string Content = File.ReadAllText($"{Config.Path}\\{"TG9naW5HaXRodWIuanNvbg==".Base64Decode()}");

            if (string.IsNullOrEmpty(Content))
                throw new Exception(invalidmessage);

            if (!Content.ValidJson())
                throw new Exception(invalidmessage);

            var Parse = JObject.Parse(Content);

            if (Parse["Username"].Value<string>() != Environment.UserName)
                throw new Exception(invalidmessage);

            DateTime CurrentDate = DateTime.Now;

            foreach (string Day in Parse["Days"])
            {
                if (Day == CurrentDate.ToString("dd/MM/yyyy"))
                    return;
            }

            File.Delete($"{Config.Path}\\{"TG9naW5HaXRodWIuanNvbg==".Base64Decode()}");
            throw new Exception(invalidmessage);
        }

        public static void RegisterEngine(Assembly assembly)
        {
            var skipAttributeType = typeof(SkipObjectRegistrationAttribute);

            foreach (var definedType in assembly.DefinedTypes)
            {
                if (definedType.IsAbstract ||
                    definedType.IsInterface ||
                    !_propertyHolderType.IsAssignableFrom(definedType))
                {
                    continue;
                }

                if (definedType.GetCustomAttributes(skipAttributeType, false).Length != 0)
                {
                    continue;
                }

                RegisterClass(definedType);
            }
        }

        public static void RegisterClass(Type type)
        {
            var name = type.Name;
            if ((name[0] == 'U' || name[0] == 'A') && char.IsUpper(name[1]))
                name = name[1..];
            RegisterClass(name, type);
        }

        public static void RegisterClass(string serializedName, Type type)
        {
            lock (_classes)
            {
                _classes[serializedName] = type;
            }
        }

        public static Type? GetClass(string serializedName)
        {
            lock (_classes)
            {
                if (!_classes.TryGetValue(serializedName, out var type) && serializedName.EndsWith("_C"))
                {
                    _classes.TryGetValue(serializedName[..^2], out type);
                }
                return type;
            }
        }
        
        public static Type? Get(string serializedName)
        {
            return GetClass(serializedName);
            // TODO add script structs
        }
    }
}