using Galaxy_Swapper_v2.Workspace.Generation.Formats;
using Galaxy_Swapper_v2.Workspace.Generation.Types;
using Galaxy_Swapper_v2.Workspace.Properties;
using Galaxy_Swapper_v2.Workspace.Structs;
using Galaxy_Swapper_v2.Workspace.Swapping.Other;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

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
                Dictionary<string, int> stats;

                if (parseStats is null)
                {
                    stats = new Dictionary<string, int>();
                }
                else stats = parseStats[Type.ToString()].ToDictionary(c => c["key"].Value<string>(), c => c["count"].Value<int>());

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

                            if (!Cosmetic["UEFNTag"].KeyIsNullOrEmpty())
                                NewCosmetic.UEFNTag = Cosmetic["UEFNTag"].Value<string>();

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

        public static void AddMaterialOverridesArray(JToken parse, Asset asset)
        {
            if (parse["MaterialOverridesArray"] is not null)
            {
                asset.MaterialData = new();

                if (parse["MaterialOverridesArray"]["Search"].KeyIsNullOrEmpty())
                {
                    asset.MaterialData.Offset = parse["MaterialOverridesArray"]["Offset"].Value<long>();
                }
                else
                {
                    asset.MaterialData.SearchBuffer = parse["MaterialOverridesArray"]["Search"].Value<string>().HexToByte();
                }

                foreach (var material in parse["MaterialOverridesArray"]["Materials"])
                {
                    asset.MaterialData.Materials.Add(new() { Material = material["OverrideMaterial"].Value<string>(), MaterialOverrideIndex = material["MaterialOverrideIndex"].Value<int>() });
                }

                asset.MaterialData.MaterialOverrideFlags = new() { MaterialOverrideFlags = parse["MaterialOverridesArray"]["MaterialOverrideFlags"]["Index"].Value<int>() };

                if (parse["MaterialOverridesArray"]["MaterialOverrideFlags"]["Search"].KeyIsNullOrEmpty())
                {
                    asset.MaterialData.MaterialOverrideFlags.Offset = parse["MaterialOverridesArray"]["MaterialOverrideFlags"]["Offset"].Value<long>();
                }
                else
                {
                    asset.MaterialData.MaterialOverrideFlags.SearchBuffer = parse["MaterialOverridesArray"]["MaterialOverrideFlags"]["Search"].Value<string>().HexToByte();
                }
            }
        }

        public static void AddTextureParametersArray(JToken parse, Asset asset)
        {
            if (parse["TextureParametersArray"] is not null)
            {
                asset.TextureData = new();

                if (parse["TextureParametersArray"]["Search"].KeyIsNullOrEmpty())
                {
                    asset.TextureData.Offset = parse["TextureParametersArray"]["Offset"].Value<long>();
                }
                else
                {
                    asset.TextureData.SearchBuffer = parse["TextureParametersArray"]["Search"].Value<string>().HexToByte();
                }

                foreach (var texture in parse["TextureParametersArray"]["Textures"])
                {
                    asset.TextureData.TextureParameters.Add(new() { TextureOverride = texture["TextureOverride"].Value<string>(), TextureParameterNameForMaterial = texture["TextureParameterNameForMaterial"].Value<string>(), MaterialIndexForTextureParameter = texture["MaterialIndexForTextureParameter"].Value<int>() });
                }
            }
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

        public static string CreateNameSwap(string name)
        {
            var writer = new Writer(new byte[sizeof(int) + name.Length + 1]);

            writer.Write<int>(name.Length + 1);
            writer.WriteBytes(Encoding.ASCII.GetBytes(name));
            writer.WriteByte(0);

            return Misc.ByteToHex(writer.ToByteArray(writer.Position));
        }

        public static string CreateNameSwapWithoutAnyLength(string name, int length)
        {
            var writer = new Writer(new byte[length]);

            writer.WriteBytes(Encoding.ASCII.GetBytes(name));

            for (int i = 0; i < length - name.Length; i++)
            {
                writer.WriteByte(0x20);
            }

            return Misc.ByteToHex(writer.ToByteArray(writer.Position));
        }
    }
}