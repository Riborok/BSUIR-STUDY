namespace Lab2
{
    partial class Main
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
            this.tbObservations = new System.Windows.Forms.TextBox();
            this.lbObservations = new System.Windows.Forms.Label();
            this.pnl = new System.Windows.Forms.Panel();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnKMeans = new System.Windows.Forms.Button();
            this.btnMaximin = new System.Windows.Forms.Button();
            this.pb = new System.Windows.Forms.PictureBox();
            this.pnl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb)).BeginInit();
            this.SuspendLayout();
            // 
            // tbObservations
            // 
            this.tbObservations.Location = new System.Drawing.Point(12, 35);
            this.tbObservations.Name = "tbObservations";
            this.tbObservations.Size = new System.Drawing.Size(209, 20);
            this.tbObservations.TabIndex = 1;
            // 
            // lbObservations
            // 
            this.lbObservations.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbObservations.Location = new System.Drawing.Point(12, 9);
            this.lbObservations.Name = "lbObservations";
            this.lbObservations.Size = new System.Drawing.Size(208, 23);
            this.lbObservations.TabIndex = 3;
            this.lbObservations.Text = "Amount of observations";
            // 
            // pnl
            // 
            this.pnl.Controls.Add(this.btnStart);
            this.pnl.Controls.Add(this.btnKMeans);
            this.pnl.Controls.Add(this.btnMaximin);
            this.pnl.Controls.Add(this.lbObservations);
            this.pnl.Controls.Add(this.tbObservations);
            this.pnl.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnl.Location = new System.Drawing.Point(0, 0);
            this.pnl.Name = "pnl";
            this.pnl.Size = new System.Drawing.Size(800, 68);
            this.pnl.TabIndex = 7;
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnStart.Location = new System.Drawing.Point(236, 12);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(142, 43);
            this.btnStart.TabIndex = 6;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnKMeans
            // 
            this.btnKMeans.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnKMeans.Location = new System.Drawing.Point(569, 12);
            this.btnKMeans.Name = "btnKMeans";
            this.btnKMeans.Size = new System.Drawing.Size(142, 43);
            this.btnKMeans.TabIndex = 5;
            this.btnKMeans.Text = "KMeans";
            this.btnKMeans.UseVisualStyleBackColor = true;
            this.btnKMeans.Click += new System.EventHandler(this.btnKMeans_Click);
            // 
            // btnMaximin
            // 
            this.btnMaximin.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnMaximin.Location = new System.Drawing.Point(399, 12);
            this.btnMaximin.Name = "btnMaximin";
            this.btnMaximin.Size = new System.Drawing.Size(142, 43);
            this.btnMaximin.TabIndex = 4;
            this.btnMaximin.Text = "Maximin";
            this.btnMaximin.UseVisualStyleBackColor = true;
            this.btnMaximin.Click += new System.EventHandler(this.btnMaximin_Click);
            // 
            // pb
            // 
            this.pb.BackColor = System.Drawing.SystemColors.Desktop;
            this.pb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pb.Location = new System.Drawing.Point(0, 68);
            this.pb.Name = "pb";
            this.pb.Size = new System.Drawing.Size(800, 382);
            this.pb.TabIndex = 8;
            this.pb.TabStop = false;
            this.pb.Paint += new System.Windows.Forms.PaintEventHandler(this.pb_Paint);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pb);
            this.Controls.Add(this.pnl);
            this.Name = "Main";
            this.Text = "Main";
            this.pnl.ResumeLayout(false);
            this.pnl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.PictureBox pb;

        private System.Windows.Forms.Button btnStart;

        private System.Windows.Forms.Button btnMaximin;
        private System.Windows.Forms.Button btnKMeans;

        private System.Windows.Forms.Panel pnl;

        private System.Windows.Forms.Label lbObservations;

        private System.Windows.Forms.TextBox tbObservations;

        #endregion
    }
}