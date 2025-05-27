namespace Lab2
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
			_client.Dispose();
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tbPublicChat = new System.Windows.Forms.TextBox();
			this.tbWriteMsg = new System.Windows.Forms.TextBox();
			this.lbPublicChat = new System.Windows.Forms.Label();
			this.lbWriteMsg = new System.Windows.Forms.Label();
			this.btnSend = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// tbPublicChat
			// 
			this.tbPublicChat.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tbPublicChat.Location = new System.Drawing.Point(16, 55);
			this.tbPublicChat.Margin = new System.Windows.Forms.Padding(4);
			this.tbPublicChat.Multiline = true;
			this.tbPublicChat.Name = "tbPublicChat";
			this.tbPublicChat.ReadOnly = true;
			this.tbPublicChat.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbPublicChat.Size = new System.Drawing.Size(579, 483);
			this.tbPublicChat.TabIndex = 1;
			// 
			// tbWriteMsg
			// 
			this.tbWriteMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tbWriteMsg.Location = new System.Drawing.Point(689, 55);
			this.tbWriteMsg.Margin = new System.Windows.Forms.Padding(4);
			this.tbWriteMsg.Multiline = true;
			this.tbWriteMsg.Name = "tbWriteMsg";
			this.tbWriteMsg.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbWriteMsg.Size = new System.Drawing.Size(348, 483);
			this.tbWriteMsg.TabIndex = 0;
			// 
			// lbPublicChat
			// 
			this.lbPublicChat.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lbPublicChat.Location = new System.Drawing.Point(16, 11);
			this.lbPublicChat.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lbPublicChat.Name = "lbPublicChat";
			this.lbPublicChat.Size = new System.Drawing.Size(580, 41);
			this.lbPublicChat.TabIndex = 2;
			this.lbPublicChat.Text = "Public chat";
			// 
			// lbWriteMsg
			// 
			this.lbWriteMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lbWriteMsg.Location = new System.Drawing.Point(689, 11);
			this.lbWriteMsg.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lbWriteMsg.Name = "lbWriteMsg";
			this.lbWriteMsg.Size = new System.Drawing.Size(209, 41);
			this.lbWriteMsg.TabIndex = 3;
			this.lbWriteMsg.Text = "Write message";
			// 
			// btnSend
			// 
			this.btnSend.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.btnSend.Location = new System.Drawing.Point(907, 11);
			this.btnSend.Margin = new System.Windows.Forms.Padding(4);
			this.btnSend.Name = "btnSend";
			this.btnSend.Size = new System.Drawing.Size(132, 37);
			this.btnSend.TabIndex = 4;
			this.btnSend.Text = "Send";
			this.btnSend.UseVisualStyleBackColor = true;
			this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1067, 554);
			this.Controls.Add(this.btnSend);
			this.Controls.Add(this.lbWriteMsg);
			this.Controls.Add(this.lbPublicChat);
			this.Controls.Add(this.tbWriteMsg);
			this.Controls.Add(this.tbPublicChat);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "Form1";
			this.Text = "Client";
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		private System.Windows.Forms.Button btnSend;

		private System.Windows.Forms.Label lbWriteMsg;

		private System.Windows.Forms.Label lbPublicChat;

		private System.Windows.Forms.TextBox tbWriteMsg;

		private System.Windows.Forms.TextBox tbPublicChat;

		#endregion
	}
}