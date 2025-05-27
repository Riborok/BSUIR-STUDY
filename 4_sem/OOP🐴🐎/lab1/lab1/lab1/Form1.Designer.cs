namespace lab1
{
    partial class Form1
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
        private void InitializeComponent() {
            this.pl = new System.Windows.Forms.Panel();
            this.btn = new System.Windows.Forms.Button();
            this.pb = new System.Windows.Forms.PictureBox();
            this.pl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb)).BeginInit();
            this.SuspendLayout();
            // 
            // pl
            // 
            this.pl.Controls.Add(this.btn);
            this.pl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pl.Location = new System.Drawing.Point(0, 379);
            this.pl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pl.Name = "pl";
            this.pl.Size = new System.Drawing.Size(800, 71);
            this.pl.TabIndex = 0;
            // 
            // btn
            // 
            this.btn.Location = new System.Drawing.Point(16, 11);
            this.btn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btn.Name = "btn";
            this.btn.Size = new System.Drawing.Size(160, 46);
            this.btn.TabIndex = 0;
            this.btn.Text = "Generate";
            this.btn.UseVisualStyleBackColor = true;
            this.btn.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // pb
            // 
            this.pb.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pb.Location = new System.Drawing.Point(0, 0);
            this.pb.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pb.Name = "pb";
            this.pb.Size = new System.Drawing.Size(800, 379);
            this.pb.TabIndex = 1;
            this.pb.TabStop = false;
            this.pb.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint_1);
            // 
            // Figure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pb);
            this.Controls.Add(this.pl);
            this.Location = new System.Drawing.Point(15, 15);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Figure";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.pl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pb)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.PictureBox pb;

        private System.Windows.Forms.Button btn;

        private System.Windows.Forms.Panel pl;

        #endregion
    }
}