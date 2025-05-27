using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab4
{
    public partial class frmMain : Form
    {
        private Perceptron _perceptron;
        public frmMain()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            textBox1.Text = @"4";
            textBox1.Focus();
        }

        private void btnSolve_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out int count)) {
                _perceptron = new Perceptron(count);
                _perceptron.Solve();
                DisplayClasses(_perceptron._classes, tbClasses);
                DisplayFunctions(_perceptron._functions, tbRes);
            }
        }

        private static void DisplayFunctions(List<Perceptron.Vector> vectors, TextBox listBox) {
            StringBuilder sb = new StringBuilder();
            foreach (var vector in vectors) {
                string valuesString = "(" + string.Join(", ", vector.Values) + ")";
                sb.Append(valuesString);
                sb.AppendLine();
            }
            listBox.Text = sb.ToString();
        }

        private static void DisplayClasses(List<Perceptron.TestClass> vectors, TextBox listBox) {
            StringBuilder sb = new StringBuilder();
            int counter = 1;
            foreach (var clas in vectors) {
                string temp = $"Class № {counter++}";
                sb.Append(temp);
                foreach (var vector in clas.Values) {
                    string valuesString = "(" + string.Join(", ", vector.Values) + ")";
                    sb.Append(valuesString);
                }
                sb.AppendLine();
            }
            listBox.Text = sb.ToString();
        }
    }
}