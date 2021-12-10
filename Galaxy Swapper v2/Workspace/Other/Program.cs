using Galaxy_Swapper_v2.Properties;
using Galaxy_Swapper_v2.Workspace;
using Galaxy_Swapper_v2.Workspace.Forms;
using Galaxy_Swapper_v2.Workspace.Other;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Galaxy_Swapper_v2
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (Directory.GetCurrentDirectory().StartsWith(Global.Appdata))
            {
                MessageBox.Show("Swapper Cannot Be Started Through Winrar! Please Extract The Swapper Before Opening.", "Please Read:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SettingsController.Initialize();
            if (!Directory.Exists(Settings.Default.FortniteInstall))
            {
                Settings.Default.FortniteInstall = FortniteInstallLocation();
                Settings.Default.Save();
            }
            if (Login_Page.CheckKey() != true)
            {
                new Login_Page().ShowDialog();
                if (Login_Page.CheckKey() == false)
                    Environment.Exit(0);
            }
            CheckForUnkownUcas();
            RPC.StartRPC();
            Application.Run(new MainView());
        }
        public static void CheckForUnkownUcas()
        {
            string PakFolder = Settings.Default.FortniteInstall;
            System.IO.DirectoryInfo di = new DirectoryInfo(PakFolder);
            foreach (DirectoryInfo PakDirList in di.GetDirectories())
            {
                try
                {
                    foreach (string Paks in Directory.EnumerateFiles(PakDirList.FullName, "*.pak*", SearchOption.AllDirectories))
                    {
                        if (!PakDirList.FullName.Contains("Galaxy Swapper v2") && !PakDirList.FullName.Contains("~GalaxyLobby"))
                            File.Delete(Paks);
                    }
                    foreach (string Sigs in Directory.EnumerateFiles(PakDirList.FullName, "*.sig*", SearchOption.AllDirectories))
                    {
                        if (!PakDirList.FullName.Contains("Galaxy Swapper v2") && !PakDirList.FullName.Contains("~GalaxyLobby"))
                            File.Delete(Sigs);
                    }
                    foreach (string Ucas in Directory.EnumerateFiles(PakDirList.FullName, "*.ucas*", SearchOption.AllDirectories))
                    {
                        if (!PakDirList.FullName.Contains("Galaxy Swapper v2") && !PakDirList.FullName.Contains("~GalaxyLobby"))
                            File.Delete(Ucas);
                    }
                    foreach (string Utocs in Directory.EnumerateFiles(PakDirList.FullName, "*.utoc*", SearchOption.AllDirectories))
                    {
                        if (!PakDirList.FullName.Contains("Galaxy Swapper v2") && !PakDirList.FullName.Contains("~GalaxyLobby"))
                            File.Delete(Utocs);
                    }
                }
                catch { return; }
            }
        }
        public static string FortniteInstallLocation()
        {
            if (System.IO.Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Epic\UnrealEngineLauncher"))
            {
                if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Epic\UnrealEngineLauncher\LauncherInstalled.dat"))
                {
                    return string.Empty;
                }
                JObject Parse = JObject.Parse(System.IO.File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Epic\UnrealEngineLauncher\LauncherInstalled.dat"));
                foreach (var Array in Parse["InstallationList"])
                {
                    if (System.IO.Directory.Exists(Array["InstallLocation"].ToString() + @"\FortniteGame"))
                        return Array["InstallLocation"].ToString() + @"\FortniteGame\Content\Paks";
                }
            }
            else
                return string.Empty;
            return string.Empty;
        }
    }
}
