using System.Windows;
using RabinCryptosystemResearchHelper;

// ReSharper disable InconsistentNaming

namespace RabinCryptosystemResearch
{
    public partial class RangeInputDialog
    {
        public int pStart { get; private set; }
        public int pEnd { get; private set; }
        public int qStart { get; private set; }
        public int qEnd { get; private set; }
        public int bStart { get; private set; }
        public int bEnd { get; private set; }

        public RangeInputDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(PStartTextBox.Text, out int pStart) && int.TryParse(PEndTextBox.Text, out int pEnd) &&
                int.TryParse(QStartTextBox.Text, out int qStart) && int.TryParse(QEndTextBox.Text, out int qEnd) &&
                int.TryParse(BStartTextBox.Text, out int bStart) && int.TryParse(BEndTextBox.Text, out int bEnd))
            {
                this.pStart = pStart;
                this.pEnd = pEnd;
                this.qStart = qStart;
                this.qEnd = qEnd;
                this.bStart = bStart;
                this.bEnd = bEnd;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Invalid input. Please enter integers for p, q, and b.");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}