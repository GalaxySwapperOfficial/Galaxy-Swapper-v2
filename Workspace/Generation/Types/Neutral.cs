using Galaxy_Swapper_v2.Workspace.Generation.Formats;
using Galaxy_Swapper_v2.Workspace.Properties;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Galaxy_Swapper_v2.Workspace.Generation.Types
{
    public static class Neutral
    {
        public static void Format(Cosmetic Cosmetic, List<Option> Options, JToken Empty, Generate.Type CacheType, params string[] Types)
        {
            var Parse = Cosmetic.Parse;
            var OverrideOptions = new List<Option>();

            if (Parse["OverrideOptions"] != null && (Parse["OverrideOptions"] as JArray).Any())
            {
                foreach (var Override in Parse["OverrideOptions"])
                {
                    switch (Override["Type"].Value<string>())
                    {
                        case "Exception":
                            {
                                var OverrideCosmetic = Generate.Cache[CacheType].Cosmetics[Override["CacheKey"].Value<string>()];
                                OverrideOptions.Add(new Option
                                {
                                    Name = OverrideCosmetic.Name,
                                    ID = OverrideCosmetic.ID,
                                    Parse = OverrideCosmetic.Parse,
                                    Icon = OverrideCosmetic.Icon
                                });
                            }
                            break;
                        case "Override":
                            {
                                var NewOption = new Option
                                {
                                    Name = $"{Override["Name"].Value<string>()} to {Cosmetic.Name}",
                                    ID = Override["ID"].Value<string>(),
                                    OverrideIcon = Cosmetic.Icon,
                                    Parse = null // Not needed we will never use it
                                };

                                if (Override["Message"] != null)
                                    NewOption.Message = Override["Message"].Value<string>();

                                if (!Override["Override"].KeyIsNullOrEmpty())
                                    NewOption.Icon = Override["Override"].Value<string>();
                                else if (!Override["Icon"].KeyIsNullOrEmpty())
                                    NewOption.Icon = Override["Icon"].Value<string>();
                                else
                                    NewOption.Icon = string.Format(Generate.Domain, NewOption.ID);

                                if (!Override["UseMainUEFN"].KeyIsNullOrEmpty())
                                    NewOption.UseMainUEFN = Override["UseMainUEFN"].Value<bool>();

                                NewOption.Nsfw = Cosmetic.Nsfw;
                                NewOption.Socials = Cosmetic.Socials;
                                NewOption.Cosmetic = Cosmetic;

                                if (Override["UEFNTag"] is not null)
                                    NewOption.UEFNTag = Override["UEFNTag"].Value<string>();

                                if (Override["Downloadables"] != null)
                                {
                                    foreach (var downloadable in Override["Downloadables"])
                                    {
                                        if (!downloadable["pak"].KeyIsNullOrEmpty() && !downloadable["sig"].KeyIsNullOrEmpty() && !downloadable["ucas"].KeyIsNullOrEmpty() && !downloadable["utoc"].KeyIsNullOrEmpty())
                                            NewOption.Downloadables.Add(new Downloadable() { Pak = downloadable["pak"].Value<string>(), Sig = downloadable["sig"].Value<string>(), Ucas = downloadable["ucas"].Value<string>(), Utoc = downloadable["utoc"].Value<string>() });
                                    }
                                }

                                foreach (var Asset in Override["Assets"])
                                {
                                    var NewAsset = new Asset() { Object = Asset["AssetPath"].Value<string>() };

                                    if (Asset["AssetPathTo"] != null)
                                        NewAsset.OverrideObject = Asset["AssetPathTo"].Value<string>();

                                    if (Asset["Buffer"] != null)
                                        NewAsset.OverrideBuffer = Asset["Buffer"].Value<string>();

                                    if (Asset["Swaps"] != null)
                                        NewAsset.Swaps = Asset["Swaps"];

                                    if (Asset["Invalidate"] is not null)
                                        NewAsset.Invalidate = Asset["Invalidate"].Value<bool>();

                                    NewOption.Exports.Add(NewAsset);
                                }

                                if (CacheType == Generate.Type.Characters && Settings.Read(Settings.Type.HeroDefinition).Value<bool>() && !Override["HID"].KeyIsNullOrEmpty())
                                {
                                    var NewAsset = new Asset() { Object = Override["HID"]["AssetPath"].Value<string>() };

                                    if (Override["HID"]["AssetPathTo"] != null)
                                        NewAsset.OverrideObject = Override["HID"]["AssetPathTo"].Value<string>();

                                    if (Override["HID"]["Buffer"] != null)
                                        NewAsset.OverrideBuffer = Override["HID"]["Buffer"].Value<string>();

                                    if (Override["HID"]["Swaps"] != null)
                                        NewAsset.Swaps = Override["HID"]["Swaps"];

                                    if (Override["HID"]["Invalidate"] is not null)
                                        NewAsset.Invalidate = Override["HID"]["Invalidate"].Value<bool>();

                                    NewOption.Exports.Add(NewAsset);
                                }

                                Cosmetic.Options.Add(NewOption);
                            }
                            break;
                        default:
                            continue;
                    }
                }
            }

            if (!Parse["UEFNFormat"].KeyIsNullOrEmpty() && Parse["UEFNFormat"].Value<bool>())
            {
                var parse = Endpoint.Read(Endpoint.Type.UEFN);

                foreach (var option in parse["Swaps"])
                {
                    var NewOption = new Option
                    {
                        Name = $"{option["Name"].Value<string>()} to {Cosmetic.Name}",
                        ID = option["ID"].Value<string>(),
                        OverrideIcon = Cosmetic.Icon,
                        UEFNFormat = true,
                        Parse = null // Not needed we will never use it
                    };

                    if (Cosmetic.Options.Exists(option => option.ID == NewOption.ID && option.Name == NewOption.Name))
                    {
                        continue;
                    }

                    if (!option["Override"].KeyIsNullOrEmpty())
                        NewOption.Icon = option["Override"].Value<string>();
                    else if (!option["Icon"].KeyIsNullOrEmpty())
                        NewOption.Icon = option["Icon"].Value<string>();
                    else
                        NewOption.Icon = string.Format(Generate.Domain, NewOption.ID);

                    if (!Parse["Message"].KeyIsNullOrEmpty())
                        NewOption.Message = Parse["Message"].Value<string>();

                    if (!option["Message"].KeyIsNullOrEmpty())
                        NewOption.OptionMessage = option["Message"].Value<string>();

                    NewOption.Nsfw = Cosmetic.Nsfw;
                    NewOption.UseMainUEFN = Cosmetic.UseMainUEFN;
                    NewOption.Socials = Cosmetic.Socials;
                    NewOption.Cosmetic = Cosmetic;
                    NewOption.UEFNTag = Cosmetic.UEFNTag;

                    foreach (var Asset in option["Assets"])
                    {
                        var NewAsset = new Asset() { Object = Asset["AssetPath"].Value<string>() };

                        if (Asset["AssetPathTo"] != null)
                            NewAsset.OverrideObject = Asset["AssetPathTo"].Value<string>();

                        if (Asset["Buffer"] != null)
                            NewAsset.OverrideBuffer = Asset["Buffer"].Value<string>();

                        if (Asset["Swaps"] != null)
                            NewAsset.Swaps = Asset["Swaps"].DeepClone();

                        if (Asset["Invalidate"] is not null)
                            NewAsset.Invalidate = Asset["Invalidate"].Value<bool>();

                        if (!Asset["IsID"].KeyIsNullOrEmpty() && Asset["IsID"].Value<bool>())
                        {
                            if (!Parse["LobbyName"].KeyIsNullOrEmpty() && !option["LobbyName"].KeyIsNullOrEmpty())
                            {
                                string lobbyName = option["LobbyName"].Value<string>();
                                string overrideLobbyName = Parse["LobbyName"].Value<string>();

                                if (lobbyName.Length > overrideLobbyName.Length || lobbyName.Length.Equals(overrideLobbyName.Length))
                                {
                                    ((JArray)NewAsset.Swaps).Add(JObject.FromObject(new
                                    {
                                        type = "hex",
                                        search = Misc.ByteToHex(Encoding.ASCII.GetBytes(lobbyName)),
                                        replace = Generate.CreateNameSwapWithoutAnyLength(overrideLobbyName, lobbyName.Length)
                                    }));
                                }
                            }

                            if (!Parse["Description"].KeyIsNullOrEmpty() && !option["Description"].KeyIsNullOrEmpty())
                            {
                                string description = option["Description"].Value<string>();
                                string overrideDescription = Parse["Description"].Value<string>();

                                if (description.Length > overrideDescription.Length || description.Equals(overrideDescription))
                                {
                                    ((JArray)NewAsset.Swaps).Add(JObject.FromObject(new
                                    {
                                        type = "hex",
                                        search = Misc.ByteToHex(Encoding.ASCII.GetBytes(description)),
                                        replace = Generate.CreateNameSwapWithoutAnyLength(overrideDescription, description.Length)
                                    }));
                                }
                            }

                            if (!Parse["Introduction"].KeyIsNullOrEmpty() && !option["Introduction"].KeyIsNullOrEmpty())
                            {
                                ((JArray)NewAsset.Swaps).Add(JObject.FromObject(new
                                {
                                    type = "tag",
                                    search = option["Introduction"].Value<string>(),
                                    replace = Parse["Introduction"].Value<string>()
                                }));
                            }

                            if (!Parse["Set"].KeyIsNullOrEmpty() && !option["Set"].KeyIsNullOrEmpty())
                            {
                                ((JArray)NewAsset.Swaps).Add(JObject.FromObject(new
                                {
                                    type = "tag",
                                    search = option["Set"].Value<string>(),
                                    replace = Parse["Set"].Value<string>()
                                }));
                            }
                        }

                        NewOption.Exports.Add(NewAsset);
                    }

                    if (Parse["Additional"] is not null)
                    {
                        foreach (var Additional in Parse["Additional"])
                        {
                            var NewAsset = new Asset() { Object = Additional["Object"].Value<string>(), Swaps = Additional["Swaps"] };
                            if (Additional["OverrideObject"] != null)
                                NewAsset.OverrideObject = Additional["OverrideObject"].Value<string>();
                            if (Additional["Buffer"] != null && !string.IsNullOrEmpty(Additional["Buffer"].Value<string>()))
                                NewAsset.OverrideBuffer = Additional["Buffer"].Value<string>();
                            if (Additional["StreamData"] is not null)
                                NewAsset.IsStreamData = Additional["StreamData"].Value<bool>();
                            if (Additional["Invalidate"] is not null)
                                NewAsset.Invalidate = Additional["Invalidate"].Value<bool>();

                            NewOption.Exports.Add(NewAsset);
                        }
                    }

                    if (Settings.Read(Settings.Type.HeroDefinition).Value<bool>() && Parse["HeroDefinition"] is not null && !option["HeroDefinition"].KeyIsNullOrEmpty())
                    {
                        var cid = new Asset() { Object = option["HeroDefinition"].Value<string>(), OverrideObject = Parse["HeroDefinition"]["Object"].Value<string>() };

                        if (Parse["HeroDefinition"]["Swaps"] is not null)
                            cid.Swaps = Parse["HeroDefinition"]["Swaps"];

                        NewOption.Exports.Add(cid);
                    }

                    var newfallback = new Asset() { Object = "/Game/Athena/Heroes/Meshes/Bodies/CP_Athena_Body_F_Fallback", OverrideObject = Parse["Object"].Value<string>(), Swaps = Parse["Swaps"] };

                    Generate.AddMaterialOverridesArray(Parse, newfallback);
                    Generate.AddTextureParametersArray(Parse, newfallback);

                    NewOption.Exports.Add(newfallback);
                    Cosmetic.Options.Add(NewOption);
                }

                if (Parse["Objects"] is null)
                    return;
            }

            var BlackListed = new List<string>();

            if (Parse["BlackList"] != null && (Parse["BlackList"] as JArray).Any())
                BlackListed = (Parse["BlackList"] as JArray).ToObject<List<string>>();

            if (CacheType == Generate.Type.Characters && !BlackListed.Contains("Default"))
            {
                //Defaults!
            }

            if (Parse["UseOptions"] != null && !Parse["UseOptions"].Value<bool>())
                return;

            foreach (var Option in Options.Concat(OverrideOptions))
            {
                var OParse = Option.Parse;
                bool Continue = false;

                if (BlackListed.Contains($"{Option.Name}:{Option.ID}") || $"{Cosmetic.Name}:{Cosmetic.ID}" == $"{Option.Name}:{Option.ID}")
                    continue;

                foreach (string Type in Types)
                {
                    if (OParse[Type] != null && OParse[Type].Value<string>() != Parse[Type].Value<string>() && OParse[Type].Value<string>() != "Any" && Parse[Type].Value<string>() != "Any")
                    {
                        Continue = true;
                    }
                }

                if (Continue)
                    continue;

                var NewOption = (Option)Option.Clone();
                var Objects = OParse["Objects"].ToDictionary(obj => obj["Type"].Value<string>(), obj => obj["Object"].Value<string>());
                
                NewOption.Exports = new List<Asset>();

                foreach (var Object in Parse["Objects"])
                {
                    Asset NewAsset = null;

                    if (Objects.ContainsKey(Object["Type"].Value<string>()))
                    {
                        string ObjectPath = Objects[Object["Type"].Value<string>()];
                        string OverrideObjectPath = Object["Object"].Value<string>();

                        NewAsset = new Asset() { Object = ObjectPath, OverrideObject = OverrideObjectPath };
                        Objects.Remove(Object["Type"].Value<string>());
                    }
                    else if (Object["Exceptions"] != null && ((JArray)Object["Exceptions"]).ToObject<List<string>>().Intersect(Objects.Keys).Any())
                    {
                        string Exception = ((JArray)Object["Exceptions"]).ToObject<List<string>>().Intersect(Objects.Keys).First();
                        string ObjectPath = Objects[Exception];
                        string OverrideObjectPath = Object["Object"].Value<string>();

                        NewAsset = new Asset() { Object = ObjectPath, OverrideObject = OverrideObjectPath };
                        Objects.Remove(Exception);
                    }
                    else
                    {
                        Continue = true;
                        break;
                    }

                    Generate.AddMaterialOverridesArray(Object, NewAsset);
                    Generate.AddTextureParametersArray(Object, NewAsset);

                    if (Object["Buffer"] != null && !string.IsNullOrEmpty(Object["Buffer"].Value<string>()))
                        NewAsset.OverrideBuffer = Object["Buffer"].Value<string>();

                    NewAsset.Swaps = Object["Swaps"];
                    NewOption.Exports.Add(NewAsset);
                }

                if (Continue)
                    continue;

                if (Cosmetic.Downloadables != null && Cosmetic.Downloadables.Count > 0)
                    NewOption.Downloadables = Cosmetic.Downloadables;

                if (Objects.Count != 0)
                {
                    foreach (var Object in Objects)
                    {
                        NewOption.Exports.Add(new Asset() { Object = Object.Value, OverrideObject = Empty[Object.Key].Value<string>() });
                    }
                }

                if (Parse["Additional"] != null)
                {
                    foreach (var Additional in Parse["Additional"])
                    {
                        var NewAsset = new Asset() { Object = Additional["Object"].Value<string>(), Swaps = Additional["Swaps"] };
                        if (Additional["OverrideObject"] != null)
                            NewAsset.OverrideObject = Additional["OverrideObject"].Value<string>();
                        if (Additional["Buffer"] != null && !string.IsNullOrEmpty(Additional["Buffer"].Value<string>()))
                            NewAsset.OverrideBuffer = Additional["Buffer"].Value<string>();
                        if (Additional["StreamData"] is not null)
                            NewAsset.IsStreamData = Additional["StreamData"].Value<bool>();

                        Generate.AddMaterialOverridesArray(Additional, NewAsset);
                        Generate.AddTextureParametersArray(Additional, NewAsset);

                        NewOption.Exports.Add(NewAsset);
                    }
                }

                if (Settings.Read(Settings.Type.HeroDefinition).Value<bool>() && Parse["HeroDefinition"] is not null && !OParse["HeroDefinition"].KeyIsNullOrEmpty())
                {
                    var cid = new Asset() { Object = OParse["HeroDefinition"]["Object"].Value<string>(), OverrideObject = Parse["HeroDefinition"]["Object"].Value<string>() };

                    if (Parse["HeroDefinition"]["Swaps"] is not null)
                        cid.Swaps = Parse["HeroDefinition"]["Swaps"];

                    NewOption.Exports.Add(cid);
                }

                NewOption.Message = Cosmetic.Message;
                NewOption.Name = $"{Option.Name} to {Cosmetic.Name}";
                NewOption.OverrideIcon = Cosmetic.Icon;
                NewOption.Nsfw = Cosmetic.Nsfw;
                NewOption.UseMainUEFN = Cosmetic.UseMainUEFN;
                NewOption.UEFNTag = Cosmetic.UEFNTag;
                NewOption.Socials = Cosmetic.Socials;
                NewOption.Cosmetic = Cosmetic;

                Cosmetic.Options.Add(NewOption);
            }
        }
    }
}