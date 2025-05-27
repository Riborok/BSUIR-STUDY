namespace LabN3
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
            this.numProb1 = new System.Windows.Forms.NumericUpDown();
            this.numProb2 = new System.Windows.Forms.NumericUpDown();
            this.lblProb1 = new System.Windows.Forms.Label();
            this.lvlProb2 = new System.Windows.Forms.Label();
            this.pnlPlot = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSolve = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.numLT = new System.Windows.Forms.Label();
            this.numSP = new System.Windows.Forms.Label();
            this.numSum = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numProb1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numProb2)).BeginInit();
            this.pnlPlot.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // numProb1
            // 
            this.numProb1.DecimalPlaces = 2;
            this.numProb1.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.numProb1.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            this.numProb1.Location = new System.Drawing.Point(71, 11);
            this.numProb1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.numProb1.Maximum = new decimal(new int[] { 99, 0, 0, 131072 });
            this.numProb1.Minimum = new decimal(new int[] { 1, 0, 0, 131072 });
            this.numProb1.Name = "numProb1";
            this.numProb1.Size = new System.Drawing.Size(120, 34);
            this.numProb1.TabIndex = 0;
            this.numProb1.Value = new decimal(new int[] { 5, 0, 0, 65536 });
            this.numProb1.ValueChanged += new System.EventHandler(this.numProb1_ValueChanged);
            // 
            // numProb2
            // 
            this.numProb2.DecimalPlaces = 2;
            this.numProb2.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.numProb2.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            this.numProb2.Location = new System.Drawing.Point(281, 11);
            this.numProb2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.numProb2.Maximum = new decimal(new int[] { 99, 0, 0, 131072 });
            this.numProb2.Minimum = new decimal(new int[] { 1, 0, 0, 131072 });
            this.numProb2.Name = "numProb2";
            this.numProb2.Size = new System.Drawing.Size(120, 34);
            this.numProb2.TabIndex = 1;
            this.numProb2.Value = new decimal(new int[] { 5, 0, 0, 65536 });
            this.numProb2.ValueChanged += new System.EventHandler(this.numProb2_ValueChanged);
            // 
            // lblProb1
            // 
            this.lblProb1.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblProb1.Location = new System.Drawing.Point(3, 14);
            this.lblProb1.Name = "lblProb1";
            this.lblProb1.Size = new System.Drawing.Size(62, 31);
            this.lblProb1.TabIndex = 2;
            this.lblProb1.Text = "P(C1)";
            // 
            // lvlProb2
            // 
            this.lvlProb2.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lvlProb2.Location = new System.Drawing.Point(209, 14);
            this.lvlProb2.Name = "lvlProb2";
            this.lvlProb2.Size = new System.Drawing.Size(66, 31);
            this.lvlProb2.TabIndex = 3;
            this.lvlProb2.Text = "P(C2)";
            // 
            // pnlPlot
            // 
            this.pnlPlot.Controls.Add(this.panel2);
            this.pnlPlot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPlot.Location = new System.Drawing.Point(0, 0);
            this.pnlPlot.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlPlot.Name = "pnlPlot";
            this.pnlPlot.Size = new System.Drawing.Size(1015, 518);
            this.pnlPlot.TabIndex = 4;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.numSum);
            this.panel2.Controls.Add(this.numSP);
            this.panel2.Controls.Add(this.numLT);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 414);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1015, 104);
            this.panel2.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(3, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 57);
            this.label2.TabIndex = 10;
            this.label2.Text = "False alarm probability";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(670, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(212, 57);
            this.label3.TabIndex = 11;
            this.label3.Text = "Probability of total classification error";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(281, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(229, 66);
            this.label1.TabIndex = 11;
            this.label1.Text = "Probability of missing detection";
            // 
            // btnSolve
            // 
            this.btnSolve.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnSolve.Location = new System.Drawing.Point(431, 11);
            this.btnSolve.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSolve.Name = "btnSolve";
            this.btnSolve.Size = new System.Drawing.Size(183, 34);
            this.btnSolve.TabIndex = 5;
            this.btnSolve.Text = "Solve";
            this.btnSolve.UseVisualStyleBackColor = true;
            this.btnSolve.Click += new System.EventHandler(this.btnSolve_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblProb1);
            this.panel1.Controls.Add(this.numProb1);
            this.panel1.Controls.Add(this.lvlProb2);
            this.panel1.Controls.Add(this.btnSolve);
            this.panel1.Controls.Add(this.numProb2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1015, 55);
            this.panel1.TabIndex = 12;
            // 
            // numLT
            // 
            this.numLT.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.numLT.Location = new System.Drawing.Point(127, 9);
            this.numLT.Name = "numLT";
            this.numLT.Size = new System.Drawing.Size(148, 57);
            this.numLT.TabIndex = 12;
            // 
            // numSP
            // 
            this.numSP.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.numSP.Location = new System.Drawing.Point(516, 9);
            this.numSP.Name = "numSP";
            this.numSP.Size = new System.Drawing.Size(148, 57);
            this.numSP.TabIndex = 13;
            // 
            // numSum
            // 
            this.numSum.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.numSum.Location = new System.Drawing.Point(860, 9);
            this.numSum.Name = "numSum";
            this.numSum.Size = new System.Drawing.Size(148, 57);
            this.numSum.TabIndex = 14;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1015, 518);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pnlPlot);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.numProb1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numProb2)).EndInit();
            this.pnlPlot.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label numSum;

        private System.Windows.Forms.Label numSP;

        private System.Windows.Forms.Label numLT;

        private System.Windows.Forms.Panel panel2;

        private System.Windows.Forms.Panel panel1;

        private System.Windows.Forms.Label label3;

        private System.Windows.Forms.Label label2;

        private System.Windows.Forms.Label label1;

        private System.Windows.Forms.Button btnSolve;

        private System.Windows.Forms.Panel pnlPlot;

        private System.Windows.Forms.Label lblProb1;
        private System.Windows.Forms.Label lvlProb2;

        private System.Windows.Forms.NumericUpDown numProb1;
        private System.Windows.Forms.NumericUpDown numProb2;

        #endregion
    }
}