using Galaxy_Swapper_v2.Workspace.Generation.Formats;
using Galaxy_Swapper_v2.Workspace.Generation.Types;
using Galaxy_Swapper_v2.Workspace.Properties;
using Galaxy_Swapper_v2.Workspace.Structs;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Galaxy_Swapper_v2.Workspace.Generation
{
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
                var parseStats = Endpoint.Read(Endpoint.Type.Stats);
                var stats = parseStats[Type.ToString()].ToDictionary(c => c["key"].Value<string>(), c => c["count"].Value<int>());

                var Stopwatch = new Stopwatch();
                Stopwatch.Start();

                if (string.IsNullOrEmpty(Domain))
                    Domain = Parse["Low"].Value<string>();

                Parse = Parse[Type.ToString()];

                var Array = Parse["Array"];
                var NewCache = new Frontend();

                if (!Parse["Empty"].KeyIsNullOrEmpty())
                    NewCache.Empty = Parse["Empty"];

                bool HideNsfw = Settings.Read(Settings.Type.HideNsfw).Value<bool>();

                if (Array.Count() != 0)
                {
                    foreach (var Cosmetic in Array)
                    {
                        if (Cosmetic["Frontend"].Value<bool>())
                        {
                            var NewCosmetic = new Cosmetic() { Name = Cosmetic["Name"].Value<string>(), ID = Cosmetic["ID"].Value<string>(), Parse = Cosmetic, Type = Type };

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

                            if (!Cosmetic["UseMainUEFN"].KeyIsNullOrEmpty())
                                NewCosmetic.UseMainUEFN = Cosmetic["UseMainUEFN"].Value<bool>();

                            if (HideNsfw && NewCosmetic.Nsfw)
                                continue;

                            if (Cosmetic["Downloadables"] != null)
                            {
                                foreach (var downloadable in Cosmetic["Downloadables"])
                                {
                                    if (!downloadable["pak"].KeyIsNullOrEmpty() && !downloadable["sig"].KeyIsNullOrEmpty() && !downloadable["ucas"].KeyIsNullOrEmpty() && !downloadable["utoc"].KeyIsNullOrEmpty())
                                        NewCosmetic.Downloadables.Add(new Downloadable() { Pak = downloadable["pak"].Value<string>(), Sig = downloadable["sig"].Value<string>(), Ucas = downloadable["ucas"].Value<string>(), Utoc = downloadable["utoc"].Value<string>() });
                                }
                            }

                            if (Cosmetic["Socials"] is not null)
                            {
                                var sparse = Endpoint.Read(Endpoint.Type.Socials);
                                foreach (var social in Cosmetic["Socials"])
                                {
                                    string type = social["type"].Value<string>().ToUpper();

                                    if (sparse[type] is null)
                                        continue;

                                    var newsocial = new Social() { Icon = sparse[type]["Icon"].Value<string>() };

                                    if (!social["header"].KeyIsNullOrEmpty())
                                    {
                                        newsocial.Header = social["header"].Value<string>();
                                    }

                                    if (!social["url"].KeyIsNullOrEmpty())
                                    {
                                        newsocial.URL = social["url"].Value<string>();
                                    }

                                    NewCosmetic.Socials.Add(newsocial);
                                }
                            }

                            string key = $"{NewCosmetic.Name}:{NewCosmetic.ID}";

                            if (stats.ContainsKey(key))
                            {
                                NewCosmetic.Stats = stats[key];
                            }

                            NewCache.Cosmetics.Add(key, NewCosmetic);
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

                if (Settings.Read(Settings.Type.SortByStats).Value<bool>())
                {
                    NewCache.Cosmetics = NewCache.Cosmetics.OrderByDescending(pair => pair.Value.Stats).ToDictionary(pair => pair.Key, pair => pair.Value);
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
                        Neutral.Format(Cache[Type].Cosmetics[Key], Cache[Type].Options, Cache[Type].Empty, Type, "Gender");
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