using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lab2.KMeans_Maximin;

namespace Lab2
{
    public partial class Main : Form
    {
        private enum BtnEnableMask : byte
        {
            BtnStart   = 0b100,
            BtnMaximin = 0b010,
            BtnKMeans  = 0b001
        }
        
        private const int DefaultObservationsCount = 10_000;
        
        private MaximinSolver _maximinSolver;
        private ClusterDrawer _clusterDrawer;

        public Main()
        {
            InitializeComponent();
            UpdateTextBoxesText(DefaultObservationsCount);
            SetBtnEnable(BtnEnableMask.BtnStart);
        }
        
        private void pb_Paint(object sender, PaintEventArgs e)
        {
            _clusterDrawer?.DrawClusters(e.Graphics);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!TryParseTextBoxes(out int observationsCount))
                return;

            ApplyBounds(ref observationsCount);
            UpdateTextBoxesText(observationsCount);

            _maximinSolver = new MaximinSolver(observationsCount, new Size(pb.Width, pb.Height));
            _clusterDrawer = new ClusterDrawer(_maximinSolver.Observations, _maximinSolver.Centroids);
            
            SetBtnEnable(BtnEnableMask.BtnMaximin);
            pb.Invalidate();
        }
        
        private void btnMaximin_Click(object sender, EventArgs e)
        {
            if (!_maximinSolver.RunMaximinAlgorithm())
                SetBtnEnable(BtnEnableMask.BtnKMeans);
            pb.Invalidate();
        }
        
        private void btnKMeans_Click(object sender, EventArgs e)
        {
            var kMeansSolver = new KMeansSolver(_maximinSolver.Observations, _maximinSolver.Centroids);
            kMeansSolver.RunKMeansAlgorithm();
            SetBtnEnable(BtnEnableMask.BtnStart);
            pb.Invalidate();
        }

        private bool TryParseTextBoxes(out int observationsCount)
        {
            return int.TryParse(tbObservations.Text, out observationsCount);
        }

        private static void ApplyBounds(ref int observationsCount)
        {
            observationsCount = BoundsChecker.CheckObservationsCount(observationsCount);
        }

        private void UpdateTextBoxesText(int observationsCount)
        {
            tbObservations.Text = observationsCount.ToString();
        }

        private void SetBtnEnable(BtnEnableMask mask)
        {
            btnStart.Enabled   = ((byte)mask & 0b100) !=0;
            btnMaximin.Enabled = ((byte)mask & 0b010) !=0;
            btnKMeans.Enabled  = ((byte)mask & 0b001) !=0;
        }
    }
}