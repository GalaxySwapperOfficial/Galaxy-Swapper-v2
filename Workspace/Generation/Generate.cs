using Galaxy_Swapper_v2.Workspace.Generation.Formats;
using Galaxy_Swapper_v2.Workspace.Generation.Types;
using Galaxy_Swapper_v2.Workspace.Properties;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Galaxy_Swapper_v2.Workspace.Generation
{
    /// <summary>
    /// All the code below was provided from: https://github.com/GalaxySwapperOfficial/Galaxy-Swapper-v2
    /// You can also find us at https://galaxyswapperv2.com/Guilded
    /// </summary>
    public static class Generate
    {
        public enum Type
        {
            Characters,
            Backpacks,
            Pickaxes,
            Dances,
            Gliders,
            Weapons,
            Misc
        }
        public static string Domain { get; set; } = default!;
        public static Dictionary<Type, Frontend> Cache = new Dictionary<Type, Frontend>();
        public static Dictionary<string, Cosmetic> Frontend(Type Type)
        {
            if (!Cache.ContainsKey(Type))
            {
                var Parse = Endpoint.Read(Endpoint.Type.Cosmetics);
                var Stopwatch = new Stopwatch();
                Stopwatch.Start();

                if (string.IsNullOrEmpty(Domain))
                    Domain = Parse["Low"].Value<string>();

                Parse = Parse[Type.ToString()];

                var Array = Parse["Array"];
                var NewCache = new Frontend();

                if (!Parse["Empty"].KeyIsNullOrEmpty())
                    NewCache.Empty = Parse["Empty"];

                if (Array.Count() != 0)
                {
                    foreach (var Cosmetic in Array)
                    {
                        if (Cosmetic["Frontend"].Value<bool>())
                        {
                            var NewCosmetic = new Cosmetic() { Name = Cosmetic["Name"].Value<string>(), ID = Cosmetic["ID"].Value<string>(), Parse = Cosmetic };

                            if (!Cosmetic["Message"].KeyIsNullOrEmpty())
                                NewCosmetic.Message = Cosmetic["Message"].Value<string>();

                            if (Cosmetic["Override"].KeyIsNullOrEmpty())
                                NewCosmetic.Icon = string.Format(Domain, NewCosmetic.ID);
                            else
                                NewCosmetic.Icon = Cosmetic["Override"].Value<string>();

                            if (!Cosmetic["OverrideFrontend"].KeyIsNullOrEmpty())
                                NewCosmetic.OverrideFrontend = Cosmetic["OverrideFrontend"].Value<string>();

                            if (!Cosmetic["Nsfw"].KeyIsNullOrEmpty())
                                NewCosmetic.Nsfw = Cosmetic["Nsfw"].Value<bool>();

                            if (Cosmetic["Downloadables"] != null)
                            {
                                foreach (var downloadable in Cosmetic["Downloadables"])
                                {
                                    if (!downloadable["pak"].KeyIsNullOrEmpty() && !downloadable["sig"].KeyIsNullOrEmpty() && !downloadable["ucas"].KeyIsNullOrEmpty() && !downloadable["utoc"].KeyIsNullOrEmpty())
                                        NewCosmetic.Downloadables.Add(new Downloadable() { Pak = downloadable["pak"].Value<string>(), Sig = downloadable["sig"].Value<string>(), Ucas = downloadable["ucas"].Value<string>(), Utoc = downloadable["utoc"].Value<string>() });
                                }
                            }

                            NewCache.Cosmetics.Add($"{NewCosmetic.Name}:{NewCosmetic.ID}", NewCosmetic);
                        }

                        if (Cosmetic["Option"] != null && Cosmetic["Option"].Value<bool>())
                        {
                            var NewOption = new Option() { Name = Cosmetic["Name"].Value<string>(), ID = Cosmetic["ID"].Value<string>(), Parse = Cosmetic };

                            if (!Cosmetic["Message"].KeyIsNullOrEmpty())
                                NewOption.OptionMessage = Cosmetic["Message"].Value<string>();

                            if (Cosmetic["Override"].KeyIsNullOrEmpty())
                                NewOption.Icon = string.Format(Domain, NewOption.ID);
                            else
                                NewOption.Icon = Cosmetic["Override"].Value<string>();

                            NewCache.Options.Add(NewOption);
                        }
                    }
                }

                Cache.Add(Type, NewCache);
                Log.Information($"Loaded {Type} in {Stopwatch.Elapsed.TotalSeconds} seconds, With {Array.Count()} items!");
            }
            return Cache[Type].Cosmetics;
        }

        public static List<Option> Options(string Key, Type Type)
        {
            if (Cache[Type].Cosmetics[Key].Options.Count == 0)
            {
                switch (Type)
                {
                    case Type.Characters:
                        if (Settings.Read(Settings.Type.CharacterGender).Value<bool>())
                            Neutral.Format(Cache[Type].Cosmetics[Key], Cache[Type].Options, Cache[Type].Empty, Type, "Gender");
                        else
                            Neutral.Format(Cache[Type].Cosmetics[Key], Cache[Type].Options, Cache[Type].Empty, Type);
                        break;
                    case Type.Backpacks:
                        if (Settings.Read(Settings.Type.BackpackGender).Value<bool>())
                            Neutral.Format(Cache[Type].Cosmetics[Key], Cache[Type].Options, Cache[Type].Empty, Type, "Gender", "Type");
                        else
                            Neutral.Format(Cache[Type].Cosmetics[Key], Cache[Type].Options, Cache[Type].Empty, Type, "Type");
                        break;
                    case Type.Pickaxes:
                        Neutral.Format(Cache[Type].Cosmetics[Key], Cache[Type].Options, Cache[Type].Empty, Type, "Type");
                        break;
                    case Type.Dances:
                        Dances.Format(Cache[Type].Cosmetics[Key], Cache[Type].Options);
                        break;
                    case Type.Gliders:
                        Gliders.Format(Cache[Type].Cosmetics[Key], Cache[Type].Options);
                        break;
                    case Type.Weapons:
                        Weapons.Format(Cache[Type].Cosmetics[Key]);
                        break;
                }
            }
            return Cache[Type].Cosmetics[Key].Options;
        }
    }
}