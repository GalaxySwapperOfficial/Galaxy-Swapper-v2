using Galaxy_Swapper_v2.Workspace.Other;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Galaxy_Swapper_v2.Workspace.Other.Endpoint;

namespace Galaxy_Swapper_v2.Workspace.Usercontrols
{
    public partial class Dashboard : UserControl
    {
        public Dashboard()
        {
            InitializeComponent();
            foreach (Control x in this.Controls)
            {
                if (x is Label) { x.ForeColor = Colors.TextHex(); }
            }
            label6.BackColor = Colors.ButtonHex();
            label6.ForeColor = Colors.TextHex();
            label7.BackColor = Colors.ButtonHex();
            label7.ForeColor = Colors.TextHex();
        }
        private void Dashboard_Load(object sender, EventArgs e)
        {
            Task.Run(() => {
                CheckForIllegalCrossThreadCalls = false;
                label5.Text = APIReturn(Endpoints.Languages, "Credit");
                label6.Text = APIReturn(Endpoints.Languages, "PatchNotes");

                JArray SwapLogsFile = JArray.Parse(File.ReadAllText(Global.Appdata + "\\Galaxy Swapper v2\\Swaps.log"));
                string Message = string.Empty;
                foreach (var Swap in SwapLogsFile)
                {
                    Message += $"{Swap["Name"]}{Environment.NewLine}";
                }
                if (Message.Contains("To"))
                    label7.Text = Message;
                else
                    label7.Text = Endpoint.APIReturn(Endpoints.Languages, "NoSwappedLogsMsg");
                JObject parse = JObject.Parse(FortniteAPI.APIReturn(FortniteAPI.Endpoints.News, null));
                if (parse["data"]["br"]["image"] == null)
                    pictureBox1.ImageLocation = "https://www.galaxyswapperv2.com/Icons/InvalidIcon.png";
                else
                    pictureBox1.ImageLocation = parse["data"]["br"]["image"].ToString();
            });
        }
    }
}
