using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client {
	public partial class FormMain : Form
	{
		private readonly HttpClient _httpClient;
		
		public FormMain() 
		{
			InitializeComponent();
			StartPosition = FormStartPosition.CenterScreen;
			cbMethod.Items.AddRange(new object[] { "GET", "PUT", "DELETE", "POST", "COPY", "MOVE" });
			cbMethod.SelectedIndex = 0;
			_httpClient = new HttpClient("http://localhost:8081/");
		}

		private async void btnSend_Click(object sender, EventArgs e)
		{
			Task<HttpResponseMessage> responseMessage;
			switch (cbMethod.SelectedItem)
			{
				case "GET":
					responseMessage = _httpClient.GetAsync(tbPath.Text);
					var result = await responseMessage;
					if (result.IsSuccessStatusCode)
						tbContent.Text = await result.Content.ReadAsStringAsync();
					break;
				case "PUT":
					responseMessage = _httpClient.PutAsync(tbPath.Text, tbContent.Text);
					break;
				case "POST":
					responseMessage = _httpClient.PostAsync(tbPath.Text, tbContent.Text);
					break;
				case "DELETE":
					responseMessage = _httpClient.DeleteAsync(tbPath.Text);
					break;
				case "COPY":
					responseMessage = _httpClient.CopyAsync(tbPath.Text, tbContent.Text);
					break;
				case "MOVE":
					responseMessage = _httpClient.MoveAsync(tbPath.Text, tbContent.Text);
					break;
				default:
					throw new ArgumentException();
			}
			await responseMessage;
			tbStatusCode.Text = responseMessage.Result.StatusCode.ToString();
		}

		private void cbMethod_SelectedIndexChanged(object sender, EventArgs e)
		{
			ClearText();
			lbContent.Visible = tbContent.Visible = true;
			tbContent.ReadOnly = false;
			switch (cbMethod.SelectedItem)
			{
				case "GET":
					tbContent.ReadOnly = true;
					goto case "PUT";
				case "PUT":
				case "POST":
					lbContent.Text = @"Text";
					tbContent.Multiline = true;
					break;
				case "COPY":
				case "MOVE":
					lbContent.Text = @"Destination Path";
					tbContent.Multiline = false;
					break;
				case "DELETE":
					tbContent.Visible = lbContent.Visible = false;
					break;
			}
		}

		private void ClearText() => tbStatusCode.Text = tbContent.Text = string.Empty;
	}
}