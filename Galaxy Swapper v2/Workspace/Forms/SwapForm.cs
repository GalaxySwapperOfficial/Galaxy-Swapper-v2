using Galaxy_Swapper_v2.Properties;
using Galaxy_Swapper_v2.Workspace.Other;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Galaxy_Swapper_v2.Workspace.Other.Endpoint;

namespace Galaxy_Swapper_v2.Workspace.Forms
{
    public partial class SwapForm : Form
    {
        public static string Json;
        public SwapForm(string TempJson)
        {
            InitializeComponent();
            ControlHelper.SetForm(this);
            ControlHelper.SetDrag(panel1);
            ControlHelper.SetDrag(Dragbar);
            Json = TempJson;
            LoadColors();
        }
        private void CloseBox_Click(object sender, EventArgs e) => this.Close();
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
        private void SwapForm_Load(object sender, EventArgs e)
        {
            JObject parse = JObject.Parse(Json);
            SearchI.ImageLocation = parse["Swapicon"].ToString();
            ReplaceI.ImageLocation = parse["Icon"].ToString();
            CosmeticNameLabel.Text = parse["Name"].ToString();
            this.Name = parse["Name"].ToString();
            RPC.Update($"{this.Name.Substring(this.Name.LastIndexOf("To ")).Replace("To", "")}");
            Task.Run(() => {
                Global.CloseFortnite();
                if (parse["Message"].ToString() != "false")
                    MessageBox.Show(this, parse["Message"].ToString(), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
            SwappedActive.ForeColor = Color.Red;
            List<string> FilesUsed = new List<string>();
            foreach (var Swaps in JArray.Parse(File.ReadAllText(Global.Appdata + "\\Galaxy Swapper v2\\Swaps.log")))
            {
                if (Swaps["Name"].ToString() == parse["Name"].ToString())
                {
                    SwappedActive.ForeColor = Color.Yellow;
                    SwappedActive.Text = "ON";
                }
                foreach (var Files in JArray.Parse(Swaps["Files"].ToString()))
                {
                    if (!FilesUsed.Contains(Files["File"].ToString()))
                    {
                        FilesUsed.Add(Files["File"].ToString());
                    }
                }
            }
            foreach (var Assets in JObject.Parse(Json)["Assets"])
            {
                if (!FilesUsed.Contains(Assets["AssetUcas"].ToString()))
                {
                    FilesUsed.Add(Assets["AssetUcas"].ToString());
                }
            }
            if (FilesUsed.Count > 2)
            {
                MessageBox.Show(this, Endpoint.APIReturn(Endpoints.Languages, "ToManyCosmetics"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }
        }
        private void Control_Click(object sender, EventArgs e)
        {
            var Sender = (Control)sender;
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
        public void LOG(string Content) => Logs.Text = $"LOG : {Content}";
        private void Swap_DoWork(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            Stopwatch TotalTime = new Stopwatch();
            TotalTime.Start();
            int Assetcount = 1;
            List<string> Paks = new List<string>();
            foreach (var Assets in JObject.Parse(Json)["Assets"])
            {
                LOG($"Exporting Asset {Assetcount}");
                List<byte> Asset = new List<byte>(Cue4parse.ExportAsset(Assets["AssetUcas"].ToString(), Assets["AssetPath"].ToString()));
                LOG($"Swapping Asset {Assetcount} Strings");
                foreach (var Swap in Assets["Swaps"])
                {
                    string Search = Swap["search"].ToString();
                    string Replace = Swap["replace"].ToString();
                    switch (Swap["type"].ToString())
                    {
                        case "string":
                            {
                                byte[] SearchByte = Encoding.ASCII.GetBytes(Search);
                                byte[] ReplaceByte = Encoding.ASCII.GetBytes(Replace);
                                int SearchPos = SwapUtils.IndexOfSequence(Asset.ToArray(), SearchByte);
                                //If Offset Is Postive We Will Swap
                                if (SearchPos > 0)
                                {
                                    if (SearchByte.Length < ReplaceByte.Length) //Any Length
                                    {
                                        int SearchLengthPos = SwapUtils.IndexOfSequence(Asset.ToArray(), SwapUtils.ByteLength(SearchByte));
                                        Asset.RemoveRange(SearchLengthPos, 1); //Removes Our Search Length Value
                                        Asset.InsertRange(SearchLengthPos, SwapUtils.ByteLength(ReplaceByte)); //Inserts Our New Length Value
                                        Asset.RemoveRange(SearchPos, SearchByte.Length); //Remove Our Search String
                                        Asset.InsertRange(SearchPos, ReplaceByte); //Inserts Our Replace String
                                        Asset.RemoveRange(SwapUtils.IndexOfSequence(Asset.ToArray(), new byte[] { 248, 112 }) - 25, ReplaceByte.Length - SearchByte.Length); //Removes Uexp To Match Orginal File Size This Any Length Code Is Not Updated To New (New Is Private)
                                    }
                                    else //Normal
                                    {
                                        Asset.RemoveRange(SearchPos, SearchByte.Length); //Remove Our Search String
                                        Asset.InsertRange(SearchPos, SwapUtils.MatchToByte(ReplaceByte, SearchByte.Length)); //Inseart Our Replace String
                                    }
                                }
                            }
                            break;
                        case "hex":
                            {
                                byte[] SearchByte = SwapUtils.HexToByte(Swap["search"].ToString());
                                byte[] ReplaceByte = SwapUtils.HexToByte(Swap["replace"].ToString());
                                int SearchPos = SwapUtils.IndexOfSequence(Asset.ToArray(), SearchByte);
                                //If Offset Is Postive We Will Swap
                                if (SearchPos > 0)
                                {
                                    Asset.RemoveRange(SearchPos, SearchByte.Length); //Removes Our Search Hex
                                    Asset.InsertRange(SearchPos, ReplaceByte); //Inserts Our Replace Hex
                                }
                            }
                            break;
                    }
                }
                byte[] FinalAsset = Asset.ToArray();
                if (Global.Compressed == true) //If Asset Is Oodle Then We Shall Compress It
                {
                    FinalAsset = SwapUtils.InvalidScript(FinalAsset.ToArray());
                    FinalAsset = Oodle.Compress(FinalAsset);
                    if (FinalAsset.Length > Global.CompressedLength)
                    {
                        MessageBox.Show(this, "Search Byte Is Longer Then Replace Byte! Please Verify Fortnite In Galaxy Swapper Settings And Try Again.", "Caught A Error Please Read!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    FinalAsset = SwapUtils.MatchToByte(FinalAsset, Global.CompressedLength);
                }
                DupeFiles();
                LOG($"Applying Asset {Assetcount}");
                using (FileStream UcasEdit = File.Open(Settings.Default.FortniteInstall + $"\\Galaxy Swapper v2\\{Path.GetFileName(Global.Pak)}", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    UcasEdit.Position = Global.Offset;
                    UcasEdit.Write(FinalAsset, 0, FinalAsset.Length);
                    UcasEdit.Close();
                }
                Assetcount++;
                Paks.Add(Path.GetFileNameWithoutExtension(Global.Pak));
                LOG($"Swapped Asset {Assetcount}");
            }
            SwapLogController(Paks, "add");
            TimeSpan st = TotalTime.Elapsed;
            LOG($"Completed In {st.Seconds} Seconds");
            MessageBox.Show(this, $"Successfully Converted In {st.Seconds} Seconds", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void Revert_DoWork(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            Stopwatch TotalTime = new Stopwatch();
            TotalTime.Start();
            int Assetcount = 1;
            foreach (var Assets in JObject.Parse(Json)["Assets"])
            {
                LOG($"Exporting Asset {Assetcount}");
                Cue4parse.ExportAsset(Assets["AssetUcas"].ToString(), Assets["AssetPath"].ToString());
                if (!File.Exists(Settings.Default.FortniteInstall + "\\Galaxy Swapper v2\\" + Path.GetFileName(Global.Pak)))
                {
                    LOG($"Asset {Assetcount} Was Not Swapped");
                    break;
                }

                LOG($"Reverting Asset {Assetcount}");
                using (FileStream UcasEdit = File.Open(Settings.Default.FortniteInstall + $"\\Galaxy Swapper v2\\{Path.GetFileName(Global.Pak)}", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    UcasEdit.Position = Global.Offset;
                    UcasEdit.Write(Global.CompressedBytes, 0, Global.CompressedBytes.Length);
                    UcasEdit.Close();
                }
                Assetcount++;
                LOG($"Reverted Asset {Assetcount}");
            }
            SwapLogController(null, "remove");
            TimeSpan st = TotalTime.Elapsed;
            LOG($"Completed In {st.Seconds} Seconds");
            MessageBox.Show(this, $"Successfully Reverted In {st.Seconds} Seconds", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public void DupeFiles()
        {
            string[] Exenstions = new string[]
            {
            ".ucas",
            ".utoc",
            ".pak",
            ".sig"
            };
            if (!Directory.Exists(Settings.Default.FortniteInstall + "\\Galaxy Swapper v2")) { Directory.CreateDirectory(Settings.Default.FortniteInstall + "\\Galaxy Swapper v2"); }
            foreach (string Ex in Exenstions)
            {
                string FileName = Path.GetFileNameWithoutExtension(Global.Pak);
                if (!File.Exists(Settings.Default.FortniteInstall + $"\\Galaxy Swapper v2\\{FileName}{Ex}"))
                {
                    LOG($"Backing Up {Path.GetFileName(Global.Pak)}..");
                    File.Copy(Settings.Default.FortniteInstall + $"\\{FileName}{Ex}", Settings.Default.FortniteInstall + $"\\Galaxy Swapper v2\\{FileName}{Ex}");
                }
            }
        }
        public void SwapLogController(List<string> Paks, string Func)
        {
            JArray SwapLogsFile = JArray.Parse(File.ReadAllText(Global.Appdata + "\\Galaxy Swapper v2\\Swaps.log"));
            if (Func == "add")
            {
                JArray Files = new JArray();
                foreach (string Pak in Paks)
                {
                    JObject pakformat = JObject.FromObject(new
                    {
                        File = Pak
                    });
                    Files.Add(pakformat);
                }
                JObject json = JObject.FromObject(new
                {
                    Name = JObject.Parse(Json)["Name"],
                    Files
                });
                SwapLogsFile.Add(json);
                File.WriteAllText(Global.Appdata + "\\Galaxy Swapper v2\\Swaps.log", SwapLogsFile.ToString());
                SwappedActive.ForeColor = Color.Yellow;
                SwappedActive.Text = "ON";
            }
            else
            {
                foreach (var Swaps in SwapLogsFile)
                {
                    if (Swaps["Name"].ToString() == JObject.Parse(Json)["Name"].ToString())
                    {
                        SwapLogsFile.Remove(Swaps);
                        File.WriteAllText(Global.Appdata + "\\Galaxy Swapper v2\\Swaps.log", SwapLogsFile.ToString());
                        SwappedActive.ForeColor = Color.Red;
                        SwappedActive.Text = "OFF";
                        return;
                    }
                }
            }
        }
    }
    public static class SwapUtils
    {
        public static byte[] MatchToByte(byte[] content, int ByeLength)
        {
            List<byte> result = new List<byte>(content);
            int difference = ByeLength - content.Length;
            for (int i = 0; i < difference; i++)
                result.Add(0);
            return result.ToArray();
        }
        public static int IndexOfSequence(byte[] buffer, byte[] pattern)
        {
            //CREDIT: https://stackoverflow.com/a/31107925
            int i = Array.IndexOf(buffer, pattern[0], 0);
            while (i >= 0 && i <= buffer.Length - pattern.Length)
            {
                byte[] segment = new byte[pattern.Length];
                Buffer.BlockCopy(buffer, i, segment, 0, pattern.Length);
                if (segment.SequenceEqual(pattern))
                    return i;
                i = Array.IndexOf(buffer, pattern[0], i + 1);
            }
            return -1;
        }
        public static byte[] HexToByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            byte[] data = new byte[hexString.Length / 2];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }
            return data;
        }
        public static byte[] ByteLength(byte[] bYtE)
        {
            List<byte> Asset = new List<byte>();
            Asset.Add(Convert.ToByte(bYtE.Length));
            return Asset.ToArray();
        }
        public static byte[] InvalidScript(byte[] Asset)
        {
            List<byte> AssetBytes = new List<byte>(Asset);
            string[] Scripts = new string[]
            {
                        "CustomCharacterBodyPartData",
                        "FaceCustomCharacterHatData",
                        "CustomCharacterHatData",
                        "CustomCharacterHeadData"
            };
            foreach (string Scwipt in Scripts)
            {
                byte[] Search = Encoding.ASCII.GetBytes(Scwipt);
                byte[] Replace = Encoding.ASCII.GetBytes("-");
                int offset = SwapUtils.IndexOfSequence(Asset.ToArray(), Search);
                if (offset > 0)
                {
                    AssetBytes.RemoveRange(offset, Search.Length);
                    AssetBytes.InsertRange(offset, SwapUtils.MatchToByte(Replace, Search.Length));
                }
            }
            return AssetBytes.ToArray();
        }
    }
}
