using Siticone.UI.WinForms;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;

namespace Galaxy_Swapper_v2.Workspace.Other
{
    public static class ControlHelper
    {
        public static void SetForm(Form Form)
        {
            SetEclipse(Form, 9);
            SetDrag(Form);
            SetShadowForm(Form);
            Form.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
        }
        public static void SetEclipse(Control x, int Amount)
        {
            SiticoneElipse Eclipse = new SiticoneElipse();
            Eclipse.BorderRadius = Amount;
            Eclipse.SetElipse(x);
        }
        public static void SetShadowForm(Form x)
        {
            SiticoneShadowForm ShadowForm = new SiticoneShadowForm();
            ShadowForm.SetShadowForm(x);
        }
        public static void SetDrag(Control x)
        {
            SiticoneDragControl Drag = new SiticoneDragControl();
            Drag.SetDrag(x);
        }
        public static void ButtonHover(Control Button)
        {
            Button.MouseEnter += MouseEnter;
            Button.MouseLeave += MouseLeave;
            SetEclipse(Button, 9);
        }
        public static void MouseEnter(object sender, EventArgs e)
        {
            var lbl = sender as Label;
            lbl.BackColor = Color.FromArgb(69, 74, 107);
        }
        public static void MouseLeave(object sender, EventArgs e)
        {
            var lbl = sender as Label;
            lbl.BackColor = Colors.ButtonHex();
        }
        /*
         *  using (WebClient wc = new WebClient())
                {
                    using (Stream s = wc.OpenRead(ImageURL))
                    {
                        using (Bitmap srce = new Bitmap(s))
                        {
                            var dest = new Bitmap(Pic.Width, Pic.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                            using (var gr = Graphics.FromImage(dest))
                            {
                                gr.SmoothingMode = SmoothingMode.None;
                                gr.InterpolationMode = InterpolationMode.Low;
                                gr.CompositingQuality = CompositingQuality.HighSpeed;
                                gr.PixelOffsetMode = PixelOffsetMode.None;
                                gr.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                                gr.DrawImage(srce, new Rectangle(Point.Empty, dest.Size));
                            }
                            Pic.Image = dest;
                        }
                    }
                }
         */
        public static void LoadImage(PictureBox Pic, string ImageURL)
        {
            BackgroundWorker ImageLoader = new BackgroundWorker();
            ImageLoader.DoWork += delegate
            {
                Pic.ImageLocation = ImageURL;
            };
            ImageLoader.RunWorkerAsync();
        }
        public static Label Lbl(string Text)
        {
            Label label = new Label()
            {
                Text = Text,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Colors.TextHex(),
                Size = new Size(820, 38)
            };
            label.Font = new Font(label.Font.FontFamily, 13);
            return label;
        }
        public static FlowLayoutPanel Skins = new FlowLayoutPanel()
        {
            AutoScroll = true,
            Dock = DockStyle.Fill
        };
        public static FlowLayoutPanel Backblings = new FlowLayoutPanel()
        {
            AutoScroll = true,
            Dock = DockStyle.Fill
        };
        public static FlowLayoutPanel Pickaxes = new FlowLayoutPanel()
        {
            AutoScroll = true,
            Dock = DockStyle.Fill
        };
        public static FlowLayoutPanel Emotes = new FlowLayoutPanel()
        {
            AutoScroll = true,
            Dock = DockStyle.Fill
        };
        public static FlowLayoutPanel Wraps = new FlowLayoutPanel()
        {
            AutoScroll = true,
            Dock = DockStyle.Fill
        };
        public static FlowLayoutPanel Misc = new FlowLayoutPanel()
        {
            AutoScroll = true,
            Dock = DockStyle.Fill
        };
    }
}
