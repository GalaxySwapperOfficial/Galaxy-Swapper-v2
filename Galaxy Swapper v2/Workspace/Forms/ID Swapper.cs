using Galaxy_Swapper_v2.Properties;
using Galaxy_Swapper_v2.Workspace.Other;
using Ionic.Zlib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static Galaxy_Swapper_v2.Workspace.Other.Endpoint;

namespace Galaxy_Swapper_v2.Workspace.Forms
{
    public partial class ID_Swapper : Form
    {
        public static string CID;
        public static int Offset;
        public static int CompressedSize;
        public static byte[] Uncompressed;
        public static byte[] Compressed;
        public ID_Swapper()
        {
            InitializeComponent();
            ControlHelper.SetForm(this);
            ControlHelper.SetDrag(panel1);
            ControlHelper.SetDrag(Dragbar);
            LoadColors();
        }
        private void ID_Swapper_Load(object sender, EventArgs e)
        {
            JObject parse = JObject.Parse(Endpoint.APIReturn(Endpoint.Endpoints.IDSwapper, null));
            List<string> FilesUsed = new List<string>();
            foreach (var Swaps in JArray.Parse(File.ReadAllText(Global.Appdata + "\\Galaxy Swapper v2\\Swaps.log")))
            {
                foreach (var Files in JArray.Parse(Swaps["Files"].ToString()))
                {
                    if (!FilesUsed.Contains(Files["File"].ToString()))
                    {
                        FilesUsed.Add(Files["File"].ToString());
                    }
                }
            }
            if (!FilesUsed.Contains(parse["AssetRegUcas"].ToString()))
            {
                FilesUsed.Add(parse["AssetRegUcas"].ToString());
            }
            if (FilesUsed.Count > 2)
            {
                MessageBox.Show(this, Endpoint.APIReturn(Endpoints.Languages, "ToManyCosmetics"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }
            label2.Click += delegate
            {
                Global.UrlStart(Endpoint.APIReturn(Endpoints.IDSwapper, "Tutorial"));
            };
            label3.Click += delegate
            {
                Global.UrlStart(Endpoint.APIReturn(Endpoints.IDSwapper, "IDList"));
            };
            textBox1.TextChanged += delegate
            {
                label5.Text = textBox1.Text.Length.ToString();
            };
            textBox2.TextChanged += delegate
            {
                label6.Text = textBox2.Text.Length.ToString();
            };
            textBox1.Text = Settings.Default.SearchCID;
            textBox2.Text = Settings.Default.ReplaceCID;
        }
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
                if (x is TextBox)
                {
                    x.BackColor = Colors.ButtonHex();
                    x.ForeColor = Colors.TextHex();
                }
            }
        }
        private void CloseBox_Click(object sender, EventArgs e) => this.Close();
        private void Control_Click(object sender, EventArgs e)
        {
            var Sender = (Control)sender;
            JObject parse = JObject.Parse(FortniteAPI.APIReturn(FortniteAPI.Endpoints.Aes, null));
            if (parse["data"]["mainKey"] == null)
            {
                MessageBox.Show(this, "ID Swapper Is Currently Down! Please Try Again Later.", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (textBox1.Text.Length < textBox2.Text.Length)
            {
                MessageBox.Show(this, "Search String Is Longer Then Replace!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (textBox1.Text.Length == 0)
            {
                MessageBox.Show(this, "Search String Is Blank!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (textBox2.Text.Length == 0)
            {
                MessageBox.Show(this, "Replace String Is Blank!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using (BackgroundWorker ConvertWorker = new BackgroundWorker())
            {
                switch (Sender.Name)
                {
                    case "ConvertButton":
                        ConvertWorker.DoWork += Swap_DoWork;
                        break;
                    case "RevertButton":
                        ConvertWorker.DoWork += Revert_DoWork;
                        break;
                }
                ConvertWorker.RunWorkerAsync();
            }
        }
        public void DupeFiles(string FileName)
        {
            string[] Exenstions = new string[]
            {
            ".pak",
            ".sig"
            };
            if (!Directory.Exists(Settings.Default.FortniteInstall + "\\~Galaxy Swapper v2")) { Directory.CreateDirectory(Settings.Default.FortniteInstall + "\\~Galaxy Swapper v2"); }
            foreach (string Ex in Exenstions)
            {
                if (!File.Exists(Settings.Default.FortniteInstall + $"\\~Galaxy Swapper v2\\{FileName}{Ex}"))
                {
                    File.Copy(Settings.Default.FortniteInstall + $"\\{FileName}{Ex}", Settings.Default.FortniteInstall + $"\\~Galaxy Swapper v2\\{FileName}{Ex}");
                }
            }
        }
        private void Swap_DoWork(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            Stopwatch TotalTime = new Stopwatch();
            TotalTime.Start();
            JObject parse = JObject.Parse(Endpoint.APIReturn(Endpoint.Endpoints.IDSwapper, null));
            CID = $"{textBox1.Text}.{textBox1.Text}";
            Cue4parse.ExportAsset(parse["AssetRegUcas"].ToString(), parse["AssetReg"].ToString());
            DupeFiles(parse["AssetRegUcas"].ToString());

            List<byte> Asset = new List<byte>(Uncompressed);

            byte[] SearchByte = Encoding.ASCII.GetBytes($"{textBox1.Text}.{textBox1.Text}");
            byte[] ReplaceByte = Encoding.ASCII.GetBytes($"{textBox2.Text}.{textBox2.Text}");

            int SearchPos = SwapUtils.IndexOfSequence(Asset.ToArray(), SearchByte);
            Asset.RemoveRange(SearchPos, SearchByte.Length);
            Asset.InsertRange(SearchPos, SwapUtils.MatchToByte(ReplaceByte, SearchByte.Length));

            byte[] Compressed = ZlibCompress(Asset.ToArray());
            Compressed = SwapUtils.MatchToByte(Compressed, CompressedSize);

            using (FileStream UcasEdit = File.Open(Settings.Default.FortniteInstall + $"\\~Galaxy Swapper v2\\{parse["AssetRegUcas"]}.pak", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                UcasEdit.Position = Offset;
                UcasEdit.Write(Compressed, 0, Compressed.Length);
                UcasEdit.Close();
            }
            SwapLogController(parse["AssetRegUcas"].ToString(), "add");
            TimeSpan st = TotalTime.Elapsed;
            MessageBox.Show(this, $"Successfully Converted In {st.Seconds} Seconds", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void Revert_DoWork(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            Stopwatch TotalTime = new Stopwatch();
            TotalTime.Start();
            JObject parse = JObject.Parse(Endpoint.APIReturn(Endpoint.Endpoints.IDSwapper, null));
            if (File.Exists(Settings.Default.FortniteInstall + $"\\~Galaxy Swapper v2\\{parse["AssetRegUcas"]}.pak"))
            {
                CID = $"{textBox1.Text}.{textBox1.Text}";
                Cue4parse.ExportAsset(parse["AssetRegUcas"].ToString(), parse["AssetReg"].ToString());
                DupeFiles(parse["AssetRegUcas"].ToString());

                using (FileStream UcasEdit = File.Open(Settings.Default.FortniteInstall + $"\\~Galaxy Swapper v2\\{parse["AssetRegUcas"]}.pak", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    UcasEdit.Position = Offset;
                    UcasEdit.Write(Compressed, 0, Compressed.Length);
                    UcasEdit.Close();
                }
            }
            SwapLogController(null, "remove");
            TimeSpan st = TotalTime.Elapsed;
            MessageBox.Show(this, $"Successfully Reverted In {st.Seconds} Seconds", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public static byte[] ZlibCompress(byte[] input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (ZlibStream zls = new ZlibStream(ms, CompressionMode.Compress, CompressionLevel.Level7))
                {
                    zls.Write(input, 0, input.Length);
                }
                return ms.ToArray();
            }
        }
        public void SwapLogController(string Paks, string Func)
        {
            JArray SwapLogsFile = JArray.Parse(File.ReadAllText(Global.Appdata + "\\Galaxy Swapper v2\\Swaps.log"));
            if (Func == "add")
            {
                JArray Files = new JArray();
                JObject pakformat = JObject.FromObject(new
                {
                    File = Paks
                });
                Files.Add(pakformat);
                JObject json = JObject.FromObject(new
                {
                    Name = textBox1.Text + " To " + textBox2.Text,
                    Files
                });
                SwapLogsFile.Add(json);
                Settings.Default.SearchCID = textBox1.Text;
                Settings.Default.ReplaceCID = textBox2.Text;
                Settings.Default.Save();
                File.WriteAllText(Global.Appdata + "\\Galaxy Swapper v2\\Swaps.log", SwapLogsFile.ToString());
            }
            else
            {
                foreach (var Swaps in SwapLogsFile)
                {
                    if (Swaps["Name"].ToString() == textBox1.Text + " To " + textBox2.Text)
                    {
                        SwapLogsFile.Remove(Swaps);
                        File.WriteAllText(Global.Appdata + "\\Galaxy Swapper v2\\Swaps.log", SwapLogsFile.ToString());
                        return;
                    }
                }
            }
        }
    }
}
