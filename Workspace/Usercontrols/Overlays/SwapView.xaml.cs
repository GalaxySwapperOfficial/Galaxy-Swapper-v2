using Galaxy_Swapper_v2.Workspace.Components;
using Galaxy_Swapper_v2.Workspace.Compression;
using Galaxy_Swapper_v2.Workspace.Generation.Formats;
using Galaxy_Swapper_v2.Workspace.Properties;
using Galaxy_Swapper_v2.Workspace.Swapping;
using Galaxy_Swapper_v2.Workspace.Swapping.Other;
using Galaxy_Swapper_v2.Workspace.Swapping.Providers;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using static Galaxy_Swapper_v2.Workspace.Global;

namespace Galaxy_Swapper_v2.Workspace.Usercontrols.Overlays
{
    public partial class SwapView : UserControl
    {
        private Option Option;
        public SwapView(Option option)
        {
            InitializeComponent();
            Option = option;
            DisplayName.Text = Option.Name;
        }

        private void Close_Click(object sender, MouseButtonEventArgs e) => Memory.MainView.RemoveOverlay();
        private void SwapView_Loaded(object sender, RoutedEventArgs e)
        {
            if (Option.Nsfw)
            {
                if (Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Warning"), Languages.Read(Languages.Type.View, "SwapView", "NSFW"), MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    Memory.MainView.RemoveOverlay();
                    return;
                }
            }

            Icon.LoadImage(Option.Icon);
            OverrideIcon.LoadImage(Option.OverrideIcon);

            Convert.Content = Languages.Read(Languages.Type.View, "SwapView", "Convert");
            Revert.Content = Languages.Read(Languages.Type.View, "SwapView", "Revert");

            foreach (var social in Option.Socials)
            {
                Socials.Children.Add(new CSocialControl(social) { Margin = new Thickness(5, 0, 5, 0) });
            }

            if (Option.UEFNFormat && SwapLogs.IsSwappedUEFNSwapped(Option.Name, out string revertitem))
            {
                Message.Display(Languages.Read(Languages.Type.Header, "Warning"), string.Format(Languages.Read(Languages.Type.Message, "UEFNRevert"), revertitem), MessageBoxButton.OK);
                Memory.MainView.RemoveOverlay();
                return;
            }
            else if (SwapLogs.IsSwapped(Option.Name))
            {
                Converted.Text = Languages.Read(Languages.Type.View, "SwapView", "ON");
                Converted.Foreground = Colors.Text;
            }
            else if (Settings.Read(Settings.Type.KickWarning).Value<bool>())
            {
                SwapLogs.Read(out int Count, out int AssetCount, out int Ucas, out int Utoc);
                if (AssetCount + Option.Exports.Count > 13)
                {
                    Message.Display(Languages.Read(Languages.Type.Header, "Warning"), string.Format(Languages.Read(Languages.Type.Message, "MaxAssetCount"), 13), MessageBoxButton.OK);
                    Memory.MainView.RemoveOverlay();
                    return;
                }
            }
            else
                Converted.Text = Languages.Read(Languages.Type.View, "SwapView", "OFF");

            if (Settings.Read(Settings.Type.CloseFortnite).Value<bool>())
                EpicGamesLauncher.Close();

            if (!string.IsNullOrEmpty(Option.Message) && Option.Message.ToLower() != "false")
                Message.Display(Languages.Read(Languages.Type.Header, "Warning"), Option.Message, MessageBoxButton.OK);
            if (!string.IsNullOrEmpty(Option.OptionMessage) && Option.OptionMessage.ToLower() != "false")
                Message.Display(Languages.Read(Languages.Type.Header, "Warning"), Option.OptionMessage, MessageBoxButton.OK);

            Presence.Update(Option.Name);
        }

        public enum Type
        {
            None, // ?
            Info,
            Warning,
            Error
        }

        public void Output(string Content, Type Type)
        {
            this.Dispatcher.Invoke(() =>
            {
                switch (Type)
                {
                    case Type.Info:
                        LOG.Foreground = Colors.Text;
                        Log.Information(Content);
                        break;
                    case Type.Warning:
                        LOG.Foreground = Colors.Yellow;
                        Log.Warning(Content);
                        break;
                    case Type.Error:
                        LOG.Foreground = Colors.Red;
                        Log.Error(Content);
                        break;
                }

                LOG.Text = Content;

                if (LOG.Text.Length > 64)
                    LOG.FontSize = 10F;
                else
                    LOG.FontSize = 14F;
            });
        }

        private BackgroundWorker Worker { get; set; } = default!;
        private void Worker_Click(object sender, RoutedEventArgs e)
        {
            if (Worker is not null && Worker.IsBusy)
            {
                Message.Display(Languages.Read(Languages.Type.Header, "Warning"), Languages.Read(Languages.Type.View, "SwapView", "WorkerBusy"), MessageBoxButton.OK);
                return;
            }

            string epicinstallation = Settings.Read(Settings.Type.EpicInstalltion).Value<string>();
            if (string.IsNullOrEmpty(epicinstallation) || !File.Exists(epicinstallation))
            {
                Log.Information(Languages.Read(Languages.Type.Message, "EpicGamesLauncherPathEmpty"));
                Memory.MainView.SetOverlay(new EpicGamesLauncherDirEmpty());
                return;
            }

            EpicGamesLauncher.Close();

            Worker = new BackgroundWorker();
            Worker.RunWorkerCompleted += Worker_Completed;

            if (((Button)sender).Name == "Convert")
                Worker.DoWork += Worker_Convert;
            else
                Worker.DoWork += Worker_Revert;

            var StoryBoard = Interface.SetElementAnimations(new Interface.BaseAnim { Element = DisplayName, Property = new PropertyPath(Control.OpacityProperty), ElementAnim = new DoubleAnimation() { From = 1, To = 0, Duration = new TimeSpan(0, 0, 0, 0, 100) } }, new Interface.BaseAnim { Element = LOG, Property = new PropertyPath(Control.OpacityProperty), ElementAnim = new DoubleAnimation() { From = 0, To = 1, Duration = new TimeSpan(0, 0, 0, 0, 100), BeginTime = new TimeSpan(0, 0, 0, 0, 100) } });
            StoryBoard.Completed += delegate
            {
                Worker.RunWorkerAsync();
            };
            StoryBoard.Begin();
        }

        private void Worker_Completed(object sender, RunWorkerCompletedEventArgs e) => Interface.SetElementAnimations(new Interface.BaseAnim { Element = DisplayName, Property = new PropertyPath(Control.OpacityProperty), ElementAnim = new DoubleAnimation() { From = 0, To = 1, Duration = new TimeSpan(0, 0, 0, 0, 100), BeginTime = new TimeSpan(0, 0, 0, 0, 100) } }, new Interface.BaseAnim { Element = LOG, Property = new PropertyPath(Control.OpacityProperty), ElementAnim = new DoubleAnimation() { From = 1, To = 0, Duration = new TimeSpan(0, 0, 0, 0, 100) } }).Begin();

        private void Worker_Convert(object sender, DoWorkEventArgs e)
        {
            try
            {
                var Stopwatch = new Stopwatch();
                Stopwatch.Start();

                string paks = $"{Settings.Read(Settings.Type.Installtion).Value<string>()}{CProvider.Paks}";

                Output(Languages.Read(Languages.Type.View, "SwapView", "InitializingProvider"), Type.Info);
                CProvider.InitDefault();
                CProvider.InitUEFN();

                List<string> Ucas = new List<string>();
                List<string> Utocs = new List<string>();
                foreach (var Asset in Option.Exports)
                {
                    Output(string.Format(Languages.Read(Languages.Type.View, "SwapView", "Exporting"), System.IO.Path.GetFileNameWithoutExtension(Asset.Object)), Type.Info);

                    if (!CProvider.Save(Asset.Object))
                        throw new Exception($"Failed to export {Asset.Object}");

                    Asset.Object = FormatObject(Asset.Object);
                    Asset.Export = CProvider.Export;

                    if (!string.IsNullOrEmpty(Asset.OverrideObject))
                    {
                        if (string.IsNullOrEmpty(Asset.OverrideBuffer))
                        {
                            Output(string.Format(Languages.Read(Languages.Type.View, "SwapView", "Exporting"), System.IO.Path.GetFileNameWithoutExtension(Asset.OverrideObject)), Type.Info);

                            if (!CProvider.Save(Asset.OverrideObject))
                                throw new Exception($"Failed to export {Asset.OverrideObject}");

                            Asset.OverrideExport = CProvider.Export;
                        }
                        else
                        {
                            Output(Languages.Read(Languages.Type.View, "SwapView", "Decompressing"), Type.Info);
                            Asset.OverrideExport = new () { Buffer = gzip.Decompress(Asset.OverrideBuffer) };
                        }
                        Asset.OverrideObject = FormatObject(Asset.OverrideObject);
                    }

                    Ucas.AddRange(Ucas.Contains(Asset.Export.Ucas) ? Enumerable.Empty<string>() : new[] { Asset.Export.Ucas });
                    Utocs.AddRange(Utocs.Contains(Asset.Export.Utoc) ? Enumerable.Empty<string>() : new[] { Asset.Export.Utoc });
                }

                if (Settings.Read(Settings.Type.KickWarning).Value<bool>())
                {
                    SwapLogs.Kick(out bool UcasK, out bool UtocK, Ucas, Utocs);

                    if (UtocK)
                    {
                        Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Warning"), string.Format(Languages.Read(Languages.Type.Message, "MaxPakChunkCount"), "Utoc", 2), MessageBoxButton.OK);
                        return;
                    }
                }

                if (Option.UseMainUEFN)
                {
                    Output(Languages.Read(Languages.Type.View, "SwapView", "DownloadingMainUEFN"), Type.Info);
                    UEFN.DownloadMain(paks);
                }

                if (Option.Downloadables != null &&  Option.Downloadables.Count > 0)
                {
                    Output(Languages.Read(Languages.Type.View, "SwapView", "DownloadingUEFN"), Type.Info);
                    foreach (var downloadable in Option.Downloadables)
                    {
                        UEFN.Add(paks, Option.Name, downloadable);
                    }
                }

                Output(Languages.Read(Languages.Type.View, "SwapView", "ModifyingEpicGamesLauncher"), Type.Info);
                CustomEpicGamesLauncher.Convert();

                Output(Languages.Read(Languages.Type.View, "SwapView", "ConvertingAssets"), Type.Info);

                foreach (var Export in Option.Exports)
                {
                    var Swap = new Swap(this, null, Export);
                    if (!Swap.Convert())
                        return;
                }

                if (Option.Cosmetic is not null && !Option.IsPlugin && Settings.Read(Settings.Type.ShareStats).Value<bool>())
                {
                    CosmeticTracker.Update(Option.Cosmetic.Name, Option.Cosmetic.ID, Option.Cosmetic.Type.ToString());
                }

                SwapLogs.Add(Option.Name, Option.Icon, Option.OverrideIcon, Option.Exports.Count, Ucas, Utocs, Option.UEFNFormat);

                this.Dispatcher.Invoke(() =>
                {
                    Converted.Text = Languages.Read(Languages.Type.View, "SwapView", "ON");
                    Converted.Foreground = Colors.Text;
                });

                TimeSpan TimeSpan = Stopwatch.Elapsed;
                if (TimeSpan.Minutes > 0)
                    Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"), string.Format(Languages.Read(Languages.Type.View, "SwapView", "ConvertedMinutes"), TimeSpan.Minutes), MessageBoxButton.OK);
                else
                    Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"), string.Format(Languages.Read(Languages.Type.View, "SwapView", "Converted"), TimeSpan.Seconds), MessageBoxButton.OK);
            }
            catch (FortniteDirectoryEmptyException Exception)
            {
                this.Dispatcher.Invoke(() =>
                {
                    Log.Error(Exception.Message, "Fortnite directory is null or empty");
                    Memory.MainView.SetOverlay(new FortniteDirEmpty());
                });
            }
            catch (CustomException CustomException)
            {
                Log.Error(CustomException.Message, "Caught CustomException");
                Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Error"), string.Format(Languages.Read(Languages.Type.Message, "ConvertError"), Option.Name, CustomException.Message), discord: true);
            }
            catch (Exception Exception)
            {
                Log.Error(Exception.Message, "Caught Exception");
                Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Error"), string.Format(Languages.Read(Languages.Type.Message, "ConvertError"), Option.Name, Exception.Message), solutions: Languages.ReadSolutions(Languages.Type.Message, "ConvertError"), discord: true);
            }
        }

        private void Worker_Revert(object sender, DoWorkEventArgs e)
        {
            try
            {
                var Stopwatch = new Stopwatch();
                Stopwatch.Start();

                Output(Languages.Read(Languages.Type.View, "SwapView", "InitializingProvider"), Type.Info);
                CProvider.InitDefault();

                foreach (var Asset in Option.Exports)
                {
                    Output(string.Format(Languages.Read(Languages.Type.View, "SwapView", "Exporting"), System.IO.Path.GetFileNameWithoutExtension(Asset.Object)), Type.Info);

                    if (!CProvider.Save(Asset.Object))
                        throw new Exception($"Failed to export {Asset.Object}");

                    Asset.Object = FormatObject(Asset.Object);
                    Asset.Export = CProvider.Export;
                }

                Output(Languages.Read(Languages.Type.View, "SwapView", "RevertingAssets"), Type.Info);

                foreach (var Export in Option.Exports)
                {
                    var Swap = new Swap(this, null, Export);
                    if (!Swap.Revert())
                        return;
                }

                if (Option.Downloadables != null && Option.Downloadables.Count > 0)
                {
                    Output(Languages.Read(Languages.Type.View, "SwapView", "RemovingUEFN"), Type.Info);
                    foreach (var downloadable in Option.Downloadables)
                    {
                        UEFN.Remove(Option.Name);
                    }
                }

                SwapLogs.Remove(Option.Name);

                this.Dispatcher.Invoke(() =>
                {
                    Converted.Text = Languages.Read(Languages.Type.View, "SwapView", "OFF");
                    Converted.Foreground = Colors.Text2;
                });

                TimeSpan TimeSpan = Stopwatch.Elapsed;
                if (TimeSpan.Minutes > 0)
                    Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"), string.Format(Languages.Read(Languages.Type.View, "SwapView", "RevertedMinutes"), TimeSpan.Minutes), MessageBoxButton.OK);
                else
                    Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"), string.Format(Languages.Read(Languages.Type.View, "SwapView", "Reverted"), TimeSpan.Seconds), MessageBoxButton.OK);
            }
            catch (FortniteDirectoryEmptyException Exception)
            {
                this.Dispatcher.Invoke(() =>
                {
                    Log.Error(Exception.Message, "Fortnite directory is null or empty");
                    Memory.MainView.SetOverlay(new FortniteDirEmpty());
                });
            }
            catch (CustomException CustomException)
            {
                Log.Error(CustomException.Message, "Caught CustomException");
                Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Error"), string.Format(Languages.Read(Languages.Type.Message, "RevertError"), Option.Name, CustomException.Message), discord: true);
            }
            catch (Exception Exception)
            {
                Log.Error(Exception.Message, "Caught Exception");
                Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Error"), string.Format(Languages.Read(Languages.Type.Message, "RevertError"), Option.Name, Exception.Message), discord: true, solutions: Languages.ReadSolutions(Languages.Type.Message, "ConvertError"));
            }
        }

        private static string FormatObject(string Path) => (Path.Contains('.') ? Path.Split('.').First() : Path).Replace("FortniteGame/Content/", "/Game/").Replace("FortniteGame/Plugins/GameFeatures/BRCosmetics/Content/", "/BRCosmetics/") + ".uasset";
    }
}