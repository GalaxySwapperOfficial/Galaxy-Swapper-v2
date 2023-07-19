using CUE4Parse.Utils;
using Galaxy_Swapper_v2.Workspace.Generation;
using Galaxy_Swapper_v2.Workspace.Properties;
using Galaxy_Swapper_v2.Workspace.Usercontrols.Overlays;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Net;
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
            CharacterGenderHeader.Text = Languages.Read(Languages.Type.View, "SettingsView", "CharacterGenderHeader");
            CharacterGenderDescription.Text = Languages.Read(Languages.Type.View, "SettingsView", "CharacterGenderDescription");
            BackPackGenderHeader.Text = Languages.Read(Languages.Type.View, "SettingsView", "BackPackGenderHeader");
            BackPackGenderDescription.Text = Languages.Read(Languages.Type.View, "SettingsView", "BackPackGenderDescription");

            //Buttons
            EditPath.Content = Languages.Read(Languages.Type.View, "SettingsView", "EditPath");
            //RemoveBackup.Content = Languages.Read(Languages.Type.View, "SettingsView", "RemoveBackup");
            ResetSwapData.Content = Languages.Read(Languages.Type.View, "SettingsView", "ResetSwapData");
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

            if (Settings.Read(Settings.Type.RichPresence).Value<bool>())
                DiscordRichPresence.IsChecked = true;
            if (Settings.Read(Settings.Type.CloseFortnite).Value<bool>())
                AutoCloseFortnite.IsChecked = true;
            if (Settings.Read(Settings.Type.KickWarning).Value<bool>())
                KickWarning.IsChecked = true;
            if (Settings.Read(Settings.Type.CharacterGender).Value<bool>())
                CharacterGender.IsChecked = true;
            if (Settings.Read(Settings.Type.BackpackGender).Value<bool>())
                BackPackGender.IsChecked = true;

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

        private void RemoveBackup_Click(object sender, RoutedEventArgs e)
        {
            if (Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Question"), Languages.Read(Languages.Type.Message, "RemoveBackup"), MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                string Installtion = $"{Settings.Read(Settings.Type.Installtion).Value<string>()}\\FortniteGame\\Content\\Paks";
                if (Directory.Exists($"{Installtion}\\Galaxy Swapper v2"))
                {
                    try
                    {
                        Directory.Delete($"{Installtion}\\Galaxy Swapper v2", true);
                    }
                    catch (Exception Exception)
                    {
                        Log.Error(Exception, $"Failed to delete {Installtion}\\Galaxy Swapper v2");
                        Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Error"), Languages.Read(Languages.Type.Message, "RemoveError"), MessageBoxButton.OK, solutions: Languages.ReadSolutions(Languages.Type.Message, "RemoveError"));
                        return;
                    }
                }

                SwapLogs.Clear();
                SwapLogsDescription.Text = string.Format(Languages.Read(Languages.Type.View, "SettingsView", "SwapLogsDescription"), 0, 0, 0);
            }
        }

        private void ResetSwapData_Click(object sender, RoutedEventArgs e)
        {
            if (Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Question"), Languages.Read(Languages.Type.Message, "ResetSwapData"), MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                SwapData.Delete();
            }
        }

        private void Verify_Click(object sender, RoutedEventArgs e)
        {
            string Installtion = $"{Settings.Read(Settings.Type.Installtion).Value<string>()}\\FortniteGame\\Content\\Paks";

            foreach (string Unkown in Directory.GetDirectories(Installtion))
            {
                Log.Information($"Found {Unkown}");
                if (Directory.GetFiles(Unkown, "*.ucas").Length != 0 || Directory.GetFiles(Unkown, "*.pak").Length != 0)
                {
                    try
                    {
                        Directory.Delete(Unkown, true);
                        Log.Information($"Deleted {Unkown} (Contains ucas or sig)");
                    }
                    catch (Exception Exception)
                    {
                        Log.Error(Exception, $"Failed to delete {Unkown}");
                        Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Error"), string.Format(Languages.Read(Languages.Type.Message, "VerifyError"), Unkown), MessageBoxButton.OK, solutions: Languages.ReadSolutions(Languages.Type.Message, "VerifyError"));
                        return;
                    }
                }
                else
                    Log.Information($"Skipped {Unkown} (Does not contain ucas or sig)");
            }

            SwapData.Delete();
            SwapLogs.Clear();
            SwapLogsDescription.Text = string.Format(Languages.Read(Languages.Type.View, "SettingsView", "SwapLogsDescription"), 0, 0, 0);

            Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"), Languages.Read(Languages.Type.Message, "Verify"), MessageBoxButton.OK);
            EpicGamesLauncher.Verify();
            Environment.Exit(0);
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

                    foreach (var iostore in new DirectoryInfo(paks).GetFiles())
                    {
                        var ext = iostore.Extension.SubstringAfter('.');

                        if (ext != "backup")
                            continue;

                        var oldiostore = new FileInfo(iostore.FullName.Replace(".backup", ".utoc"));

                        if (iostore.Length != oldiostore.Length)
                        {
                            Log.Error($"Failed to revert {iostore.Name} due to length missmatch! Outdated?");
                            continue;
                        }

                        Log.Information($"Moving: {iostore.FullName}\nTo: {oldiostore.FullName}");
                        File.Copy(iostore.FullName, oldiostore.FullName, true); //No need to delete backup since Fortnite won't mount it anyways..
                    }

                    Log.Information("Finished reverting all cosmetics");
                    UEFN.Clear();
                    SwapData.Delete();
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
            Log.Information("Downloading Fortnite Launcher");

            const string URL = "https://galaxyswapperv2.com/Downloads/Fortnite%20Launcher.exe";
            string FortniteLauncherPath = $"{Config.Path}\\FortniteLauncher.exe";

            try
            {
                if (File.Exists(FortniteLauncherPath))
                    File.Delete(FortniteLauncherPath);

                using (WebClient WBC = new WebClient())
                {
                    WBC.DownloadFile(URL, FortniteLauncherPath);
                    WBC.Dispose();
                }
            }
            catch (Exception Exception)
            {
                Log.Error(Exception, $"Failed to download Fortnite Launcher from {URL} to {FortniteLauncherPath}");
            }

            Log.Information("Starting custom Fortntie Launcher");
            if (File.Exists(FortniteLauncherPath))
            {
                FortniteLauncherPath.UrlStart();
                Log.Information($"Launched {FortniteLauncherPath}");
            }

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
        private void CharacterGender_Click(object sender, RoutedEventArgs e)
        {
            Settings.Edit(Settings.Type.CharacterGender, CharacterGender.IsChecked);
            if (Generate.Cache.ContainsKey(Generate.Type.Characters))
                Generate.Cache[Generate.Type.Characters].Cosmetics.Values.Where(cosmetic => cosmetic.Options.Count != 0).ToList().ForEach(cosmetic => cosmetic.Options.Clear());
        }
        private void BackPackGender_Click(object sender, RoutedEventArgs e)
        {
            Settings.Edit(Settings.Type.BackpackGender, BackPackGender.IsChecked);
            if (Generate.Cache.ContainsKey(Generate.Type.Backpacks))
                Generate.Cache[Generate.Type.Backpacks]?.Cosmetics.Values.Where(cosmetic => cosmetic.Options.Count != 0).ToList().ForEach(cosmetic => cosmetic.Options.Clear());
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