using Galaxy_Swapper_v2.Workspace.CProvider;
using Galaxy_Swapper_v2.Workspace.Generation;
using Galaxy_Swapper_v2.Workspace.Properties;
using Galaxy_Swapper_v2.Workspace.Swapping.Other;
using Galaxy_Swapper_v2.Workspace.Swapping.Providers;
using Galaxy_Swapper_v2.Workspace.Usercontrols.Overlays;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WindowsAPICodePack.Dialogs;

namespace Galaxy_Swapper_v2.Workspace.Usercontrols
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private bool IsLoaded = false;
        public void SettingsView_Loaded(object sender, RoutedEventArgs e)
        {
            SwapLogs.Read(out int Count, out int AssetCount, out int Ucas, out int Utoc);

            InstallationHeader.Text = Languages.Read(Languages.Type.View, "SettingsView", "InstallationHeader");
            EpicInstallationHeader.Text = Languages.Read(Languages.Type.View, "SettingsView", "EpicInstallationHeader");
            SwapLogsHeader.Text = Languages.Read(Languages.Type.View, "SettingsView", "SwapLogsHeader");
            SwapLogsDescription.Text = string.Format(Languages.Read(Languages.Type.View, "SettingsView", "SwapLogsDescription"), Count, Ucas, Utoc);
            ShortCutsHeader.Text = Languages.Read(Languages.Type.View, "SettingsView", "ShortCutsHeader");
            ShortCutsDescription.Text = Languages.Read(Languages.Type.View, "SettingsView", "ShortCutsDescription");
            RefreshCosmeticsHeader.Text = Languages.Read(Languages.Type.View, "SettingsView", "RefreshCosmeticsHeader");
            RefreshCosmeticsDescription.Text = Languages.Read(Languages.Type.View, "SettingsView", "RefreshCosmeticsDescription");
            LanguageHeader.Text = Languages.Read(Languages.Type.View, "SettingsView", "LanguageHeader");
            LanguageDescription.Text = Languages.Read(Languages.Type.View, "SettingsView", "LanguageDescription");
            DiscordRichPresenceHeader.Text = Languages.Read(Languages.Type.View, "SettingsView", "DiscordRichPresenceHeader");
            DiscordRichPresenceDescription.Text = Languages.Read(Languages.Type.View, "SettingsView", "DiscordRichPresenceDescription");
            AutoCloseFortniteHeader.Text = Languages.Read(Languages.Type.View, "SettingsView", "AutoCloseFortniteHeader");
            AutoCloseFortniteDescription.Text = Languages.Read(Languages.Type.View, "SettingsView", "AutoCloseFortniteDescription");
            KickWarningHeader.Text = Languages.Read(Languages.Type.View, "SettingsView", "KickWarningHeader");
            KickWarningDescription.Text = Languages.Read(Languages.Type.View, "SettingsView", "KickWarningDescription");
            BackPackGenderHeader.Text = Languages.Read(Languages.Type.View, "SettingsView", "BackPackGenderHeader");
            BackPackGenderDescription.Text = Languages.Read(Languages.Type.View, "SettingsView", "BackPackGenderDescription");
            NsfwHeader.Text = Languages.Read(Languages.Type.View, "SettingsView", "NsfwHeader");
            NsfwDescription.Text = Languages.Read(Languages.Type.View, "SettingsView", "NsfwDescription");
            ShareStatsHeader.Text = Languages.Read(Languages.Type.View, "SettingsView", "ShareStatsHeader");
            ShareStatsDescription.Text = Languages.Read(Languages.Type.View, "SettingsView", "ShareStatsDescription");
            SortByStatsHeader.Text = Languages.Read(Languages.Type.View, "SettingsView", "SortByStatsHeader");
            SortByStatsDescription.Text = Languages.Read(Languages.Type.View, "SettingsView", "SortByStatsDescription");
            HeroDefinitionHeader.Text = Languages.Read(Languages.Type.View, "SettingsView", "HeroDefinitionHeader");
            HeroDefinitionDescription.Text = Languages.Read(Languages.Type.View, "SettingsView", "HeroDefinitionDescription");

            //Buttons
            EditPath.Content = Languages.Read(Languages.Type.View, "SettingsView", "EditPath");
            EpicEditPath.Content = Languages.Read(Languages.Type.View, "SettingsView", "EpicEditPath");
            Verify.Content = Languages.Read(Languages.Type.View, "SettingsView", "Verify");
            Show.Content = Languages.Read(Languages.Type.View, "SettingsView", "Show");
            Reset.Content = Languages.Read(Languages.Type.View, "SettingsView", "Reset");
            RevertAllCosmetics.Content = Languages.Read(Languages.Type.View, "SettingsView", "RevertAllCosmetics");
            StartFortnite.Content = Languages.Read(Languages.Type.View, "SettingsView", "StartFortnite");
            CloseFortnite.Content = Languages.Read(Languages.Type.View, "SettingsView", "CloseFortnite");
            RefreshCosmetics.Content = Languages.Read(Languages.Type.View, "SettingsView", "RefreshCosmetics");
            LanguageSelection.Content = Languages.Read(Languages.Type.View, "SettingsView", "LanguageSelection");


            if (IsLoaded)
                return;

            InstallationDescription.Text = Settings.Read(Settings.Type.Installtion).Value<string>();
            EpicInstallationDescription.Text = Settings.Read(Settings.Type.EpicInstalltion).Value<string>();

            if (Settings.Read(Settings.Type.RichPresence).Value<bool>())
                DiscordRichPresence.IsChecked = true;
            if (Settings.Read(Settings.Type.CloseFortnite).Value<bool>())
                AutoCloseFortnite.IsChecked = true;
            if (Settings.Read(Settings.Type.KickWarning).Value<bool>())
                KickWarning.IsChecked = true;
            if (Settings.Read(Settings.Type.BackpackGender).Value<bool>())
                BackPackGender.IsChecked = true;
            if (Settings.Read(Settings.Type.HeroDefinition).Value<bool>())
                HeroDefinition.IsChecked = true;
            if (Settings.Read(Settings.Type.HideNsfw).Value<bool>())
                Nsfw.IsChecked = true;
            if (Settings.Read(Settings.Type.ShareStats).Value<bool>())
                ShareStats.IsChecked = true;
            if (Settings.Read(Settings.Type.SortByStats).Value<bool>())
                SortByStats.IsChecked = true;

            IsLoaded = true;
        }

        private void ShowConverted_Click(object sender, RoutedEventArgs e) => Memory.MainView.SetOverlay(new SwappedView());

        private void EditPath_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new CommonOpenFileDialog() { IsFolderPicker = true, Title = Languages.Read(Languages.Type.View, "SettingsView", "EditPathTip") })
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    string selected = dialog.FileName;

                    if (selected.Contains("\\FortniteGame"))
                        selected = selected.Split("\\FortniteGame").First();

                    if (!Directory.Exists($"{selected}\\FortniteGame\\Content\\Paks"))
                    {
                        Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Error"), Languages.Read(Languages.Type.Message, "InvalidInstallationSelection"), MessageBoxButton.OK);
                        return;
                    }

                    Settings.Edit(Settings.Type.Installtion, selected);
                    InstallationDescription.Text = Settings.Read(Settings.Type.Installtion).Value<string>();
                }
            }
        }

        private void EpicEditPath_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new CommonOpenFileDialog() { IsFolderPicker = true, Title = Languages.Read(Languages.Type.View, "SettingsView", "EpicEditPathTip") })
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    string selected = dialog.FileName;

                    if (selected.Contains("\\Epic Games"))
                        selected = selected.Split("\\Epic Games").First();

                    if (!Directory.Exists($"{selected}\\Epic Games\\Launcher\\Portal\\Binaries\\Win64") || !File.Exists($"{selected}\\Epic Games\\Launcher\\Portal\\Binaries\\Win64\\EpicGamesLauncher.exe"))
                    {
                        Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Error"), Languages.Read(Languages.Type.Message, "InvalidEpicInstallationSelection"), MessageBoxButton.OK);
                        return;
                    }

                    Settings.Edit(Settings.Type.EpicInstalltion, $"{selected}\\Epic Games\\Launcher\\Portal\\Binaries\\Win64\\EpicGamesLauncher.exe");
                    EpicInstallationDescription.Text = Settings.Read(Settings.Type.EpicInstalltion).Value<string>();
                }
            }
        }

        private void Verify_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string Installtion = $"{Settings.Read(Settings.Type.Installtion).Value<string>()}\\FortniteGame\\Content\\Paks";

                EpicGamesLauncher.Close();
                CProviderManager.DefaultProvider?.Dispose();
                CProviderManager.DefaultProvider = null!;
                CProviderManager.UEFNProvider?.Dispose();
                CProviderManager.UEFNProvider = null!;

                Log.Information("Scanning for unknown game files");
                foreach (string Unkown in Directory.GetDirectories(Installtion))
                {
                    Log.Information($"Found directory that contains game files: {Unkown}");
                    if (Directory.GetFiles(Unkown, "*.ucas").Length != 0 || Directory.GetFiles(Unkown, "*.pak").Length != 0)
                    {
                        Directory.Delete(Unkown, true);
                        Log.Information($"Deleted {Unkown}");
                    }
                    else
                        Log.Information($"Skipped {Unkown} (Does not contain ucas or sig)");
                }

                foreach (var iostore in new DirectoryInfo(Installtion).GetFiles())
                {
                    var ext = iostore.Extension.SubstringAfter('.');

                    if (ext != "backup")
                        continue;

                    Log.Information($"Deleting: {iostore.FullName}");
                    File.Delete(iostore.FullName);
                }

                CustomEpicGamesLauncher.Revert();
                SwapLogs.Clear();
                UEFN.Clear(Installtion);

                Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"), Languages.Read(Languages.Type.Message, "Verify"), MessageBoxButton.OK);
                EpicGamesLauncher.Verify();
                Environment.Exit(0);
            }
            catch (Exception Exception)
            {
                Log.Error(Exception, $"Failed to verify game files");
                Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Error"), Languages.Read(Languages.Type.Message, "VerifyError"), MessageBoxButton.OK, solutions: Languages.ReadSolutions(Languages.Type.Message, "RemoveError"));
                return;
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            if (Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Question"), Languages.Read(Languages.Type.Message, "ResetSwaps"), MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                SwapLogs.Clear();
                SwapLogsDescription.Text = string.Format(Languages.Read(Languages.Type.View, "SettingsView", "SwapLogsDescription"), 0, 0, 0);
            }
        }

        private void RevertAllCosmetics_Click(object sender, RoutedEventArgs e)
        {
            if (Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Question"), Languages.Read(Languages.Type.Message, "RevertAllCosmetics"), MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    string paks = $"{Settings.Read(Settings.Type.Installtion).Value<string>()}\\FortniteGame\\Content\\Paks";
                    var directoryInfo = new DirectoryInfo(paks);

                    //So files aren't in use.
                    EpicGamesLauncher.Close();
                    CProviderManager.DefaultProvider?.Dispose();
                    CProviderManager.DefaultProvider = null!;
                    CProviderManager.UEFNProvider?.Dispose();
                    CProviderManager.UEFNProvider = null!;

                    foreach (FileInfo item in directoryInfo.EnumerateFiles("*.utoc*", SearchOption.TopDirectoryOnly))
                    {
                        FileInfo fileInfo = new FileInfo(item.FullName.SubstringBeforeLast('.') + ".backup");

                        if (fileInfo.Exists)
                        {
                            var reader = new Reader(item.FullName);
                            var readerbackup = new Reader(fileInfo.FullName);

                            var header = new FIoStoreTocHeader(reader);
                            var headerbackup = new FIoStoreTocHeader(readerbackup);

                            if (header.Compare(headerbackup) && reader.Length == readerbackup.Length)
                            {
                                Log.Information($"{fileInfo.Name} IO header matches {item.Name} and will apply backup");
                                reader.Dispose();
                                readerbackup.Dispose();
                                File.Move(fileInfo.FullName, item.FullName, true);
                            }
                            else
                            {
                                Log.Warning($"{fileInfo.Name} IO header does not match {item.Name} attempting to remove backup");
                                reader.Dispose();
                                readerbackup.Dispose();
                                File.Delete(fileInfo.FullName);
                            }
                        }
                    }

                    UEFN.Clear(paks);
                    SwapLogs.Clear();
                    SwapLogsDescription.Text = string.Format(Languages.Read(Languages.Type.View, "SettingsView", "SwapLogsDescription"), 0, 0, 0);
                }
                catch (Exception Exception)
                {
                    Log.Error(Exception, $"Failed to revert all cosmetics");
                    Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Error"), Languages.Read(Languages.Type.Message, "RevertAllCosmeticsError"), MessageBoxButton.OK, solutions: Languages.ReadSolutions(Languages.Type.Message, "RemoveError"));
                    return;
                }
            }
        }

        private void StartFortnite_Click(object sender, RoutedEventArgs e)
        {
            Log.Information("Starting Fortnite");
            EpicGamesLauncher.Launch();

            Log.Information("Closing Galaxy Swapper");
            Environment.Exit(0);
        }

        private void CloseFortnite_Click(object sender, RoutedEventArgs e)
        {
            EpicGamesLauncher.Close();
            Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"), Languages.Read(Languages.Type.Message, "CloseFortnite"), MessageBoxButton.OK);
        }

        private void DiscordRichPresence_Click(object sender, RoutedEventArgs e)
        {
            Settings.Edit(Settings.Type.RichPresence, DiscordRichPresence.IsChecked);
            if (DiscordRichPresence.IsChecked == true)
            {
                Presence.Initialize();
                Presence.Update("Settings");
            }
            else
                Presence.Client.ClearPresence();
        }

        private void LanguageSelection_Click(object sender, RoutedEventArgs e)
        {
            Memory.MainView.SetOverlay(new LanguageSelectionView(this));
        }

        private void AutoCloseFortnite_Click(object sender, RoutedEventArgs e) => Settings.Edit(Settings.Type.CloseFortnite, AutoCloseFortnite.IsChecked);
        private void KickWarning_Click(object sender, RoutedEventArgs e) => Settings.Edit(Settings.Type.KickWarning, KickWarning.IsChecked);
        private void ShareStats_Click(object sender, RoutedEventArgs e) => Settings.Edit(Settings.Type.ShareStats, ShareStats.IsChecked);
        private void BackPackGender_Click(object sender, RoutedEventArgs e)
        {
            Settings.Edit(Settings.Type.BackpackGender, BackPackGender.IsChecked);
            if (Generate.Cache.ContainsKey(Generate.Type.Backpacks))
                Generate.Cache[Generate.Type.Backpacks]?.Cosmetics.Values.Where(cosmetic => cosmetic.Options.Count != 0).ToList().ForEach(cosmetic => cosmetic.Options.Clear());
        }

        private void HeroDefinition_Click(object sender, RoutedEventArgs e)
        {
            Settings.Edit(Settings.Type.HeroDefinition, HeroDefinition.IsChecked);
            if (Generate.Cache.ContainsKey(Generate.Type.Characters))
                Generate.Cache[Generate.Type.Characters]?.Cosmetics.Values.Where(cosmetic => cosmetic.Options.Count != 0).ToList().ForEach(cosmetic => cosmetic.Options.Clear());
        }

        private void SortByStats_Click(object sender, RoutedEventArgs e)
        {
            Settings.Edit(Settings.Type.SortByStats, SortByStats.IsChecked);
            Generate.Cache.Clear();
            Memory.Clear();
        }

        private void Nsfw_Click(object sender, RoutedEventArgs e)
        {
            Settings.Edit(Settings.Type.HideNsfw, Nsfw.IsChecked);
            Memory.Clear();
            Memory.LobbyView = new();
            Generate.Cache.Clear();
        }

        private void RefreshCosmetics_Click(object sender, RoutedEventArgs e)
        {
            Endpoint.Clear();
            Endpoint.Read(Endpoint.Type.Cosmetics);
            Generate.Cache.Clear();
            Memory.Clear(false);
        }
    }
}