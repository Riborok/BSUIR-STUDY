using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lab1.KMeans;

namespace Lab1
{
    public partial class Main : Form
    {
        private const int DefaultObservationsCount = 10_000;
        private const int DefaultCentroidsCount = 10;
        
        private KMeansSolver _kmeansSolver;
        private ClusterDrawer _clusterDrawer;

        public Main()
        {
            InitializeComponent();
            UpdateTextBoxesText(DefaultObservationsCount, DefaultCentroidsCount);
            btnStart.Enabled = false;
            btnApply.Enabled = true;
        }

        private void pb_Paint(object sender, PaintEventArgs e)
        {
            _clusterDrawer?.DrawClusters(e.Graphics);
        }
        
        private void btnStart_Click(object sender, EventArgs e)
        {
            _kmeansSolver.RunKMeansAlgorithm();
            SwapBtnEnabled();
            pb.Invalidate();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (!TryParseTextBoxes(out int observationsCount, out int centroidsCount))
                return;

            ApplyBounds(ref observationsCount, ref centroidsCount);
            UpdateTextBoxesText(observationsCount, centroidsCount);

            _kmeansSolver = new KMeansSolver(observationsCount, centroidsCount, pb.Width, pb.Height);
            _clusterDrawer = new ClusterDrawer(_kmeansSolver);
            
            pb.Invalidate();
            SwapBtnEnabled();
        }

        private bool TryParseTextBoxes(out int observationsCount, out int centroidsCount)
        {
            return int.TryParse(tbObservations.Text, out observationsCount) & 
                   int.TryParse(tbCentroids.Text, out centroidsCount);
        }

        private static void ApplyBounds(ref int observationsCount, ref int centroidsCount)
        {
            observationsCount = BoundsChecker.CheckObservationsCount(observationsCount);
            centroidsCount = BoundsChecker.CheckCentroidsCount(centroidsCount);
        }

        private void UpdateTextBoxesText(int observationsCount, int centroidsCount)
        {
            tbObservations.Text = observationsCount.ToString();
            tbCentroids.Text = centroidsCount.ToString();
        }
        
        private void SwapBtnEnabled()
        {
            btnApply.Enabled = !btnApply.Enabled;
            btnStart.Enabled = !btnStart.Enabled;
        }
    }
}