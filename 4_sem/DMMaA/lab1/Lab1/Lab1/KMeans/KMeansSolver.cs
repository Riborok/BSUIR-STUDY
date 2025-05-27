using System;
using System.Collections.Generic;
using System.Drawing;
using Lab1.Geometry;
using Point = Lab1.Geometry.Point;

namespace Lab1.KMeans
{
    public class KMeansSolver
    {
        private static readonly Color DefaultObservationColor = Color.White;

        public Point[] Observations { get; }
        public Point[] Centroids { get; }
        
        private readonly int _width;
        private readonly int _height;

        public KMeansSolver(int observationsCount, int centroidsCount, int width, int height)
        {
            Observations = new Point[observationsCount];
            Centroids = new Point[centroidsCount];
            _width = width;
            _height = height;

            FillCentroids();
            FillObservations();
        }
        
        private void FillObservations()
        {
            Random random = new Random();
            for (int i = 0; i < Observations.Length; i++)
            {
                Observations[i] = CreateRandomPoint(random, DefaultObservationColor);
                Observations[i].Color = FindNearestClusterColor(Observations[i]);
            }
        }

        private void FillCentroids()
        {
            Random random = new Random();
            var colorGenerator = new UniqueColorGenerator();
            
            for (int i = 0; i < Centroids.Length; i++)
                Centroids[i] = CreateRandomPoint(random, colorGenerator.GenerateUniqueColor());
        }
        
        private Point CreateRandomPoint(Random random, Color color)
        {
            int x = random.Next(_width);
            int y = random.Next(_height);
            return new Point(x, y, color);
        }
        
        public void RunKMeansAlgorithm()
        {
            bool isClustersChange;
            do
            {
                RecreateCentroids();
                isClustersChange = AssignObservationsToNearestCluster();
            } while (isClustersChange);
        }
        
        private bool AssignObservationsToNearestCluster()
        {
            bool isClustersChange = false;
            for (var i = 0; i < Observations.Length; i++)  {
                var nearestCentroidColor = FindNearestClusterColor(Observations[i]);
                if (Observations[i].Color != nearestCentroidColor)
                {
                    isClustersChange = true;
                    Observations[i].Color = nearestCentroidColor;
                }
            }

            return isClustersChange;
        }
        
        private Color FindNearestClusterColor(Point observation)
        {
            double minDistance = double.MaxValue;
            Color nearestCentroidColor = DefaultObservationColor;

            foreach (var cluster in Centroids)
            {
                double distance = MathUtils.CalcEuclideanDistance(observation, cluster);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestCentroidColor = cluster.Color;
                }
            }

            return nearestCentroidColor;
        }
        
        private void RecreateCentroids()
        {
            var clusters = CreateClusters();
            for (int i = 0; i < Centroids.Length; i++)
                RecreateCentroid(clusters[Centroids[i].Color], ref Centroids[i]);
        }
        
        private IReadOnlyDictionary<Color, List<Point>> CreateClusters()
        {
            var clusters = new Dictionary<Color, List<Point>>();
            foreach (var observation in Observations)
            {
                if (!clusters.ContainsKey(observation.Color))
                    clusters[observation.Color] = new List<Point>();
                clusters[observation.Color].Add(observation);
            }
            return clusters;
        }

        private static void RecreateCentroid(List<Point> cluster, ref Point centroid)
        {
            if (cluster.Count == 0) 
                return;
                
            int totalX = 0;
            int totalY = 0;

            foreach (Point observation in cluster)
            {
                totalX += observation.X;
                totalY += observation.Y;
            }
                
            centroid.X = totalX / cluster.Count;
            centroid.Y = totalY / cluster.Count;
        }
    }
}