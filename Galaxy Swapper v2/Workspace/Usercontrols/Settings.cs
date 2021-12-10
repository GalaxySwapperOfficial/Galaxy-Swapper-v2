using Galaxy_Swapper_v2.Workspace.Forms;
using Galaxy_Swapper_v2.Workspace.Other;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Galaxy_Swapper_v2.Workspace.Other.Endpoint;

namespace Galaxy_Swapper_v2.Workspace.Usercontrols
{
    public partial class Settings : UserControl
    {
        MainView MainView;
        public Settings(MainView main)
        {
            InitializeComponent();
            LoadColors();
            MainView = main;
            FortniteInstallLabel.Text = Properties.Settings.Default.FortniteInstall;
            using (var reader = new StringReader(Encryption.Compression.Decompress(File.ReadAllText(Global.Appdata + "\\Galaxy Swapper v2\\Key.config"))))
            {
                int keydays = 0;
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    if (line.Contains("/"))
                        keydays++;
                }
                if (keydays > 30)
                    label14.Text = $"Key Days: ∞";
                else
                    label14.Text = $"Key Days: {keydays}";
            }
            label13.Text = $"Username: {Global.Username}";
            pictureBox2.ImageLocation = Global.ProfilePicture;
            ControlHelper.SetEclipse(pictureBox2, 100);
            if (bool.Parse(SettingsController.ConfigReturn("DiscordRPC")) == false)
            {
                checkBox1.Checked = true;
            }
            if (bool.Parse(SettingsController.ConfigReturn("EmoteWarning")) == false)
            {
                checkBox2.Checked = true;
            }
            if (bool.Parse(SettingsController.ConfigReturn("HideNonePerfectDefaults")) == false)
            {
                checkBox3.Checked = true;
            }
        }
        public void LoadColors()
        {
            foreach (Control x in this.Controls)
            {
                if (x is Label) {
                    if (x.BackColor != Color.Transparent) { x.BackColor = Colors.ButtonHex(); ControlHelper.ButtonHover(x); }
                    x.ForeColor = Colors.TextHex();
                }
            }
        }

        private void StartFNButton_Click(object sender, EventArgs e)
        {
            "com.epicgames.launcher://apps/Fortnite?action=launch&silent=true".UrlStart();
            this.ParentForm.Visible = false;
            while (true)
            {
                Process[] ProcessID = Process.GetProcessesByName("FortniteClient-Win64-Shipping");
                if (ProcessID.Length == 0)
                    Thread.Sleep(1000);
                else
                {
                    ProcessID[0].WaitForExit();
                    Process.GetProcessesByName("EpicGamesLauncher")[0].Kill();
                    Environment.Exit(0);
                }
            }
        }
        private void label2_Click(object sender, EventArgs e)
        {
            Global.CloseFortnite();
            MessageBox.Show(this, Endpoint.APIReturn(Endpoints.Languages, "CloseFN"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void label5_Click(object sender, EventArgs e)
        {
            new Theme_Editor(MainView, this).ShowDialog();
        }

        private void label7_Click(object sender, EventArgs e)
        {
            JArray SwapLogsFile = JArray.Parse(File.ReadAllText(Global.Appdata + "\\Galaxy Swapper v2\\Swaps.log"));
            string Filesused = "";
            string CosmeticsSwapped = "";
            foreach (var Swap in SwapLogsFile)
            {
                CosmeticsSwapped += Swap["Name"].ToString() + Environment.NewLine;
                foreach (var Pak in JArray.Parse(Swap["Files"].ToString()))
                {
                    if (!Filesused.Contains(Pak["File"].ToString()))
                        Filesused += Pak["File"].ToString() + Environment.NewLine;
                }
            }
            if (CosmeticsSwapped.Contains("To"))
                MessageBox.Show(this, $"Cosmetics Swapped:{Environment.NewLine}{CosmeticsSwapped}{Environment.NewLine}Files Used:{Environment.NewLine}{Filesused}", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(this, Endpoint.APIReturn(Endpoints.Languages, "NoSwappedLogsMsg"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void label6_Click(object sender, EventArgs e)
        {
            ControlHelper.Skins.Controls.Clear();
            ControlHelper.Backblings.Controls.Clear();
            ControlHelper.Pickaxes.Controls.Clear();
            ControlHelper.Emotes.Controls.Clear();
            ControlHelper.Misc.Controls.Clear();
            ControlHelper.Wraps.Controls.Clear();
            MessageBox.Show(this, Endpoint.APIReturn(Endpoints.Languages, "RefreshCosmetic"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void label8_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, Endpoint.APIReturn(Endpoints.Languages, "ResetLogsMsg1"), string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                if (File.Exists(Global.Appdata + "\\Galaxy Swapper v2\\Swaps.log"))
                    File.WriteAllText(Global.Appdata + "\\Galaxy Swapper v2\\Swaps.log", "[]");
                MessageBox.Show(this, Endpoint.APIReturn(Endpoints.Languages, "ResetLogsMsg2"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            try
            {
                if (Directory.Exists(Properties.Settings.Default.FortniteInstall + "\\Galaxy Swapper v2"))
                    Directory.Delete(Properties.Settings.Default.FortniteInstall + "\\Galaxy Swapper v2", true);
                if (Directory.Exists(Properties.Settings.Default.FortniteInstall + "\\~GalaxyLobby"))
                    Directory.Delete(Properties.Settings.Default.FortniteInstall + "\\~GalaxyLobby", true);
                File.WriteAllText(Global.Appdata + "\\Galaxy Swapper v2\\Swaps.log", "[]");
            }
            catch (Exception error)
            {
                MessageBox.Show(this, $"Caught A Error While Trying To Delete Duped Ucas!\n{error.ToString()}\nPlease Restart The Swapper And Try Again.", "Caught A Error Please Read!", MessageBoxButtons.OK, MessageBoxIcon.Error); ;
                return;
            }
            MessageBox.Show(this, Endpoint.APIReturn(Endpoints.Languages, "RemoveDupedUcasMessage"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void FolderIcon_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderselection = new FolderBrowserDialog();
            folderselection.Description = "Select Your Fortnite Install Location!";
            folderselection.UseDescriptionForTitle = true;
            if (folderselection.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(folderselection.SelectedPath + "\\global.ucas"))
                {
                    Properties.Settings.Default.FortniteInstall = folderselection.SelectedPath;
                    Properties.Settings.Default.Save();
                    FortniteInstallLabel.Text = folderselection.SelectedPath;
                }
                else
                {
                    MessageBox.Show(this, "The Selected Location Does Not Contain global.ucas!\nMake Sure You Selected Your Paks Folder.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            JObject NewSettingsFile = JObject.Parse(File.ReadAllText(Global.Appdata + "\\Galaxy Swapper v2\\Settings.config"));
            if (checkBox1.Checked == true)
            {
                NewSettingsFile["DiscordRPC"] = false;
                RPC.Client.ClearPresence();
            }
            else
            {
                NewSettingsFile["DiscordRPC"] = true;
                RPC.StartRPC();
            }
            File.WriteAllText(Global.Appdata + "\\Galaxy Swapper v2\\Settings.config", NewSettingsFile.ToString());
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            JObject NewSettingsFile = JObject.Parse(File.ReadAllText(Global.Appdata + "\\Galaxy Swapper v2\\Settings.config"));
            if (checkBox2.Checked == true)
            {
                NewSettingsFile["EmoteWarning"] = false;
            }
            else
            {
                NewSettingsFile["EmoteWarning"] = true;
            }
            File.WriteAllText(Global.Appdata + "\\Galaxy Swapper v2\\Settings.config", NewSettingsFile.ToString());
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            JObject NewSettingsFile = JObject.Parse(File.ReadAllText(Global.Appdata + "\\Galaxy Swapper v2\\Settings.config"));
            if (checkBox3.Checked == true)
            {
                NewSettingsFile["HideNonePerfectDefaults"] = false;
            }
            else
            {
                NewSettingsFile["HideNonePerfectDefaults"] = true;
            }
            ControlHelper.Skins.Controls.Clear();
            File.WriteAllText(Global.Appdata + "\\Galaxy Swapper v2\\Settings.config", NewSettingsFile.ToString());
        }
        //can't be asked to optimize this so....
        private void label3_Click(object sender, EventArgs e)
        {
            string PakFolder = Properties.Settings.Default.FortniteInstall;
            System.IO.DirectoryInfo di = new DirectoryInfo(PakFolder);
            foreach (FileInfo file in di.GetFiles())
            {
                if (file.Extension == ".ucas")
                {
                    var Matching = FindMatchingLength(file.Length, file.Name);
                    if (Matching != null)
                    {
                        //So Which Ever File Is Smaller Will Be Our Normal Ucas And Which Ever Is Bigger Name Length Is Duped
                        if (Matching.Length > file.Name.Length)
                        {
                            try
                            {
                                File.Delete(PakFolder + "\\" + Matching);
                                if (File.Exists(PakFolder + "\\" + Matching.Replace(".ucas", ".utoc")))
                                    File.Delete(PakFolder + "\\" + Matching.Replace(".ucas", ".utoc"));
                                if (File.Exists(PakFolder + "\\" + Matching.Replace(".ucas", ".pak")))
                                    File.Delete(PakFolder + "\\" + Matching.Replace(".ucas", ".pak"));
                                if (File.Exists(PakFolder + "\\" + Matching.Replace(".ucas", ".sig")))
                                    File.Delete(PakFolder + "\\" + Matching.Replace(".ucas", ".sig"));
                            }
                            catch (Exception error)
                            {
                                MessageBox.Show(this, $"Caught A Error While Trying To Delete Duped Ucas!\n{error.ToString()}\nPlease Restart The Swapper And Try Again.", "Caught A Error Please Read!", MessageBoxButtons.OK, MessageBoxIcon.Error); ;
                                return;
                            }
                        }
                    }
                }
            }
            //Finds Any Ucas,Utocs,Paks,Sigs Files In Any Other Directory Then Pak Folder And Deletes Them
            foreach (DirectoryInfo PakDirList in di.GetDirectories())
            {
                try
                {
                    foreach (string Paks in Directory.EnumerateFiles(PakDirList.FullName, "*.pak*", SearchOption.AllDirectories))
                        File.Delete(Paks);
                    foreach (string Sigs in Directory.EnumerateFiles(PakDirList.FullName, "*.sig*", SearchOption.AllDirectories))
                        File.Delete(Sigs);
                    foreach (string Ucas in Directory.EnumerateFiles(PakDirList.FullName, "*.ucas*", SearchOption.AllDirectories))
                        File.Delete(Ucas);
                    foreach (string Utocs in Directory.EnumerateFiles(PakDirList.FullName, "*.utoc*", SearchOption.AllDirectories))
                        File.Delete(Utocs);
                }
                catch (Exception error)
                {
                    MessageBox.Show(this, $"Caught A Error While Trying To Delete Duped Ucas!\n{error.ToString()}\nPlease Restart The Swapper And Try Again.", "Caught A Error Please Read!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            File.WriteAllText(Global.Appdata + "\\Galaxy Swapper v2\\Swaps.log", "[]");
            Global.UrlStart("com.epicgames.launcher://apps/Fortnite?action=verify&silent=false");
            MessageBox.Show(this, Endpoint.APIReturn(Endpoints.Languages, "VerifyMessage"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public static string FindMatchingLength(long length, string CurrentPakName)
        {
            string PakFolder = Properties.Settings.Default.FortniteInstall;
            System.IO.DirectoryInfo di = new DirectoryInfo(PakFolder);

            foreach (FileInfo file in di.GetFiles())
            {
                if (file.Extension == ".ucas")
                {
                    if (file.Length == length)
                        if (file.Name != CurrentPakName)
                            return file.Name;
                }
            }
            return null;
        }
    }
}
