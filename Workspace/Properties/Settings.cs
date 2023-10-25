using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;

namespace Galaxy_Swapper_v2.Workspace.Properties
{
    public static class Settings
    {
        public static Dictionary<string, dynamic> Cache = new Dictionary<string, dynamic>();
        public static readonly string Path = $"{App.Config}\\Settings.json";
        public static JObject Parse { get; set; } = default!;
        public enum Type
        {
            Installtion,
            EpicInstalltion,
            Language,
            RichPresence,
            CloseFortnite,
            KickWarning,
            Reminded,
            CharacterGender,
            BackpackGender,
            HideNsfw,
            ShareStats,
            SortByStats,
            HeroDefinition
        }
        public static void Initialize()
        {
            if (!IsValid())
                Create();

            Populate();
            Log.Information("Successfully initialized settings");
        }

        private static bool IsValid()
        {
            if (!File.Exists(Path))
                return false;

            if (!Misc.CanEdit(Path))
                return false;

            string Content = File.ReadAllText(Path);

            if (string.IsNullOrEmpty(Content) || !Content.ValidJson())
                return false;

            var parse = JObject.Parse(Content);

            foreach (string Key in Enum.GetNames(typeof(Type)))
            {
                if (parse[Key] == null)
                    return false;
            }

            Parse = parse;
            return true;
        }

        private static void Create()
        {
            var Object = JObject.FromObject(new
            {
                Installtion = EpicGamesLauncher.FortniteInstallation(),
                EpicInstalltion = EpicGamesLauncher.Installation(),
                Language = "EN",
                RichPresence = true,
                CloseFortnite = true,
                KickWarning = true,
                Reminded = string.Empty,
                CharacterGender = true,
                BackpackGender = true,
                HideNsfw = false,
                ShareStats = true,
                SortByStats = true,
                HeroDefinition = true
            });

            Parse = Object;

            try
            {
                if (File.Exists(Path))
                    File.Delete(Path);

                File.WriteAllText(Path, Object.ToString());
                Log.Information($"Created Settings file to {Path}");
            }
            catch (Exception Exception)
            {
                Log.Error(Exception, $"Failed to write settings file! Settings will now only be in memory");
            }
        }

        public static void Populate()
        {
            try
            {
                if (Cache.Count != 0)
                    Cache.Clear();

                foreach (string Key in Enum.GetNames(typeof(Type)))
                    Cache.Add(Key, Parse[Key].Value<dynamic>());

                Log.Information($"Successfully populated settings cache with {Cache.Count} enum properties");
            }
            catch (Exception Exception)
            {
                Log.Error(Exception, "Failed to populated settings cache");
            }
        }

        public static JToken Read(Type Type)
        {
            if (Cache == null || !Cache.ContainsKey($"{Type}"))
            {
                Log.Error($"Settings cache does not contain {Type} attempting to fix this.");
                Create();
                Populate();
            }
            return Cache[Type.ToString()];
        }

        public static void Edit(Type Key, JToken Value)
        {
            Cache[Key.ToString()] = Value;

            var NewObject = new JObject();
            foreach (var Object in Cache)
            {
                NewObject.Add(Object.Key, Object.Value);
            }

            Parse = NewObject;

            Log.Information($"Set {Key} to {Value}");

            try
            {
                File.WriteAllText(Path, NewObject.ToString());
            }
            catch (Exception Exception)
            {
                Log.Error(Exception, $"Failed to write settings file! New changes will only be in memory");
            }
        }
    }
}