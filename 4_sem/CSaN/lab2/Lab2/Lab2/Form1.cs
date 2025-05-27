using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab2
{
	public partial class Form1 : Form
	{
		private readonly Client _client;
		public Form1()
		{
			InitializeComponent();
			StartPosition = FormStartPosition.CenterScreen;
			_client = new Client(tbPublicChat);

			InputBox inputBox = new InputBox();
			_client.Connect(ReadIpAddress(inputBox), ReadPort(inputBox));
		}
		
		private static IPAddress ReadIpAddress(InputBox inputBox)
		{
			while (true)
			{
				string ipAddressStr = inputBox.GetStr("Enter IP address:", "IP Address", "127.0.0.1");
				if (IPAddress.TryParse(ipAddressStr, out IPAddress ipAddress))
					return ipAddress;
				
				MessageBox.Show(@"Invalid IP address", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private static int ReadPort(InputBox inputBox)
		{
			while (true)
			{
				string portStr = inputBox.GetStr("Enter port:", "Port", "8081");
				if (int.TryParse(portStr, out int port) && port >= 0 && port <= ushort.MaxValue)
					return port;
				
				MessageBox.Show(@"Invalid port", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnSend_Click(object sender, EventArgs e)
		{
			_client.Send(tbWriteMsg.Text);
			tbWriteMsg.Text = "";
		}
	}
}