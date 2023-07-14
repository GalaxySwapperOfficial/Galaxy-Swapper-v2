using Galaxy_Swapper_v2.Workspace.Usercontrols;
using Galaxy_Swapper_v2.Workspace.Usercontrols.Overlays;
using Galaxy_Swapper_v2.Workspace.Views;
using System.Windows.Controls;

namespace Galaxy_Swapper_v2.Workspace.Utilities
{
    public static class Memory
    {
        public static MainView MainView { get; set; } = default!;
        public static DashboardView DashboardView = DashboardView ?? new DashboardView();
        public static SettingsView SettingsView = SettingsView ?? new SettingsView();
        public static NotesView NotesView = NotesView ?? new NotesView();
        public static NoOptionsView NoOptionsView = NoOptionsView ?? new NoOptionsView();
        public static PluginsView PluginsView = PluginsView ?? new PluginsView();
        public static MiscView MiscView = MiscView ?? new MiscView();
        public static FovView FovView = FovView ?? new FovView();

        private static CosmeticsView Characters = Characters ?? new CosmeticsView(Generation.Generate.Type.Characters);
        public static CosmeticsView LoadCharacters(TextBox SearchBar)
        {
            Characters.SetSearchBar(SearchBar);
            return Characters;
        }

        private static CosmeticsView Backpacks = Backpacks ?? new CosmeticsView(Generation.Generate.Type.Backpacks);
        public static CosmeticsView LoadBackpacks(TextBox SearchBar)
        {
            Backpacks.SetSearchBar(SearchBar);
            return Backpacks;
        }

        private static CosmeticsView Pickaxes = Pickaxes ?? new CosmeticsView(Generation.Generate.Type.Pickaxes);
        public static CosmeticsView LoadPickaxes(TextBox SearchBar)
        {
            Pickaxes.SetSearchBar(SearchBar);
            return Pickaxes;
        }

        private static CosmeticsView Dances = Dances ?? new CosmeticsView(Generation.Generate.Type.Dances);
        public static CosmeticsView LoadDances(TextBox SearchBar)
        {
            Dances.SetSearchBar(SearchBar);
            return Dances;
        }

        private static CosmeticsView Gliders = Gliders ?? new CosmeticsView(Generation.Generate.Type.Gliders);
        public static CosmeticsView LoadGliders(TextBox SearchBar)
        {
            Gliders.SetSearchBar(SearchBar);
            return Gliders;
        }

        private static CosmeticsView Weapons = Weapons ?? new CosmeticsView(Generation.Generate.Type.Weapons);
        public static CosmeticsView LoadWeapons(TextBox SearchBar)
        {
            Weapons.SetSearchBar(SearchBar);
            return Weapons;
        }

        public static void Clear(bool All = true)
        {
            Characters = new CosmeticsView(Generation.Generate.Type.Characters);
            Backpacks = new CosmeticsView(Generation.Generate.Type.Backpacks);
            Pickaxes = new CosmeticsView(Generation.Generate.Type.Pickaxes);
            Dances = new CosmeticsView(Generation.Generate.Type.Dances);
            Weapons = new CosmeticsView(Generation.Generate.Type.Weapons);
            Gliders = new CosmeticsView(Generation.Generate.Type.Gliders);

            if (All)
            {
                DashboardView = new DashboardView();
                SettingsView = new SettingsView();
                NotesView = new NotesView();
                NoOptionsView = new NoOptionsView();
            }
        }
    }
}