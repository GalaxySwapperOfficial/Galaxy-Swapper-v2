using Galaxy_Swapper_v2.Workspace.Other;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Galaxy_Swapper_v2.Workspace.Forms
{
    public partial class OptionsForm : Form
    {
        public static string Json;
        public OptionsForm(string json)
        {
            InitializeComponent();
            ControlHelper.SetForm(this);
            ControlHelper.SetDrag(Dragbar);
            label1.ForeColor = Colors.TextHex();
            this.BackColor = Colors.MHex();
            Dragbar.BackColor = Colors.SHex();
            Json = json;
        }
        private void Control_Click(object sender, EventArgs e)
        {
            var Sender = (Control)sender;
            string CosmeticJson = string.Empty;
            switch (JObject.Parse(Json)["Type"].ToString())
            {
                case "Skin:Mesh":
                    CosmeticJson = Endpoint.APIReturn(Endpoint.Endpoints.Skins, null);
                    break;
                case "Skin:Default":
                    CosmeticJson = Endpoint.APIReturn(Endpoint.Endpoints.SpecialSkins, null);
                    break;
                case "Backblings":
                    CosmeticJson = Endpoint.APIReturn(Endpoint.Endpoints.Backblings, null);
                    break;
                case "Pickaxes":
                    CosmeticJson = Endpoint.APIReturn(Endpoint.Endpoints.Pickaxes, null);
                    break;
                case "Emote":
                    CosmeticJson = Endpoint.APIReturn(Endpoint.Endpoints.Emotes, null);
                    break;
                case "Wraps":
                    CosmeticJson = Endpoint.APIReturn(Endpoint.Endpoints.Wraps, null);
                    break;
                case "Misc":
                    CosmeticJson = Endpoint.APIReturn(Endpoint.Endpoints.Misc, null);
                    break;
            }
            foreach (var cosmetic in JArray.Parse(CosmeticJson))
            {
                if (cosmetic["Name"].ToString() == Sender.Name)
                {
                    this.WindowState = FormWindowState.Minimized;
                    new SwapForm(cosmetic.ToString()).ShowDialog();
                    this.Close();
                }
            }
        }
        public PictureBox CosmeticIcon(string IconURL, string Json)
        {
            PictureBox Icon = new PictureBox()
            {
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(int.Parse(SettingsController.ConfigReturn("IconSize")), int.Parse(SettingsController.ConfigReturn("IconSize"))),
                Name = Json
            };
            Icon.Click += Control_Click;
            ControlHelper.LoadImage(Icon, IconURL);
            return Icon;
        }
        private void OptionsForm_Load(object sender, EventArgs e)
        {
            JObject parse = JObject.Parse(Json);
            this.Name = parse["Name"].ToString();
            RPC.Update(this.Name);
            foreach (var Cosmetic in parse["Options"])
            {
                flowLayoutPanel1.Controls.Add(CosmeticIcon(Cosmetic["Icon"].ToString(), Cosmetic["Name"].ToString()));
            }
        }
        private void CloseBox_Click(object sender, EventArgs e) => this.Close();
    }
}
