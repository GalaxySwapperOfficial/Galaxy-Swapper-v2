using Galaxy_Swapper_v2.Workspace.Other;
using Galaxy_Swapper_v2.Workspace.Usercontrols;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static Galaxy_Swapper_v2.Workspace.Other.Endpoint;

namespace Galaxy_Swapper_v2.Workspace.Forms
{
    public partial class Theme_Editor : Form
    {
        MainView MainView;
        Settings Settings;
        public Theme_Editor(MainView main, Settings settings)
        {
            InitializeComponent();
            ControlHelper.SetForm(this);
            ControlHelper.SetDrag(Dragbar);
            LoadColors();
            foreach (Control x in this.Controls)
            {
                if (x.Height == 50) { x.Click += ClrPnl_Click; }
            }
            trackBar1.Value = int.Parse(SettingsController.ConfigReturn("IconSize"));
            if (trackBar1.Value == 80) { label11.Text = $"Cosmetic Icons Size:{trackBar1.Value} (Default)"; }
            else { label11.Text = $"Cosmetic Icons Size:{trackBar1.Value}"; }
            MainView = main;
            Settings = settings;
        }
        public void LoadColors()
        {
            this.BackColor = Colors.MHex();
            Dragbar.BackColor = Colors.SHex();
            MHex.BackColor = Colors.MHex();
            SHex.BackColor = Colors.SHex();
            ButtonHex.BackColor = Colors.ButtonHex();
            TextHex.BackColor = Colors.TextHex();
            SecTextHex.BackColor = Colors.SecTextHex();
            HHex.BackColor = Colors.HHex();
            foreach (Control x in this.Controls)
            {
                if (x is Label)
                {
                    if (x.BackColor != Color.Transparent) { x.BackColor = Colors.ButtonHex(); ControlHelper.ButtonHover(x); }
                    x.ForeColor = Colors.TextHex();
                }
            }
        }
        public void ClrPnl_Click(object sender, EventArgs e)
        {
            using (ColorDialog a = new ColorDialog())
            {
                if (a.ShowDialog() == DialogResult.OK)
                {
                    ((Panel)sender).BackColor = a.Color;
                    JObject NewColorSettings = JObject.Parse(File.ReadAllText(Global.Appdata + "\\Galaxy Swapper v2\\Colors.json"));
                    NewColorSettings[((Panel)sender).Name] = ColorTranslator.ToHtml(a.Color);
                    File.WriteAllText(Global.Appdata + "\\Galaxy Swapper v2\\Colors.json", NewColorSettings.ToString());
                    LoadColors();
                    MainView.LoadColors();
                    Settings.LoadColors();
                }
            }
        }
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            JObject NewSettingsFile = JObject.Parse(File.ReadAllText(Global.Appdata + "\\Galaxy Swapper v2\\Settings.config"));
            NewSettingsFile["IconSize"] = trackBar1.Value;
            File.WriteAllText(Global.Appdata + "\\Galaxy Swapper v2\\Settings.config", NewSettingsFile.ToString());
            ControlHelper.Skins.Controls.Clear();
            ControlHelper.Backblings.Controls.Clear();
            ControlHelper.Pickaxes.Controls.Clear();
            ControlHelper.Emotes.Controls.Clear();
            ControlHelper.Misc.Controls.Clear();
            ControlHelper.Wraps.Controls.Clear();
            if (trackBar1.Value == 80) { label11.Text = $"Cosmetic Icons Size:{trackBar1.Value} (Default)"; }
            else { label11.Text = $"Cosmetic Icons Size:{trackBar1.Value}"; }
        }
        private void label5_Click(object sender, EventArgs e)
        {
            File.Delete(Global.Appdata + "\\Galaxy Swapper v2\\Colors.json");
            File.Delete(Global.Appdata + "\\Galaxy Swapper v2\\Settings.config");
            SettingsController.Initialize();
            LoadColors();
            MainView.LoadColors();
            Settings.LoadColors();
            trackBar1.Value = 80;
            trackBar1_ValueChanged(trackBar1, EventArgs.Empty);
            ControlHelper.Skins.Controls.Clear();
            ControlHelper.Backblings.Controls.Clear();
            ControlHelper.Pickaxes.Controls.Clear();
            ControlHelper.Emotes.Controls.Clear();
            ControlHelper.Misc.Controls.Clear();
            ControlHelper.Wraps.Controls.Clear();
            MessageBox.Show(this, Endpoint.APIReturn(Endpoints.Languages, "ResetSettings"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void CloseBox_Click(object sender, EventArgs e) => this.Close();
    }
}
