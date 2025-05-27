using System.Collections.Generic;
using System.Drawing;
using Lab2.Geometry;
using Point = Lab2.Geometry.Point;

namespace Lab2.KMeans_Maximin
{
    public class ClusterDrawer
    {
        private const int ObservationRadius = 1;
        private const int CentroidRadius = 7;
        
        private readonly PointDrawer _observationsDrawer;
        private readonly PointDrawer _centroidsDrawer;

        public ClusterDrawer(IReadOnlyCollection<Point> observations, IReadOnlyCollection<Point> centroids)
        {
            _observationsDrawer = new PointDrawer(observations, ObservationRadius);
            _centroidsDrawer = new PointDrawer(centroids, CentroidRadius);
        }

        public void DrawClusters(Graphics g)
        {
            _observationsDrawer.DrawPoints(g);
            _centroidsDrawer.DrawPoints(g);
        }
    }
}