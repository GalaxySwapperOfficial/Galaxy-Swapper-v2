using Galaxy_Swapper_v2.Workspace.Forms;
using Galaxy_Swapper_v2.Workspace.Other;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Galaxy_Swapper_v2.Workspace.Usercontrols
{
    public partial class Cosmetics : UserControl
    {
        string CosmeticType;
        public Cosmetics(string Cos)
        {
            InitializeComponent();
            CosmeticType = Cos;
            SearchBarPanel.Location = new Point(868, 0);
        }
        private void Control_Click(object sender, EventArgs e)
        {
            var Sender = (Control)sender;
            if (Sender.Name.ToLower().Contains("option"))
                new OptionsForm(Sender.Name).Show();
            else
                new SwapForm(Sender.Name).ShowDialog();
        }
        private void ExtraControl_Click(object sender, EventArgs e)
        {
            var Sender = (Control)sender;
            if (Sender.Name == "Plugin")
            {
                
            }
            else if (Sender.Name == "IDSwapper")
            {
                MessageBox.Show("ID Swapper Currently Unavailable Due To UE5 We Will Add This Feature Back Soon.");
            }
        }
        private void SearchIcon_Click(object sender, EventArgs e)
        {
            BackgroundWorker SearchTran = new BackgroundWorker();
            SearchTran.DoWork += delegate
            {
                if (SearchBarPanel.Location.X == 605)
                {
                    while (SearchBarPanel.Location.X != 868) { SearchBarPanel.Location = new Point(SearchBarPanel.Location.X + 1); }
                    SearchIcon.BackColor = Colors.SHex();
                }
                else
                {
                    while (SearchBarPanel.Location.X != 605) { SearchBarPanel.Location = new Point(SearchBarPanel.Location.X - 1); }
                    SearchIcon.BackColor = Color.Transparent;
                }
            };
            SearchTran.RunWorkerAsync();
        }
        private void Cosmetics_Load(object sender, EventArgs e)
        {
            switch (CosmeticType)
            {
                case "Skins":
                    {
                        var Flow = ControlHelper.Skins;
                        if (Flow.Controls.Count == 0)
                        {
                            Flow.Controls.Add(ControlHelper.Lbl("Mesh Skins | No Bypass!"));
                            CreateTab(Endpoint.APIReturn(Endpoint.Endpoints.Skins, null), "Skin:Mesh", Flow);
                            Flow.Controls.Add(ControlHelper.Lbl("Special Skins | Backblings Don't Show"));

                            if (bool.Parse(SettingsController.ConfigReturn("HideNonePerfectDefaults")) == true)
                            {
                                CreateTab(Endpoint.APIReturn(Endpoint.Endpoints.SpecialSkins, null), "Skin:Default", Flow);
                            }
                            else
                            {
                                foreach (var Cosmetic in JArray.Parse(Endpoint.APIReturn(Endpoint.Endpoints.SpecialSkins, null)))
                                {
                                    if (Cosmetic["Type"].ToString() == "Skin:Default")
                                    {
                                        if (Cosmetic["Message"].ToString() == "false")
                                            Flow.Controls.Add(CosmeticIcon(Cosmetic["Icon"].ToString(), Cosmetic.ToString()));
                                    }
                                }
                            }
                        }
                        panel1.Controls.Add(Flow);
                    }
                    return;
                case "Backblings":
                    {
                        var Flow = ControlHelper.Backblings;
                        if (Flow.Controls.Count == 0)
                        {
                            CreateTab(Endpoint.APIReturn(Endpoint.Endpoints.Backblings, null), "Backblings", Flow);
                        }
                        panel1.Controls.Add(Flow);
                    }
                    return;
                case "Pickaxes":
                    {
                        var Flow = ControlHelper.Pickaxes;
                        if (Flow.Controls.Count == 0)
                        {
                            CreateTab(Endpoint.APIReturn(Endpoint.Endpoints.Pickaxes, null), "Pickaxes", Flow);
                        }
                        panel1.Controls.Add(Flow);
                    }
                    return;
                case "Emotes":
                    {
                        var Flow = ControlHelper.Emotes;
                        if (Flow.Controls.Count == 0)
                        {
                            CreateTab(Endpoint.APIReturn(Endpoint.Endpoints.Emotes, null), "Emote", Flow);
                        }
                        panel1.Controls.Add(Flow);
                    }
                    return;
                case "Wraps":
                    {
                        var Flow = ControlHelper.Wraps;
                        if (Flow.Controls.Count == 0)
                        {
                            CreateTab(Endpoint.APIReturn(Endpoint.Endpoints.Wraps, null), "Wraps", Flow);
                        }
                        panel1.Controls.Add(Flow);
                    }
                    return;
                case "Misc":
                    {
                        var Flow = ControlHelper.Misc;
                        if (Flow.Controls.Count == 0)
                        {
                            CreateTab(Endpoint.APIReturn(Endpoint.Endpoints.Misc, null), "Misc", Flow);
                        }
                        panel1.Controls.Add(Flow);
                    }
                    return;
            }
        }
        public void CreateTab(string API, string Type, FlowLayoutPanel Flow)
        {
            foreach (var Cosmetic in JArray.Parse(API))
            {
                if (Cosmetic["Type"].ToString() == Type)
                {
                    Flow.Controls.Add(CosmeticIcon(Cosmetic["Icon"].ToString(), Cosmetic.ToString()));
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
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (richTextBox1.Text.Length == 0)
            {
                panel1.Controls.Clear();
                Cosmetics_Load(this, EventArgs.Empty);
            }
        }
        public JArray CosmeticSort(string Name, string Json)
        {
            JArray Final = new JArray();
            foreach(var Cosmetic in JArray.Parse(Json))
            {
                if (Cosmetic["Name"].ToString().ToLower().Contains(Name.ToLower()))
                {
                    if (!Cosmetic["Type"].ToString().ToLower().Contains("option"))
                    {
                        Final.Add(Cosmetic);
                    }
                }
            }
            return Final;
        }
        private void richTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (richTextBox1.Text.Length != 0)
            {
                if (e.KeyChar == Convert.ToChar(Keys.Enter))
                {
                    JArray SortedCosmetics = new JArray();
                    SortedCosmetics.Merge(CosmeticSort(richTextBox1.Text, Endpoint.APIReturn(Endpoint.Endpoints.Skins, null)));
                    SortedCosmetics.Merge(CosmeticSort(richTextBox1.Text, Endpoint.APIReturn(Endpoint.Endpoints.SpecialSkins, null)));
                    SortedCosmetics.Merge(CosmeticSort(richTextBox1.Text, Endpoint.APIReturn(Endpoint.Endpoints.Backblings, null)));
                    SortedCosmetics.Merge(CosmeticSort(richTextBox1.Text, Endpoint.APIReturn(Endpoint.Endpoints.Pickaxes, null)));
                    SortedCosmetics.Merge(CosmeticSort(richTextBox1.Text, Endpoint.APIReturn(Endpoint.Endpoints.Emotes, null)));
                    SortedCosmetics.Merge(CosmeticSort(richTextBox1.Text, Endpoint.APIReturn(Endpoint.Endpoints.Wraps, null)));
                    SortedCosmetics.Merge(CosmeticSort(richTextBox1.Text, Endpoint.APIReturn(Endpoint.Endpoints.Misc, null)));
                    panel1.Controls.Clear();
                    FlowLayoutPanel SortedFlow = new FlowLayoutPanel()
                    {
                        AutoScroll = true,
                        Dock = DockStyle.Fill
                    };
                    foreach (var Cosmetic in SortedCosmetics)
                    {
                        SortedFlow.Controls.Add(CosmeticIcon(Cosmetic["Icon"].ToString(), Cosmetic.ToString()));
                    }
                    panel1.Controls.Add(SortedFlow);
                }
            }
        }
    }
}
