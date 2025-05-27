namespace Client {
	partial class FormMain {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			_httpClient.Dispose();
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tbStatusCode = new System.Windows.Forms.TextBox();
			this.lbStatusCode = new System.Windows.Forms.Label();
			this.cbMethod = new System.Windows.Forms.ComboBox();
			this.lbMethod = new System.Windows.Forms.Label();
			this.tbPath = new System.Windows.Forms.TextBox();
			this.lbPath = new System.Windows.Forms.Label();
			this.tbContent = new System.Windows.Forms.TextBox();
			this.btnSend = new System.Windows.Forms.Button();
			this.lbContent = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// tbStatusCode
			// 
			this.tbStatusCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tbStatusCode.Location = new System.Drawing.Point(12, 222);
			this.tbStatusCode.Name = "tbStatusCode";
			this.tbStatusCode.ReadOnly = true;
			this.tbStatusCode.Size = new System.Drawing.Size(117, 29);
			this.tbStatusCode.TabIndex = 0;
			// 
			// lbStatusCode
			// 
			this.lbStatusCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lbStatusCode.Location = new System.Drawing.Point(12, 196);
			this.lbStatusCode.Name = "lbStatusCode";
			this.lbStatusCode.Size = new System.Drawing.Size(117, 23);
			this.lbStatusCode.TabIndex = 1;
			this.lbStatusCode.Text = "Status Code";
			// 
			// cbMethod
			// 
			this.cbMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbMethod.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.cbMethod.FormattingEnabled = true;
			this.cbMethod.Location = new System.Drawing.Point(12, 35);
			this.cbMethod.Name = "cbMethod";
			this.cbMethod.Size = new System.Drawing.Size(96, 32);
			this.cbMethod.TabIndex = 2;
			this.cbMethod.SelectedIndexChanged += new System.EventHandler(this.cbMethod_SelectedIndexChanged);
			// 
			// lbMethod
			// 
			this.lbMethod.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lbMethod.Location = new System.Drawing.Point(12, 9);
			this.lbMethod.Name = "lbMethod";
			this.lbMethod.Size = new System.Drawing.Size(96, 23);
			this.lbMethod.TabIndex = 3;
			this.lbMethod.Text = "Method";
			// 
			// tbPath
			// 
			this.tbPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tbPath.Location = new System.Drawing.Point(127, 38);
			this.tbPath.Name = "tbPath";
			this.tbPath.Size = new System.Drawing.Size(289, 29);
			this.tbPath.TabIndex = 4;
			// 
			// lbPath
			// 
			this.lbPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lbPath.Location = new System.Drawing.Point(127, 12);
			this.lbPath.Name = "lbPath";
			this.lbPath.Size = new System.Drawing.Size(289, 23);
			this.lbPath.TabIndex = 5;
			this.lbPath.Text = "Path";
			// 
			// tbContent
			// 
			this.tbContent.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tbContent.Location = new System.Drawing.Point(12, 100);
			this.tbContent.Multiline = true;
			this.tbContent.Name = "tbContent";
			this.tbContent.Size = new System.Drawing.Size(404, 93);
			this.tbContent.TabIndex = 6;
			// 
			// btnSend
			// 
			this.btnSend.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.btnSend.Location = new System.Drawing.Point(301, 211);
			this.btnSend.Name = "btnSend";
			this.btnSend.Size = new System.Drawing.Size(115, 40);
			this.btnSend.TabIndex = 7;
			this.btnSend.Text = "Send";
			this.btnSend.UseVisualStyleBackColor = true;
			this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
			// 
			// lbContent
			// 
			this.lbContent.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lbContent.Location = new System.Drawing.Point(12, 74);
			this.lbContent.Name = "lbContent";
			this.lbContent.Size = new System.Drawing.Size(404, 23);
			this.lbContent.TabIndex = 8;
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(428, 263);
			this.Controls.Add(this.lbContent);
			this.Controls.Add(this.btnSend);
			this.Controls.Add(this.tbContent);
			this.Controls.Add(this.lbPath);
			this.Controls.Add(this.tbPath);
			this.Controls.Add(this.lbMethod);
			this.Controls.Add(this.cbMethod);
			this.Controls.Add(this.lbStatusCode);
			this.Controls.Add(this.tbStatusCode);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormMain";
			this.Text = "HTTP Client";
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		private System.Windows.Forms.Label lbContent;

		private System.Windows.Forms.Button btnSend;

		private System.Windows.Forms.TextBox tbContent;

		private System.Windows.Forms.Label lbPath;

		private System.Windows.Forms.TextBox tbPath;

		private System.Windows.Forms.Label lbMethod;

		private System.Windows.Forms.ComboBox cbMethod;

		private System.Windows.Forms.Label lbStatusCode;

		private System.Windows.Forms.TextBox tbStatusCode;

		#endregion
	}
}