using System;
using System.Windows.Forms;

namespace Lab2
{
	public sealed partial class InputBox : Form
	{
		public InputBox()
		{
			InitializeComponent();
		}

		public string GetStr(string formTitle, string labelText, string defaultValue)
		{
			Text = formTitle;
			label1.Text = labelText;
			textBox1.Focus();
			textBox1.Text = defaultValue;
			textBox1.Select(0, defaultValue.Length);
			StartPosition = FormStartPosition.CenterScreen;
			
			DialogResult result = ShowDialog();

			return result == DialogResult.OK ? textBox1.Text : defaultValue;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}