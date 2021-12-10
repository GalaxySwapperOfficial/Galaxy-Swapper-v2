
namespace Galaxy_Swapper_v2.Workspace.Forms
{
    partial class SwapForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SwapForm));
            this.Logs = new System.Windows.Forms.Label();
            this.RevertButton = new System.Windows.Forms.Label();
            this.ConvertButton = new System.Windows.Forms.Label();
            this.CosmeticNameLabel = new System.Windows.Forms.Label();
            this.SwappedActive = new System.Windows.Forms.Label();
            this.ArrowIconImage = new System.Windows.Forms.PictureBox();
            this.ReplaceI = new System.Windows.Forms.PictureBox();
            this.SearchI = new System.Windows.Forms.PictureBox();
            this.Dragbar = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.CloseBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.ArrowIconImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ReplaceI)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SearchI)).BeginInit();
            this.Dragbar.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CloseBox)).BeginInit();
            this.SuspendLayout();
            // 
            // Logs
            // 
            this.Logs.BackColor = System.Drawing.Color.Transparent;
            this.Logs.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.Logs.ForeColor = System.Drawing.Color.White;
            this.Logs.Location = new System.Drawing.Point(13, 297);
            this.Logs.Name = "Logs";
            this.Logs.Size = new System.Drawing.Size(329, 22);
            this.Logs.TabIndex = 45;
            this.Logs.Text = "LOG : Waiting For Input..";
            this.Logs.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // RevertButton
            // 
            this.RevertButton.BackColor = System.Drawing.Color.White;
            this.RevertButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.RevertButton.ForeColor = System.Drawing.Color.White;
            this.RevertButton.Location = new System.Drawing.Point(13, 247);
            this.RevertButton.Name = "RevertButton";
            this.RevertButton.Size = new System.Drawing.Size(326, 39);
            this.RevertButton.TabIndex = 47;
            this.RevertButton.Text = "Revert";
            this.RevertButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.RevertButton.Click += new System.EventHandler(this.Control_Click);
            // 
            // ConvertButton
            // 
            this.ConvertButton.BackColor = System.Drawing.Color.White;
            this.ConvertButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ConvertButton.ForeColor = System.Drawing.Color.White;
            this.ConvertButton.Location = new System.Drawing.Point(13, 199);
            this.ConvertButton.Name = "ConvertButton";
            this.ConvertButton.Size = new System.Drawing.Size(326, 39);
            this.ConvertButton.TabIndex = 46;
            this.ConvertButton.Text = "Convert";
            this.ConvertButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ConvertButton.Click += new System.EventHandler(this.Control_Click);
            // 
            // CosmeticNameLabel
            // 
            this.CosmeticNameLabel.BackColor = System.Drawing.Color.Transparent;
            this.CosmeticNameLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.CosmeticNameLabel.ForeColor = System.Drawing.Color.White;
            this.CosmeticNameLabel.Location = new System.Drawing.Point(13, 174);
            this.CosmeticNameLabel.Name = "CosmeticNameLabel";
            this.CosmeticNameLabel.Size = new System.Drawing.Size(326, 22);
            this.CosmeticNameLabel.TabIndex = 48;
            this.CosmeticNameLabel.Text = "N/A";
            this.CosmeticNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SwappedActive
            // 
            this.SwappedActive.BackColor = System.Drawing.Color.Transparent;
            this.SwappedActive.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.SwappedActive.ForeColor = System.Drawing.Color.Red;
            this.SwappedActive.Location = new System.Drawing.Point(147, 149);
            this.SwappedActive.Name = "SwappedActive";
            this.SwappedActive.Size = new System.Drawing.Size(59, 22);
            this.SwappedActive.TabIndex = 52;
            this.SwappedActive.Text = "OFF";
            this.SwappedActive.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ArrowIconImage
            // 
            this.ArrowIconImage.Image = ((System.Drawing.Image)(resources.GetObject("ArrowIconImage.Image")));
            this.ArrowIconImage.Location = new System.Drawing.Point(146, 73);
            this.ArrowIconImage.Name = "ArrowIconImage";
            this.ArrowIconImage.Size = new System.Drawing.Size(60, 59);
            this.ArrowIconImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ArrowIconImage.TabIndex = 51;
            this.ArrowIconImage.TabStop = false;
            // 
            // ReplaceI
            // 
            this.ReplaceI.Location = new System.Drawing.Point(206, 45);
            this.ReplaceI.Name = "ReplaceI";
            this.ReplaceI.Size = new System.Drawing.Size(133, 126);
            this.ReplaceI.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ReplaceI.TabIndex = 50;
            this.ReplaceI.TabStop = false;
            // 
            // SearchI
            // 
            this.SearchI.Location = new System.Drawing.Point(13, 45);
            this.SearchI.Name = "SearchI";
            this.SearchI.Size = new System.Drawing.Size(133, 126);
            this.SearchI.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.SearchI.TabIndex = 49;
            this.SearchI.TabStop = false;
            // 
            // Dragbar
            // 
            this.Dragbar.Controls.Add(this.panel1);
            this.Dragbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.Dragbar.Location = new System.Drawing.Point(0, 0);
            this.Dragbar.Name = "Dragbar";
            this.Dragbar.Size = new System.Drawing.Size(354, 26);
            this.Dragbar.TabIndex = 53;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.CloseBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(327, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(27, 26);
            this.panel1.TabIndex = 0;
            // 
            // CloseBox
            // 
            this.CloseBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CloseBox.Image = ((System.Drawing.Image)(resources.GetObject("CloseBox.Image")));
            this.CloseBox.Location = new System.Drawing.Point(5, 4);
            this.CloseBox.Name = "CloseBox";
            this.CloseBox.Size = new System.Drawing.Size(17, 17);
            this.CloseBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.CloseBox.TabIndex = 18;
            this.CloseBox.TabStop = false;
            this.CloseBox.Click += new System.EventHandler(this.CloseBox_Click);
            // 
            // SwapForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 319);
            this.Controls.Add(this.Dragbar);
            this.Controls.Add(this.SwappedActive);
            this.Controls.Add(this.ArrowIconImage);
            this.Controls.Add(this.ReplaceI);
            this.Controls.Add(this.SearchI);
            this.Controls.Add(this.CosmeticNameLabel);
            this.Controls.Add(this.RevertButton);
            this.Controls.Add(this.ConvertButton);
            this.Controls.Add(this.Logs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SwapForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SwapForm";
            this.Load += new System.EventHandler(this.SwapForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ArrowIconImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ReplaceI)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SearchI)).EndInit();
            this.Dragbar.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.CloseBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label Logs;
        private System.Windows.Forms.Label RevertButton;
        private System.Windows.Forms.Label ConvertButton;
        private System.Windows.Forms.Label CosmeticNameLabel;
        private System.Windows.Forms.Label SwappedActive;
        private System.Windows.Forms.PictureBox ArrowIconImage;
        private System.Windows.Forms.PictureBox ReplaceI;
        private System.Windows.Forms.PictureBox SearchI;
        private System.Windows.Forms.Panel Dragbar;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox CloseBox;
    }
}