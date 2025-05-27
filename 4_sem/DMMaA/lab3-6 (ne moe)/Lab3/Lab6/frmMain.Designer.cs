namespace LabN6
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
            this.btnSolve = new System.Windows.Forms.Button();
            this.lblObjectCount = new System.Windows.Forms.Label();
            this.cbMaximized = new System.Windows.Forms.CheckBox();
            this.dgvDistances = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDistances)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSolve
            // 
            this.btnSolve.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnSolve.Location = new System.Drawing.Point(12, 156);
            this.btnSolve.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSolve.Name = "btnSolve";
            this.btnSolve.Size = new System.Drawing.Size(337, 78);
            this.btnSolve.TabIndex = 0;
            this.btnSolve.Text = "Solve";
            this.btnSolve.UseVisualStyleBackColor = true;
            this.btnSolve.Click += new System.EventHandler(this.btnSolve_Click);
            // 
            // lblObjectCount
            // 
            this.lblObjectCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblObjectCount.Location = new System.Drawing.Point(12, 9);
            this.lblObjectCount.Name = "lblObjectCount";
            this.lblObjectCount.Size = new System.Drawing.Size(337, 39);
            this.lblObjectCount.TabIndex = 2;
            this.lblObjectCount.Text = "Enter object count";
            // 
            // cbMaximized
            // 
            this.cbMaximized.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cbMaximized.Location = new System.Drawing.Point(12, 113);
            this.cbMaximized.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbMaximized.Name = "cbMaximized";
            this.cbMaximized.Size = new System.Drawing.Size(161, 39);
            this.cbMaximized.TabIndex = 3;
            this.cbMaximized.Text = "Maximized";
            this.cbMaximized.UseVisualStyleBackColor = true;
            // 
            // dgvDistances
            // 
            this.dgvDistances.AllowUserToAddRows = false;
            this.dgvDistances.AllowUserToDeleteRows = false;
            this.dgvDistances.AllowUserToResizeColumns = false;
            this.dgvDistances.AllowUserToResizeRows = false;
            this.dgvDistances.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDistances.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dgvDistances.Location = new System.Drawing.Point(0, 252);
            this.dgvDistances.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgvDistances.Name = "dgvDistances";
            this.dgvDistances.ReadOnly = true;
            this.dgvDistances.RowTemplate.Height = 24;
            this.dgvDistances.Size = new System.Drawing.Size(365, 515);
            this.dgvDistances.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.dgvDistances);
            this.panel1.Controls.Add(this.lblObjectCount);
            this.panel1.Controls.Add(this.cbMaximized);
            this.panel1.Controls.Add(this.btnSolve);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(365, 767);
            this.panel1.TabIndex = 5;
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox1.Location = new System.Drawing.Point(12, 51);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(337, 34);
            this.textBox1.TabIndex = 4;
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(365, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(850, 767);
            this.panel2.TabIndex = 6;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1215, 767);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "frmMain";
            this.Text = "Hierarchy";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDistances)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panel2;

        private System.Windows.Forms.TextBox textBox1;

        private System.Windows.Forms.Panel panel1;

        private System.Windows.Forms.DataGridView dgvDistances;


        private System.Windows.Forms.Button btnSolve;
        private System.Windows.Forms.Label lblObjectCount;
        private System.Windows.Forms.CheckBox cbMaximized;

        #endregion
    }
}