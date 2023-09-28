using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Galaxy_Swapper_v2.Workspace.Properties
{
    public static class SwapLogs
    {
        public static JArray Cache { get; set; } = default!;
        public static readonly string Path = $"{App.Config}\\SwapLogs.json";
        public static void Initialize()
        {
            try
            {
                if (Cache != null)
                    return;

                if (!File.Exists(Path))
                {
                    Log.Information($"{Path} Does not exist populating cache with empty array");
                    Cache = new JArray();
                    return;
                }

                string Content = File.ReadAllText(Path);

                if (!Content.ValidArray())
                {
                    Log.Information($"{Path} Is not in a valid json format populating cache with empty array");
                    Cache = new JArray();
                    return;
                }

                Cache = JArray.Parse(Content);

                Log.Information("Successfully initialized swaplogs");
            }
            catch (Exception Exception)
            {
                Log.Error(Exception.Message, $"Caught a error while initializing cache loading with empty array");
                Cache = new JArray();
            }
        }

        public static void Add(string Name, string Icon, string OverrideIcon, int AssetCount, List<string> UcasList, List<string> UtocList, bool UEFNFormat = false)
        {
            DateTime CurrentTime = DateTime.Now;
            var Object = JObject.FromObject(new
            {
                Name = Name,
                Icon = Icon,
                OverrideIcon = OverrideIcon,
                AssetCount = AssetCount,
                SwappedAt = CurrentTime.ToString("h:mm tt"),
                Ucas = new JArray(UcasList),
                Utoc = new JArray(UtocList),
                UEFNFormat
            });

            Cache.Add(Object);

            File.WriteAllText(Path, Cache.ToString());
        }

        public static void Remove(string Name)
        {
            var NewCache = new JArray();
            foreach (var Cosmetic in Cache)
            {
                if (Cosmetic["Name"].Value<string>() != Name)
                    NewCache.Add(Cosmetic);
            }

            Cache = NewCache;
            File.WriteAllText(Path, Cache.ToString());
        }

        public static void Remove(string Name, bool Startswith)
        {
            var NewCache = new JArray();
            foreach (var Cosmetic in Cache)
            {
                if (!Cosmetic["Name"].Value<string>().StartsWith(Name))
                    NewCache.Add(Cosmetic);
            }

            Cache = NewCache;
            File.WriteAllText(Path, Cache.ToString());
        }

        public static void Read(out int Count, out int AssetCount, out int Ucas, out int Utoc)
        {
            Count = Cache.Count();
            AssetCount = 0;
            var UcasList = new List<string>();
            var UtocList = new List<string>();

            foreach (var Cosmetic in Cache)
            {
                foreach (string ucas in Cosmetic["Ucas"])
                {
                    if (!UcasList.Contains(ucas))
                        UcasList.Add(ucas);
                }
                foreach (string utoc in Cosmetic["Utoc"])
                {
                    if (!UtocList.Contains(utoc))
                        UtocList.Add(utoc);
                }

                AssetCount += Cosmetic["AssetCount"].Value<int>();
            }

            Ucas = UcasList.Count();
            Utoc = UtocList.Count();
        }

        public static void Kick(out bool UcasK, out bool UtocK, List<string> Ucas, List<string> Utoc)
        {
            UcasK = false;
            UtocK = false;

            var UcasList = new List<string>(Ucas);
            var UtocList = new List<string>(Utoc);

            foreach (var Cosmetic in Cache)
            {
                foreach (string ucas in Cosmetic["Ucas"])
                {
                    if (!UcasList.Contains(ucas))
                        UcasList.Add(ucas);
                }
                foreach (string utoc in Cosmetic["Utoc"])
                {
                    if (!UtocList.Contains(utoc))
                        UtocList.Add(utoc);
                }
            }

            if (UcasList.Count > 2)
                UcasK = true;
            if (UtocList.Count > 2)
                UtocK = true;
        }

        public static void Clear()
        {
            Cache = new JArray();
            File.WriteAllText(Path, Cache.ToString());
        }

        public static bool IsSwapped(string Name)
        {
            foreach (var Cosmetic in Cache)
            {
                if (Cosmetic["Name"].Value<string>() == Name)
                    return true;
            }

            return false;
        }

        public static bool IsSwappedUEFNSwapped(string Name, out string revertitem)
        {
            foreach (var Cosmetic in Cache)
            {
                if (Cosmetic["Name"].Value<string>() != Name && !Cosmetic["UEFNFormat"].KeyIsNullOrEmpty() && Cosmetic["UEFNFormat"].Value<bool>())
                {
                    revertitem = Cosmetic["Name"].Value<string>();
                    return true;
                }
            }

            revertitem = null;
            return false;
        }

        public static bool IsSwapped(string Name, bool Startswith)
        {
            foreach (var Cosmetic in Cache)
            {
                if (Cosmetic["Name"].Value<string>().StartsWith(Name))
                    return true;
            }

            return false;
        }
    }
}
