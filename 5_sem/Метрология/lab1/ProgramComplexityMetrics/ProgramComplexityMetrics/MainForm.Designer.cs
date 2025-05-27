namespace ProgramComplexityMetrics
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tbProgram = new System.Windows.Forms.TextBox();
            this.butRun = new System.Windows.Forms.Button();
            this.lbProgram = new System.Windows.Forms.Label();
            this.lbResult = new System.Windows.Forms.Label();
            this.tbResult = new System.Windows.Forms.WebBrowser();
            this.butOpenProgram = new System.Windows.Forms.Button();
            this.butSaveProgram = new System.Windows.Forms.Button();
            this.butSaveResult = new System.Windows.Forms.Button();
            this.butOpenResult = new System.Windows.Forms.Button();
            this.tbProgramFileName = new System.Windows.Forms.TextBox();
            this.tbResultFileName = new System.Windows.Forms.TextBox();
            this.lbErrors = new System.Windows.Forms.Label();
            this.tbErrors = new System.Windows.Forms.TextBox();
            this.butSaveAsResult = new System.Windows.Forms.Button();
            this.butSaveAsProgram = new System.Windows.Forms.Button();
            this.butNewResult = new System.Windows.Forms.Button();
            this.butNewProgram = new System.Windows.Forms.Button();
            this.butResetResult = new System.Windows.Forms.Button();
            this.butResetProgram = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbProgram
            // 
            this.tbProgram.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbProgram.Location = new System.Drawing.Point(12, 45);
            this.tbProgram.Multiline = true;
            this.tbProgram.Name = "tbProgram";
            this.tbProgram.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbProgram.Size = new System.Drawing.Size(422, 522);
            this.tbProgram.TabIndex = 1;
            this.tbProgram.TextChanged += new System.EventHandler(this.tbProgram_TextChanged);
            // 
            // butRun
            // 
            this.butRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butRun.Location = new System.Drawing.Point(476, 597);
            this.butRun.Name = "butRun";
            this.butRun.Size = new System.Drawing.Size(422, 66);
            this.butRun.TabIndex = 7;
            this.butRun.Text = "Run";
            this.butRun.UseVisualStyleBackColor = true;
            this.butRun.Click += new System.EventHandler(this.butRun_Click);
            // 
            // lbProgram
            // 
            this.lbProgram.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbProgram.Location = new System.Drawing.Point(12, 9);
            this.lbProgram.Name = "lbProgram";
            this.lbProgram.Size = new System.Drawing.Size(300, 33);
            this.lbProgram.TabIndex = 11;
            this.lbProgram.Text = "Program:";
            this.lbProgram.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbResult
            // 
            this.lbResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbResult.Location = new System.Drawing.Point(476, 9);
            this.lbResult.Name = "lbResult";
            this.lbResult.Size = new System.Drawing.Size(300, 33);
            this.lbResult.TabIndex = 13;
            this.lbResult.Text = "Result:";
            this.lbResult.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbResult
            // 
            this.tbResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbResult.Location = new System.Drawing.Point(476, 45);
            this.tbResult.Name = "tbResult";
            this.tbResult.Size = new System.Drawing.Size(422, 522);
            this.tbResult.TabIndex = 16;
            // 
            // butOpenProgram
            // 
            this.butOpenProgram.Image = ((System.Drawing.Image)(resources.GetObject("butOpenProgram.Image")));
            this.butOpenProgram.Location = new System.Drawing.Point(316, 17);
            this.butOpenProgram.Name = "butOpenProgram";
            this.butOpenProgram.Size = new System.Drawing.Size(25, 25);
            this.butOpenProgram.TabIndex = 17;
            this.butOpenProgram.UseVisualStyleBackColor = true;
            this.butOpenProgram.Click += new System.EventHandler(this.butOpenProgram_Click);
            // 
            // butSaveProgram
            // 
            this.butSaveProgram.Image = ((System.Drawing.Image)(resources.GetObject("butSaveProgram.Image")));
            this.butSaveProgram.Location = new System.Drawing.Point(347, 17);
            this.butSaveProgram.Name = "butSaveProgram";
            this.butSaveProgram.Size = new System.Drawing.Size(25, 25);
            this.butSaveProgram.TabIndex = 18;
            this.butSaveProgram.UseVisualStyleBackColor = true;
            this.butSaveProgram.Click += new System.EventHandler(this.butSaveProgram_Click);
            // 
            // butSaveResult
            // 
            this.butSaveResult.Image = ((System.Drawing.Image)(resources.GetObject("butSaveResult.Image")));
            this.butSaveResult.Location = new System.Drawing.Point(811, 17);
            this.butSaveResult.Name = "butSaveResult";
            this.butSaveResult.Size = new System.Drawing.Size(25, 25);
            this.butSaveResult.TabIndex = 20;
            this.butSaveResult.UseVisualStyleBackColor = true;
            this.butSaveResult.Click += new System.EventHandler(this.butSaveResult_Click);
            // 
            // butOpenResult
            // 
            this.butOpenResult.Image = ((System.Drawing.Image)(resources.GetObject("butOpenResult.Image")));
            this.butOpenResult.Location = new System.Drawing.Point(780, 17);
            this.butOpenResult.Name = "butOpenResult";
            this.butOpenResult.Size = new System.Drawing.Size(25, 25);
            this.butOpenResult.TabIndex = 19;
            this.butOpenResult.UseVisualStyleBackColor = true;
            this.butOpenResult.Click += new System.EventHandler(this.butOpenResult_Click);
            // 
            // tbProgramFileName
            // 
            this.tbProgramFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbProgramFileName.Location = new System.Drawing.Point(80, 14);
            this.tbProgramFileName.Name = "tbProgramFileName";
            this.tbProgramFileName.ReadOnly = true;
            this.tbProgramFileName.Size = new System.Drawing.Size(201, 22);
            this.tbProgramFileName.TabIndex = 24;
            // 
            // tbResultFileName
            // 
            this.tbResultFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbResultFileName.Location = new System.Drawing.Point(544, 14);
            this.tbResultFileName.Name = "tbResultFileName";
            this.tbResultFileName.ReadOnly = true;
            this.tbResultFileName.Size = new System.Drawing.Size(199, 22);
            this.tbResultFileName.TabIndex = 25;
            // 
            // lbErrors
            // 
            this.lbErrors.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbErrors.Location = new System.Drawing.Point(12, 577);
            this.lbErrors.Name = "lbErrors";
            this.lbErrors.Size = new System.Drawing.Size(45, 17);
            this.lbErrors.TabIndex = 8;
            this.lbErrors.Text = "Errors:";
            this.lbErrors.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbErrors
            // 
            this.tbErrors.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbErrors.Location = new System.Drawing.Point(12, 597);
            this.tbErrors.Multiline = true;
            this.tbErrors.Name = "tbErrors";
            this.tbErrors.ReadOnly = true;
            this.tbErrors.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbErrors.Size = new System.Drawing.Size(422, 66);
            this.tbErrors.TabIndex = 9;
            // 
            // butSaveAsResult
            // 
            this.butSaveAsResult.Image = ((System.Drawing.Image)(resources.GetObject("butSaveAsResult.Image")));
            this.butSaveAsResult.Location = new System.Drawing.Point(842, 17);
            this.butSaveAsResult.Name = "butSaveAsResult";
            this.butSaveAsResult.Size = new System.Drawing.Size(25, 25);
            this.butSaveAsResult.TabIndex = 26;
            this.butSaveAsResult.UseVisualStyleBackColor = true;
            this.butSaveAsResult.Click += new System.EventHandler(this.butSaveAsResult_Click);
            // 
            // butSaveAsProgram
            // 
            this.butSaveAsProgram.Image = ((System.Drawing.Image)(resources.GetObject("butSaveAsProgram.Image")));
            this.butSaveAsProgram.Location = new System.Drawing.Point(378, 17);
            this.butSaveAsProgram.Name = "butSaveAsProgram";
            this.butSaveAsProgram.Size = new System.Drawing.Size(25, 25);
            this.butSaveAsProgram.TabIndex = 27;
            this.butSaveAsProgram.UseVisualStyleBackColor = true;
            this.butSaveAsProgram.Click += new System.EventHandler(this.butSaveAsProgram_Click);
            // 
            // butNewResult
            // 
            this.butNewResult.Image = ((System.Drawing.Image)(resources.GetObject("butNewResult.Image")));
            this.butNewResult.Location = new System.Drawing.Point(749, 17);
            this.butNewResult.Name = "butNewResult";
            this.butNewResult.Size = new System.Drawing.Size(25, 25);
            this.butNewResult.TabIndex = 28;
            this.butNewResult.UseVisualStyleBackColor = true;
            this.butNewResult.Click += new System.EventHandler(this.butNewResult_Click);
            // 
            // butNewProgram
            // 
            this.butNewProgram.Image = ((System.Drawing.Image)(resources.GetObject("butNewProgram.Image")));
            this.butNewProgram.Location = new System.Drawing.Point(287, 17);
            this.butNewProgram.Name = "butNewProgram";
            this.butNewProgram.Size = new System.Drawing.Size(25, 25);
            this.butNewProgram.TabIndex = 29;
            this.butNewProgram.UseVisualStyleBackColor = true;
            this.butNewProgram.Click += new System.EventHandler(this.butNewProgram_Click);
            // 
            // butResetResult
            // 
            this.butResetResult.Image = ((System.Drawing.Image)(resources.GetObject("butResetResult.Image")));
            this.butResetResult.Location = new System.Drawing.Point(873, 17);
            this.butResetResult.Name = "butResetResult";
            this.butResetResult.Size = new System.Drawing.Size(25, 25);
            this.butResetResult.TabIndex = 34;
            this.butResetResult.UseVisualStyleBackColor = true;
            this.butResetResult.Click += new System.EventHandler(this.butResetResult_Click);
            // 
            // butResetProgram
            // 
            this.butResetProgram.Image = ((System.Drawing.Image)(resources.GetObject("butResetProgram.Image")));
            this.butResetProgram.Location = new System.Drawing.Point(409, 17);
            this.butResetProgram.Name = "butResetProgram";
            this.butResetProgram.Size = new System.Drawing.Size(25, 25);
            this.butResetProgram.TabIndex = 35;
            this.butResetProgram.UseVisualStyleBackColor = true;
            this.butResetProgram.Click += new System.EventHandler(this.butResetProgram_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(915, 675);
            this.Controls.Add(this.butResetProgram);
            this.Controls.Add(this.butResetResult);
            this.Controls.Add(this.butNewProgram);
            this.Controls.Add(this.butNewResult);
            this.Controls.Add(this.butSaveAsProgram);
            this.Controls.Add(this.butSaveAsResult);
            this.Controls.Add(this.tbResultFileName);
            this.Controls.Add(this.tbProgramFileName);
            this.Controls.Add(this.butSaveResult);
            this.Controls.Add(this.butOpenResult);
            this.Controls.Add(this.butSaveProgram);
            this.Controls.Add(this.butOpenProgram);
            this.Controls.Add(this.tbResult);
            this.Controls.Add(this.lbResult);
            this.Controls.Add(this.lbProgram);
            this.Controls.Add(this.tbErrors);
            this.Controls.Add(this.lbErrors);
            this.Controls.Add(this.butRun);
            this.Controls.Add(this.tbProgram);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(15, 15);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Button butResetResult;
        private System.Windows.Forms.Button butResetProgram;

        private System.Windows.Forms.Button butNewResult;
        private System.Windows.Forms.Button butNewProgram;

        private System.Windows.Forms.Button butSaveAsProgram;

        private System.Windows.Forms.Button butSaveAsResult;

        private System.Windows.Forms.TextBox tbResultFileName;

        private System.Windows.Forms.TextBox tbProgramFileName;

        private System.Windows.Forms.Button butSaveResult;
        private System.Windows.Forms.Button butOpenResult;

        private System.Windows.Forms.Button butSaveProgram;

        private System.Windows.Forms.Button butOpenProgram;

        private System.Windows.Forms.Label lbProgram;
        private System.Windows.Forms.Label lbResult;

        private System.Windows.Forms.Label lbErrors;
        private System.Windows.Forms.TextBox tbErrors;

        private System.Windows.Forms.Button butRun;

        private System.Windows.Forms.TextBox tbProgram;

        private System.Windows.Forms.WebBrowser tbResult;

        #endregion
    }
}
