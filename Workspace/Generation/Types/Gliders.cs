using Galaxy_Swapper_v2.Workspace.Generation.Formats;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Galaxy_Swapper_v2.Workspace.Generation.Types
{
    public static class Gliders
    {
        public static void Format(Cosmetic Cosmetic, List<Option> Options)
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
                                var OverrideCosmetic = Generate.Cache[Generate.Type.Gliders].Cosmetics[Override["CacheKey"].Value<string>()];
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
                                    Message = Cosmetic.Message,
                                    Cosmetic = Cosmetic,
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

                                if (!Override["UEFNTag"].KeyIsNullOrEmpty())
                                    NewOption.UEFNTag = Override["UEFNTag"].Value<string>();

                                if (!Override["Nsfw"].KeyIsNullOrEmpty())
                                    NewOption.Nsfw = Override["Nsfw"].Value<bool>();

                                NewOption.Socials = Cosmetic.Socials;

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

            var BlackListed = new List<string>();

            if (Parse["BlackList"] != null && (Parse["BlackList"] as JArray).Any())
                BlackListed = (Parse["BlackList"] as JArray).ToObject<List<string>>();

            if (Parse["UseOptions"] != null && !Parse["UseOptions"].Value<bool>())
                return;

            foreach (var Option in Options.Concat(OverrideOptions))
            {
                var NewOption = (Option)Option.Clone();
                var OParse = Option.Parse;

                if (BlackListed.Contains($"{Option.Name}:{Option.ID}") || $"{Cosmetic.Name}:{Cosmetic.ID}" == $"{Option.Name}:{Option.ID}")
                    continue;

                if (Parse["SkeletalMesh"].Value<string>() != OParse["SkeletalMesh"].Value<string>())
                    continue;

                string Object = OParse["AthenaGliderItemDefinition"].Value<string>();
                string OverrideObject = Parse["AthenaGliderItemDefinition"].Value<string>();

                var NewAsset = new Asset() { Object = Object, OverrideObject = OverrideObject, Swaps = Parse["Swaps"] };

                if (Parse["Buffer"] != null && !string.IsNullOrEmpty(Parse["Buffer"].Value<string>()))
                    NewAsset.OverrideBuffer = Parse["Buffer"].Value<string>();

                Generate.AddMaterialOverridesArray(Parse, NewAsset);
                Generate.AddTextureParametersArray(Parse, NewAsset);

                NewOption.Exports.Add(NewAsset);

                if (Parse["Additional"] != null)
                {
                    foreach (var Additional in Parse["Additional"])
                    {
                        var NewAdditionalAsset = new Asset() { Object = Additional["Object"].Value<string>(), Swaps = Additional["Swaps"] };
                        if (Additional["OverrideObject"] != null)
                            NewAdditionalAsset.OverrideObject = Additional["OverrideObject"].Value<string>();
                        if (Additional["Buffer"] != null && !string.IsNullOrEmpty(Additional["Buffer"].Value<string>()))
                            NewAdditionalAsset.OverrideBuffer = Additional["Buffer"].Value<string>();

                        Generate.AddMaterialOverridesArray(Additional, NewAdditionalAsset);
                        Generate.AddTextureParametersArray(Additional, NewAdditionalAsset);

                        NewOption.Exports.Add(NewAdditionalAsset);
                    }
                }

                if (Cosmetic.Downloadables != null && Cosmetic.Downloadables.Count > 0)
                    NewOption.Downloadables = Cosmetic.Downloadables;

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