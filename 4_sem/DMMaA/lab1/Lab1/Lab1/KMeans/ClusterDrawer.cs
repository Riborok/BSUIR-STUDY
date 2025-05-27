using System.Drawing;
using Lab1.Geometry;

namespace Lab1.KMeans
{
    public class ClusterDrawer
    {
        private const int ObservationRadius = 1;
        private const int CentroidRadius = 7;
        
        private readonly PointDrawer _observationsDrawer;
        private readonly PointDrawer _centroidsDrawer;

        public ClusterDrawer(KMeansSolver kMeansSolver)
        {
            _observationsDrawer = new PointDrawer(kMeansSolver.Observations, ObservationRadius);
            _centroidsDrawer = new PointDrawer(kMeansSolver.Centroids, CentroidRadius);
        }

        public void DrawClusters(Graphics g)
        {
            _observationsDrawer.DrawPoints(g);
            _centroidsDrawer.DrawPoints(g);
        }
    }
}