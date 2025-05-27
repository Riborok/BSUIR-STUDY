namespace Lab1
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
            this.pb = new System.Windows.Forms.PictureBox();
            this.tbObservations = new System.Windows.Forms.TextBox();
            this.tbCentroids = new System.Windows.Forms.TextBox();
            this.lbObservations = new System.Windows.Forms.Label();
            this.lbCentroids = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.pnl = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pb)).BeginInit();
            this.pnl.SuspendLayout();
            this.SuspendLayout();
            // 
            // pb
            // 
            this.pb.BackColor = System.Drawing.SystemColors.Desktop;
            this.pb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pb.Location = new System.Drawing.Point(0, 0);
            this.pb.Name = "pb";
            this.pb.Size = new System.Drawing.Size(800, 450);
            this.pb.TabIndex = 0;
            this.pb.TabStop = false;
            this.pb.Paint += new System.Windows.Forms.PaintEventHandler(this.pb_Paint);
            // 
            // tbObservations
            // 
            this.tbObservations.Location = new System.Drawing.Point(12, 35);
            this.tbObservations.Name = "tbObservations";
            this.tbObservations.Size = new System.Drawing.Size(209, 20);
            this.tbObservations.TabIndex = 1;
            // 
            // tbCentroids
            // 
            this.tbCentroids.Location = new System.Drawing.Point(237, 35);
            this.tbCentroids.Name = "tbCentroids";
            this.tbCentroids.Size = new System.Drawing.Size(208, 20);
            this.tbCentroids.TabIndex = 2;
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
            // lbCentroids
            // 
            this.lbCentroids.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbCentroids.Location = new System.Drawing.Point(237, 9);
            this.lbCentroids.Name = "lbCentroids";
            this.lbCentroids.Size = new System.Drawing.Size(199, 23);
            this.lbCentroids.TabIndex = 4;
            this.lbCentroids.Text = "Amount of centroids";
            // 
            // btnApply
            // 
            this.btnApply.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnApply.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnApply.Location = new System.Drawing.Point(468, 9);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(153, 43);
            this.btnApply.TabIndex = 5;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnStart
            // 
            this.btnStart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnStart.Location = new System.Drawing.Point(627, 9);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(153, 43);
            this.btnStart.TabIndex = 6;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // pnl
            // 
            this.pnl.Controls.Add(this.lbObservations);
            this.pnl.Controls.Add(this.btnStart);
            this.pnl.Controls.Add(this.tbObservations);
            this.pnl.Controls.Add(this.btnApply);
            this.pnl.Controls.Add(this.lbCentroids);
            this.pnl.Controls.Add(this.tbCentroids);
            this.pnl.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnl.Location = new System.Drawing.Point(0, 0);
            this.pnl.Name = "pnl";
            this.pnl.Size = new System.Drawing.Size(800, 68);
            this.pnl.TabIndex = 7;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pnl);
            this.Controls.Add(this.pb);
            this.Name = "Main";
            this.Text = "Main";
            ((System.ComponentModel.ISupportInitialize)(this.pb)).EndInit();
            this.pnl.ResumeLayout(false);
            this.pnl.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel pnl;

        private System.Windows.Forms.Label lbCentroids;
        private System.Windows.Forms.Label lbObservations;

        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnStart;

        private System.Windows.Forms.TextBox tbObservations;
        private System.Windows.Forms.TextBox tbCentroids;

        private System.Windows.Forms.PictureBox pb;
        
        #endregion
    }
}