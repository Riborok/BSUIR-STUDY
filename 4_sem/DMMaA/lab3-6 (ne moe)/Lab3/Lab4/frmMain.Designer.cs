namespace Lab4
{
    partial class frmMain
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnSolve = new System.Windows.Forms.Button();
            this.lblClasses = new System.Windows.Forms.Label();
            this.lblResFuncs = new System.Windows.Forms.Label();
            this.tbClasses = new System.Windows.Forms.TextBox();
            this.tbRes = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(11, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(244, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "Enter the amount of classes";
            // 
            // btnSolve
            // 
            this.btnSolve.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnSolve.Location = new System.Drawing.Point(509, 11);
            this.btnSolve.Margin = new System.Windows.Forms.Padding(2);
            this.btnSolve.Name = "btnSolve";
            this.btnSolve.Size = new System.Drawing.Size(148, 41);
            this.btnSolve.TabIndex = 2;
            this.btnSolve.Text = "Find functions";
            this.btnSolve.UseVisualStyleBackColor = true;
            this.btnSolve.Click += new System.EventHandler(this.btnSolve_Click);
            // 
            // lblClasses
            // 
            this.lblClasses.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblClasses.Location = new System.Drawing.Point(12, 96);
            this.lblClasses.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblClasses.Name = "lblClasses";
            this.lblClasses.Size = new System.Drawing.Size(174, 25);
            this.lblClasses.TabIndex = 5;
            this.lblClasses.Text = "Classes";
            // 
            // lblResFuncs
            // 
            this.lblResFuncs.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblResFuncs.Location = new System.Drawing.Point(344, 96);
            this.lblResFuncs.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblResFuncs.Name = "lblResFuncs";
            this.lblResFuncs.Size = new System.Drawing.Size(198, 25);
            this.lblResFuncs.TabIndex = 6;
            this.lblResFuncs.Text = "Resulting functions";
            // 
            // tbClasses
            // 
            this.tbClasses.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbClasses.Location = new System.Drawing.Point(12, 124);
            this.tbClasses.Multiline = true;
            this.tbClasses.Name = "tbClasses";
            this.tbClasses.ReadOnly = true;
            this.tbClasses.Size = new System.Drawing.Size(311, 207);
            this.tbClasses.TabIndex = 7;
            // 
            // tbRes
            // 
            this.tbRes.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbRes.Location = new System.Drawing.Point(344, 124);
            this.tbRes.Multiline = true;
            this.tbRes.Name = "tbRes";
            this.tbRes.ReadOnly = true;
            this.tbRes.Size = new System.Drawing.Size(311, 207);
            this.tbRes.TabIndex = 8;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnSolve);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(668, 64);
            this.panel1.TabIndex = 9;
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox1.Location = new System.Drawing.Point(246, 17);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(127, 29);
            this.textBox1.TabIndex = 1;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(668, 343);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tbRes);
            this.Controls.Add(this.tbClasses);
            this.Controls.Add(this.lblResFuncs);
            this.Controls.Add(this.lblClasses);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.Text = "Perceptron";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TextBox textBox1;

        private System.Windows.Forms.Panel panel1;

        private System.Windows.Forms.TextBox tbRes;

        private System.Windows.Forms.TextBox tbClasses;

        private System.Windows.Forms.Label lblClasses;
        private System.Windows.Forms.Label lblResFuncs;

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSolve;

        #endregion
    }
}