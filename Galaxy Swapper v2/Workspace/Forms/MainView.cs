using Galaxy_Swapper_v2.Workspace.Other;
using Galaxy_Swapper_v2.Workspace.Usercontrols;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Galaxy_Swapper_v2.Workspace.Other.Endpoint;

namespace Galaxy_Swapper_v2.Workspace.Forms
{
    public partial class MainView : Form
    {
        public MainView()
        {
            InitializeComponent();
            ControlHelper.SetForm(this);
            ControlHelper.SetDrag(panel1);
            LoadColors();
            Tab_Click(DashboardPanel, EventArgs.Empty);
            Task.Run(() => {
                CheckForIllegalCrossThreadCalls = false;
                JObject parse = JObject.Parse(Endpoint.APIReturn(Endpoints.Status, null));
                if (bool.Parse(parse["Warning"].ToString()) == true)
                {
                    MessageBox.Show(this, parse["WarningInfo"]["WarningMessage"].ToString(), parse["WarningInfo"]["WarningHeader"].ToString(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                if (bool.Parse(parse["Downtime"].ToString()) == true)
                {
                    MessageBox.Show(this, parse["DowntimeInfo"]["DowntimeMessage"].ToString(), parse["DowntimeInfo"]["DowntimeHeader"].ToString(), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Global.UrlStart(parse["Discord"].ToString());
                    Environment.Exit(0);
                }
                if (parse["Version"].ToString() != "1.5")
                {
                    MessageBox.Show(this, string.Format(Endpoint.APIReturn(Endpoints.Languages, "SwapperUpdate"), parse["Version"].ToString()), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Global.UrlStart(parse["DownloadLink"].ToString());
                    Environment.Exit(0);
                }
                Global.CloseFortnite();
            });
        }
        public void LoadColors()
        {
            this.BackColor = Colors.MHex();
            panel1.BackColor = Colors.SHex();
            panel9.BackColor = Colors.SecTextHex();
            label8.ForeColor = Colors.SecTextHex();
            foreach (Control SideBarControls in panel1.Controls)
            {
                foreach (Control x in SideBarControls.Controls)
                {
                    if (x is Label)
                    {
                        x.ForeColor = Colors.TextHex();
                    }
                }
            }
        }
        public void ResetTab()
        {
            foreach (Control x in panel1.Controls)
            {
                if (x is Panel && x.BackColor == Colors.HHex()) { x.BackColor = Color.Transparent; }
            }
        }
        private void Tab_Click(object sender, EventArgs e)
        {
            var Sender = (Control)sender;
            if (Sender is Panel == false) { Sender = ((Control)sender).Parent; }
            if (Sender.Name == "DiscordPanel")
            {
                Global.UrlStart(Endpoint.APIReturn(Endpoints.Status, "Discord"));
                return;
            }
            ResetTab(); Sender.BackColor = Colors.HHex();
            ControlHolder.Controls.Clear();
            switch (Sender.Name)
            {
                case "DashboardPanel":
                    RPC.Update("Dashboard");
                    ControlHolder.Controls.Add(new Dashboard());
                    return;
                case "SkinsPanel":
                    RPC.Update("Skins");
                    ControlHolder.Controls.Add(new Cosmetics("Skins"));
                    return;
                case "BackblingsPanel":
                    RPC.Update("Backblings");
                    ControlHolder.Controls.Add(new Cosmetics("Backblings"));
                    return;
                case "PickaxesPanel":
                    RPC.Update("Pickaxes");
                    ControlHolder.Controls.Add(new Cosmetics("Pickaxes"));
                    return;
                case "EmotesPanel":
                    RPC.Update("Emotes");
                    ControlHolder.Controls.Add(new Cosmetics("Emotes"));
                    Task.Run(() => {
                        CheckForIllegalCrossThreadCalls = false;
                        if (bool.Parse(SettingsController.ConfigReturn("EmoteWarning")) == true)
                        {
                            MessageBox.Show(this, Endpoint.APIReturn(Endpoints.Languages, "EmoteWarning"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    });
                    return;
                case "WrapsPanel":
                    RPC.Update("Wraps");
                    ControlHolder.Controls.Add(new Cosmetics("Wraps"));
                    return;
                case "MiscPanel":
                    RPC.Update("Misc");
                    ControlHolder.Controls.Add(new Cosmetics("Misc"));
                    return;
                case "SettingsPanel":
                    RPC.Update("Settings");
                    ControlHolder.Controls.Add(new Settings(this));
                    return;
            }
        }
        private void Control_Click(object sender, EventArgs e)
        {
            var Sender = (PictureBox)sender;
            switch (Sender.Name)
            {
                case "CloseBox":
                    Environment.Exit(0);
                    return;
                case "MinimizedBox":
                    this.WindowState = FormWindowState.Minimized;
                    return;
                case "DiscordPanel":
                    Global.UrlStart(Endpoint.APIReturn(Endpoints.Status, "Discord"));
                    return;
            }
        }
    }
}
