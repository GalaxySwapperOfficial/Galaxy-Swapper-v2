
namespace Galaxy_Swapper_v2.Workspace.Forms
{
    partial class ID_Swapper
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ID_Swapper));
            this.Dragbar = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.CloseBox = new System.Windows.Forms.PictureBox();
            this.ConvertButton = new System.Windows.Forms.Label();
            this.RevertButton = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.CosmeticNameLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.Dragbar.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CloseBox)).BeginInit();
            this.SuspendLayout();
            // 
            // Dragbar
            // 
            this.Dragbar.Controls.Add(this.panel1);
            this.Dragbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.Dragbar.Location = new System.Drawing.Point(0, 0);
            this.Dragbar.Name = "Dragbar";
            this.Dragbar.Size = new System.Drawing.Size(520, 26);
            this.Dragbar.TabIndex = 54;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.CloseBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(493, 0);
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
            // ConvertButton
            // 
            this.ConvertButton.BackColor = System.Drawing.Color.White;
            this.ConvertButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ConvertButton.ForeColor = System.Drawing.Color.White;
            this.ConvertButton.Location = new System.Drawing.Point(12, 144);
            this.ConvertButton.Name = "ConvertButton";
            this.ConvertButton.Size = new System.Drawing.Size(234, 39);
            this.ConvertButton.TabIndex = 55;
            this.ConvertButton.Text = "Convert";
            this.ConvertButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ConvertButton.Click += new System.EventHandler(this.Control_Click);
            // 
            // RevertButton
            // 
            this.RevertButton.BackColor = System.Drawing.Color.White;
            this.RevertButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.RevertButton.ForeColor = System.Drawing.Color.White;
            this.RevertButton.Location = new System.Drawing.Point(261, 144);
            this.RevertButton.Name = "RevertButton";
            this.RevertButton.Size = new System.Drawing.Size(234, 39);
            this.RevertButton.TabIndex = 56;
            this.RevertButton.Text = "Revert";
            this.RevertButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.RevertButton.Click += new System.EventHandler(this.Control_Click);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(12, 193);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(234, 39);
            this.label2.TabIndex = 57;
            this.label2.Text = "Tutorial";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.White;
            this.label3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(261, 193);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(234, 39);
            this.label3.TabIndex = 58;
            this.label3.Text = "Cosmetic Id List";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox2
            // 
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBox2.Location = new System.Drawing.Point(12, 102);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(483, 25);
            this.textBox2.TabIndex = 60;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.Window;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBox1.Location = new System.Drawing.Point(12, 54);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(483, 25);
            this.textBox1.TabIndex = 59;
            // 
            // CosmeticNameLabel
            // 
            this.CosmeticNameLabel.BackColor = System.Drawing.Color.Transparent;
            this.CosmeticNameLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.CosmeticNameLabel.ForeColor = System.Drawing.Color.White;
            this.CosmeticNameLabel.Location = new System.Drawing.Point(12, 33);
            this.CosmeticNameLabel.Name = "CosmeticNameLabel";
            this.CosmeticNameLabel.Size = new System.Drawing.Size(483, 18);
            this.CosmeticNameLabel.TabIndex = 61;
            this.CosmeticNameLabel.Text = "ID That You Own:";
            this.CosmeticNameLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(12, 81);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(483, 18);
            this.label4.TabIndex = 62;
            this.label4.Text = "ID That You Want:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(497, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 25);
            this.label5.TabIndex = 63;
            this.label5.Text = "0";
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(497, 102);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 25);
            this.label6.TabIndex = 64;
            this.label6.Text = "0";
            // 
            // ID_Swapper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 244);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.CosmeticNameLabel);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.RevertButton);
            this.Controls.Add(this.ConvertButton);
            this.Controls.Add(this.Dragbar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ID_Swapper";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ID_Swapper";
            this.Load += new System.EventHandler(this.ID_Swapper_Load);
            this.Dragbar.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.CloseBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel Dragbar;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox CloseBox;
        private System.Windows.Forms.Label ConvertButton;
        private System.Windows.Forms.Label RevertButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label CosmeticNameLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}