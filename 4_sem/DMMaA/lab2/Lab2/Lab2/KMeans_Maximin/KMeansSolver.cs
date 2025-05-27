using System;
using System.Collections.Generic;
using System.Drawing;
using Lab2.Geometry;
using Point = Lab2.Geometry.Point;

namespace Lab2.KMeans_Maximin
{
    public class KMeansSolver
    {
        private static readonly Color DefaultObservationColor = Color.White;

        public Point[] Observations { get; }
        public List<Point> Centroids { get; }

        public KMeansSolver(Point[] observations, List<Point> centroids)
        {
            Observations = observations;
            Centroids = centroids;
        }
        
        public KMeansSolver(int observationsCount, int centroidsCount, Size size)
        {
            Observations = new Point[observationsCount];
            Centroids = new List<Point>(centroidsCount);

            FillCentroids(size);
            FillObservations(size);
        }
        
        private void FillObservations(Size size)
        {
            Random random = new Random();
            for (int i = 0; i < Observations.Length; i++)
            {
                Observations[i] = PointCreator.CreateRandomPoint(random, DefaultObservationColor, size);
                Observations[i].Color = FindNearestClusterColor(Observations[i]);
            }
        }

        private void FillCentroids(Size size)
        {
            Random random = new Random();
            
            for (int i = 0; i < Centroids.Capacity; i++)
                Centroids.Add(PointCreator.CreateRandomPoint(random, UniqueColorGenerator.GenerateUniqueColor(), size));
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
            for (int i = 0; i < Centroids.Count; i++)
                RecreateCentroid(clusters[Centroids[i].Color], Centroids[i]);
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

        private static void RecreateCentroid(List<Point> cluster, Point centroid)
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