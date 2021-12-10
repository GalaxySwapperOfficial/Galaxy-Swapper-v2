using Galaxy_Swapper_v2.Workspace.Other;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using static Galaxy_Swapper_v2.Workspace.Other.Endpoint;

namespace Galaxy_Swapper_v2.Workspace.Forms
{
    public partial class Login_Page : Form
    {
        public Login_Page()
        {
            InitializeComponent();
            ControlHelper.SetForm(this);
            ControlHelper.SetDrag(Dragbar);
            LoadColors();
        }
        private void CloseBox_Click(object sender, EventArgs e) => Environment.Exit(0);
        public void LoadColors()
        {
            this.BackColor = Colors.MHex();
            Dragbar.BackColor = Colors.SHex();
            foreach (Control x in this.Controls)
            {
                if (x is Label)
                {
                    if (x.BackColor != Color.Transparent) { x.BackColor = Colors.ButtonHex(); ControlHelper.ButtonHover(x); }
                    x.ForeColor = Colors.TextHex();
                }
            }
        }
        public static bool CheckKey()
        {
           //Not Provided
        }
        public static void AddDays(int Amount)
        {
           //Not Provided
        }
        private void label3_Click(object sender, EventArgs e)
        {
           //Not Provided
        }
        private void label5_Click(object sender, EventArgs e) => Global.UrlStart(Endpoint.APIReturn(Endpoints.Status, "LinkvertiseLink"));
        private void label4_Click(object sender, EventArgs e) => Global.UrlStart(Endpoint.APIReturn(Endpoints.Status, "Discord"));
    }
}
