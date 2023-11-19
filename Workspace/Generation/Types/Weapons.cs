using Galaxy_Swapper_v2.Workspace.Generation.Formats;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;

namespace Galaxy_Swapper_v2.Workspace.Generation.Types
{
    public static class Weapons
    {
        public static void Format(Cosmetic Cosmetic)
        {
            var Parse = Cosmetic.Parse;

            foreach (var Option in Parse["Options"])
            {
                var NewOption = new Option() { Name = $"{Option["Name"].Value<string>()} to {Cosmetic.Name}", Message = Cosmetic.Message, OverrideIcon = Cosmetic.Icon };

                if (Option["Message"] != null)
                    NewOption.OptionMessage = Option["Message"].Value<string>();
                if (!Option["Override"].KeyIsNullOrEmpty())
                    NewOption.Icon = Option["Override"].Value<string>();
                else if (!Option["Icon"].KeyIsNullOrEmpty())
                    NewOption.Icon = Option["Icon"].Value<string>();
                else
                    NewOption.Icon = string.Format(Generate.Domain, NewOption.ID);

                foreach (var Asset in Option["Assets"])
                {
                    var NewAsset = new Asset();

                    if (Asset["AssetPathTo"] != null)
                        NewAsset.OverrideObject = Asset["AssetPathTo"].Value<string>();

                    if (Asset["Buffer"] != null)
                        NewAsset.OverrideBuffer = Asset["Buffer"].Value<string>();

                    if (Asset["Swaps"] != null)
                        NewAsset.Swaps = Asset["Swaps"];

                    if (Asset["AssetPath"] is JArray)
                    {
                        foreach (string AssetPath in Asset["AssetPath"])
                        {
                            NewAsset.Object = AssetPath;
                            NewOption.Exports.Add(NewAsset);

                            NewAsset = (Asset)NewAsset.Clone();
                        }
                    }
                    else
                    {
                        NewAsset.Object = Option["AssetPath"].Value<string>();
                        NewOption.Exports.Add(NewAsset);
                    }
                }

                if (Cosmetic.Downloadables != null && Cosmetic.Downloadables.Count > 0)
                    NewOption.Downloadables = Cosmetic.Downloadables;

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