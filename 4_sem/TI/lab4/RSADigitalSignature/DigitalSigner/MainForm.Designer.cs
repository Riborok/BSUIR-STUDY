namespace DigitalSigner
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
            this.tbText = new System.Windows.Forms.TextBox();
            this.lbP = new System.Windows.Forms.Label();
            this.butSign = new System.Windows.Forms.Button();
            this.lbText = new System.Windows.Forms.Label();
            this.tbP = new System.Windows.Forms.TextBox();
            this.butOpenText = new System.Windows.Forms.Button();
            this.butSaveText = new System.Windows.Forms.Button();
            this.butCheckSign = new System.Windows.Forms.Button();
            this.tbTextFileName = new System.Windows.Forms.TextBox();
            this.lbErrors = new System.Windows.Forms.Label();
            this.tbErrors = new System.Windows.Forms.TextBox();
            this.butSaveAsText = new System.Windows.Forms.Button();
            this.butNewText = new System.Windows.Forms.Button();
            this.butResetText = new System.Windows.Forms.Button();
            this.tbQ = new System.Windows.Forms.TextBox();
            this.lbQ = new System.Windows.Forms.Label();
            this.tbE = new System.Windows.Forms.TextBox();
            this.lbE = new System.Windows.Forms.Label();
            this.tbD = new System.Windows.Forms.TextBox();
            this.lbD = new System.Windows.Forms.Label();
            this.tbPhi = new System.Windows.Forms.TextBox();
            this.lbPhi = new System.Windows.Forms.Label();
            this.tbR = new System.Windows.Forms.TextBox();
            this.lbR = new System.Windows.Forms.Label();
            this.cbHashingAlgorithm = new System.Windows.Forms.ComboBox();
            this.lbHashingAlgorithm = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tbText
            // 
            this.tbText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbText.Location = new System.Drawing.Point(16, 55);
            this.tbText.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbText.Multiline = true;
            this.tbText.Name = "tbText";
            this.tbText.ReadOnly = true;
            this.tbText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbText.Size = new System.Drawing.Size(429, 345);
            this.tbText.TabIndex = 1;
            // 
            // lbP
            // 
            this.lbP.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbP.Location = new System.Drawing.Point(465, 23);
            this.lbP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbP.Name = "lbP";
            this.lbP.Size = new System.Drawing.Size(255, 28);
            this.lbP.TabIndex = 6;
            this.lbP.Text = "p";
            this.lbP.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // butSign
            // 
            this.butSign.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butSign.Location = new System.Drawing.Point(16, 549);
            this.butSign.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.butSign.Name = "butSign";
            this.butSign.Size = new System.Drawing.Size(428, 36);
            this.butSign.TabIndex = 7;
            this.butSign.Text = "Sign";
            this.butSign.UseVisualStyleBackColor = true;
            this.butSign.Click += new System.EventHandler(this.butSign_Click);
            // 
            // lbText
            // 
            this.lbText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbText.Location = new System.Drawing.Point(16, 11);
            this.lbText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbText.Name = "lbText";
            this.lbText.Size = new System.Drawing.Size(400, 41);
            this.lbText.TabIndex = 11;
            this.lbText.Text = "Text:";
            this.lbText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbP
            // 
            this.tbP.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbP.Location = new System.Drawing.Point(465, 55);
            this.tbP.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbP.Name = "tbP";
            this.tbP.Size = new System.Drawing.Size(429, 26);
            this.tbP.TabIndex = 15;
            // 
            // butOpenText
            // 
            this.butOpenText.Image = ((System.Drawing.Image)(resources.GetObject("butOpenText.Image")));
            this.butOpenText.Location = new System.Drawing.Point(291, 21);
            this.butOpenText.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.butOpenText.Name = "butOpenText";
            this.butOpenText.Size = new System.Drawing.Size(33, 31);
            this.butOpenText.TabIndex = 17;
            this.butOpenText.UseVisualStyleBackColor = true;
            this.butOpenText.Click += new System.EventHandler(this.butOpenInitText_Click);
            // 
            // butSaveText
            // 
            this.butSaveText.Image = ((System.Drawing.Image)(resources.GetObject("butSaveText.Image")));
            this.butSaveText.Location = new System.Drawing.Point(329, 21);
            this.butSaveText.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.butSaveText.Name = "butSaveText";
            this.butSaveText.Size = new System.Drawing.Size(33, 31);
            this.butSaveText.TabIndex = 18;
            this.butSaveText.UseVisualStyleBackColor = true;
            this.butSaveText.Click += new System.EventHandler(this.butSaveInitText_Click);
            // 
            // butCheckSign
            // 
            this.butCheckSign.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butCheckSign.Location = new System.Drawing.Point(465, 549);
            this.butCheckSign.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.butCheckSign.Name = "butCheckSign";
            this.butCheckSign.Size = new System.Drawing.Size(428, 36);
            this.butCheckSign.TabIndex = 21;
            this.butCheckSign.Text = "Check Sign";
            this.butCheckSign.UseVisualStyleBackColor = true;
            this.butCheckSign.Click += new System.EventHandler(this.butCheckSign_Click);
            // 
            // tbTextFileName
            // 
            this.tbTextFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbTextFileName.Location = new System.Drawing.Point(71, 17);
            this.tbTextFileName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbTextFileName.Name = "tbTextFileName";
            this.tbTextFileName.ReadOnly = true;
            this.tbTextFileName.Size = new System.Drawing.Size(173, 26);
            this.tbTextFileName.TabIndex = 24;
            // 
            // lbErrors
            // 
            this.lbErrors.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbErrors.Location = new System.Drawing.Point(465, 395);
            this.lbErrors.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbErrors.Name = "lbErrors";
            this.lbErrors.Size = new System.Drawing.Size(60, 21);
            this.lbErrors.TabIndex = 8;
            this.lbErrors.Text = "Errors:";
            this.lbErrors.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbErrors
            // 
            this.tbErrors.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbErrors.Location = new System.Drawing.Point(465, 420);
            this.tbErrors.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbErrors.Multiline = true;
            this.tbErrors.Name = "tbErrors";
            this.tbErrors.ReadOnly = true;
            this.tbErrors.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbErrors.Size = new System.Drawing.Size(429, 110);
            this.tbErrors.TabIndex = 9;
            // 
            // butSaveAsText
            // 
            this.butSaveAsText.Image = ((System.Drawing.Image)(resources.GetObject("butSaveAsText.Image")));
            this.butSaveAsText.Location = new System.Drawing.Point(371, 21);
            this.butSaveAsText.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.butSaveAsText.Name = "butSaveAsText";
            this.butSaveAsText.Size = new System.Drawing.Size(33, 31);
            this.butSaveAsText.TabIndex = 27;
            this.butSaveAsText.UseVisualStyleBackColor = true;
            this.butSaveAsText.Click += new System.EventHandler(this.butSaveAsInitText_Click);
            // 
            // butNewText
            // 
            this.butNewText.Image = ((System.Drawing.Image)(resources.GetObject("butNewText.Image")));
            this.butNewText.Location = new System.Drawing.Point(251, 21);
            this.butNewText.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.butNewText.Name = "butNewText";
            this.butNewText.Size = new System.Drawing.Size(33, 31);
            this.butNewText.TabIndex = 29;
            this.butNewText.UseVisualStyleBackColor = true;
            this.butNewText.Click += new System.EventHandler(this.butNewInitText_Click);
            // 
            // butResetText
            // 
            this.butResetText.Image = ((System.Drawing.Image)(resources.GetObject("butResetText.Image")));
            this.butResetText.Location = new System.Drawing.Point(412, 21);
            this.butResetText.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.butResetText.Name = "butResetText";
            this.butResetText.Size = new System.Drawing.Size(33, 31);
            this.butResetText.TabIndex = 35;
            this.butResetText.UseVisualStyleBackColor = true;
            this.butResetText.Click += new System.EventHandler(this.butResetInitText_Click);
            // 
            // tbQ
            // 
            this.tbQ.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbQ.Location = new System.Drawing.Point(465, 125);
            this.tbQ.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbQ.Name = "tbQ";
            this.tbQ.Size = new System.Drawing.Size(429, 26);
            this.tbQ.TabIndex = 78;
            // 
            // lbQ
            // 
            this.lbQ.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbQ.Location = new System.Drawing.Point(465, 93);
            this.lbQ.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbQ.Name = "lbQ";
            this.lbQ.Size = new System.Drawing.Size(255, 28);
            this.lbQ.TabIndex = 77;
            this.lbQ.Text = "q";
            this.lbQ.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbE
            // 
            this.tbE.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbE.Location = new System.Drawing.Point(465, 195);
            this.tbE.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbE.Name = "tbE";
            this.tbE.Size = new System.Drawing.Size(429, 26);
            this.tbE.TabIndex = 82;
            // 
            // lbE
            // 
            this.lbE.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbE.Location = new System.Drawing.Point(465, 163);
            this.lbE.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbE.Name = "lbE";
            this.lbE.Size = new System.Drawing.Size(431, 28);
            this.lbE.TabIndex = 81;
            this.lbE.Text = "e";
            this.lbE.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbD
            // 
            this.tbD.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbD.Location = new System.Drawing.Point(16, 514);
            this.tbD.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbD.Name = "tbD";
            this.tbD.ReadOnly = true;
            this.tbD.Size = new System.Drawing.Size(429, 26);
            this.tbD.TabIndex = 88;
            // 
            // lbD
            // 
            this.lbD.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbD.Location = new System.Drawing.Point(16, 482);
            this.lbD.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbD.Name = "lbD";
            this.lbD.Size = new System.Drawing.Size(431, 28);
            this.lbD.TabIndex = 87;
            this.lbD.Text = "d";
            this.lbD.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbPhi
            // 
            this.tbPhi.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbPhi.Location = new System.Drawing.Point(16, 445);
            this.tbPhi.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbPhi.Name = "tbPhi";
            this.tbPhi.ReadOnly = true;
            this.tbPhi.Size = new System.Drawing.Size(429, 26);
            this.tbPhi.TabIndex = 86;
            // 
            // lbPhi
            // 
            this.lbPhi.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbPhi.Location = new System.Drawing.Point(16, 413);
            this.lbPhi.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbPhi.Name = "lbPhi";
            this.lbPhi.Size = new System.Drawing.Size(255, 28);
            this.lbPhi.TabIndex = 85;
            this.lbPhi.Text = "phi";
            this.lbPhi.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbR
            // 
            this.tbR.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbR.Location = new System.Drawing.Point(465, 266);
            this.tbR.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbR.Name = "tbR";
            this.tbR.Size = new System.Drawing.Size(429, 26);
            this.tbR.TabIndex = 84;
            // 
            // lbR
            // 
            this.lbR.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbR.Location = new System.Drawing.Point(465, 234);
            this.lbR.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbR.Name = "lbR";
            this.lbR.Size = new System.Drawing.Size(255, 28);
            this.lbR.TabIndex = 83;
            this.lbR.Text = "r";
            this.lbR.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbHashingAlgorithm
            // 
            this.cbHashingAlgorithm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbHashingAlgorithm.FormattingEnabled = true;
            this.cbHashingAlgorithm.Items.AddRange(new object[] { "Basic", "SHA256", "SHA512", "MD5" });
            this.cbHashingAlgorithm.Location = new System.Drawing.Point(465, 339);
            this.cbHashingAlgorithm.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbHashingAlgorithm.Name = "cbHashingAlgorithm";
            this.cbHashingAlgorithm.Size = new System.Drawing.Size(429, 24);
            this.cbHashingAlgorithm.TabIndex = 89;
            // 
            // lbHashingAlgorithm
            // 
            this.lbHashingAlgorithm.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbHashingAlgorithm.Location = new System.Drawing.Point(465, 309);
            this.lbHashingAlgorithm.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbHashingAlgorithm.Name = "lbHashingAlgorithm";
            this.lbHashingAlgorithm.Size = new System.Drawing.Size(431, 28);
            this.lbHashingAlgorithm.TabIndex = 90;
            this.lbHashingAlgorithm.Text = "Hashing Algorithm";
            this.lbHashingAlgorithm.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(909, 596);
            this.Controls.Add(this.lbHashingAlgorithm);
            this.Controls.Add(this.cbHashingAlgorithm);
            this.Controls.Add(this.tbD);
            this.Controls.Add(this.lbD);
            this.Controls.Add(this.tbPhi);
            this.Controls.Add(this.lbPhi);
            this.Controls.Add(this.tbR);
            this.Controls.Add(this.lbR);
            this.Controls.Add(this.tbE);
            this.Controls.Add(this.lbE);
            this.Controls.Add(this.tbQ);
            this.Controls.Add(this.lbQ);
            this.Controls.Add(this.butResetText);
            this.Controls.Add(this.butNewText);
            this.Controls.Add(this.butSaveAsText);
            this.Controls.Add(this.tbTextFileName);
            this.Controls.Add(this.butCheckSign);
            this.Controls.Add(this.butSaveText);
            this.Controls.Add(this.butOpenText);
            this.Controls.Add(this.tbP);
            this.Controls.Add(this.lbText);
            this.Controls.Add(this.tbErrors);
            this.Controls.Add(this.lbErrors);
            this.Controls.Add(this.butSign);
            this.Controls.Add(this.lbP);
            this.Controls.Add(this.tbText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(15, 15);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.ComboBox cbHashingAlgorithm;
        private System.Windows.Forms.Label lbHashingAlgorithm;

        private System.Windows.Forms.TextBox tbD;
        private System.Windows.Forms.Label lbD;
        private System.Windows.Forms.TextBox tbPhi;
        private System.Windows.Forms.Label lbPhi;
        private System.Windows.Forms.TextBox tbR;
        private System.Windows.Forms.Label lbR;

        private System.Windows.Forms.TextBox tbE;
        private System.Windows.Forms.Label lbE;

        private System.Windows.Forms.TextBox tbQ;
        private System.Windows.Forms.Label lbQ;

        private System.Windows.Forms.Button butResetText;

        private System.Windows.Forms.Button butNewText;

        private System.Windows.Forms.Button butSaveAsText;

        private System.Windows.Forms.TextBox tbTextFileName;

        private System.Windows.Forms.Button butCheckSign;

        private System.Windows.Forms.Button butSaveText;

        private System.Windows.Forms.Button butOpenText;

        private System.Windows.Forms.TextBox tbP;

        private System.Windows.Forms.Label lbText;

        private System.Windows.Forms.Label lbErrors;
        private System.Windows.Forms.TextBox tbErrors;

        private System.Windows.Forms.Button butSign;

        private System.Windows.Forms.Label lbP;

        private System.Windows.Forms.TextBox tbText;

        #endregion
    }
}
