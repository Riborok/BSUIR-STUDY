using System;
using System.Collections.Generic;
using System.Drawing;
using Lab2.Geometry;
using Point = Lab2.Geometry.Point;

namespace Lab2.KMeans_Maximin
{
    public class MaximinSolver
    {
        private static readonly Color DefaultObservationColor = Color.White;

        public Point[] Observations { get; }
        public List<Point> Centroids { get; }
        
        private readonly CentroidSumDist _centroidSumDist = new CentroidSumDist();
        private bool _isContinue = true;
        
        public MaximinSolver(int observationsCount, Size size)
        {
            Observations = new Point[observationsCount];
            Centroids = new List<Point>();

            FillObservations(size);
            AddNewCentroid(Observations[0]);
            AddNewCentroid(ComputeClusterUpdate(Centroids[0], Observations).NewCentroid);
            AssignObservationsToNearestCluster();
        }

        private void FillObservations(Size size)
        {
            Random random = new Random();
            for (int i = 0; i < Observations.Length; i++)
                Observations[i] = PointCreator.CreateRandomPoint(random, DefaultObservationColor, size);
        }

        public bool RunMaximinAlgorithm()
        {
            if (_isContinue)
            {
                var maxClusterUpdate = FindMaxClusterUpdate();
                UpdateCentroidSumDist();

                if (maxClusterUpdate.MaxDistance > _centroidSumDist.HalfAverage)
                {
                    AddNewCentroid(maxClusterUpdate.NewCentroid);
                    AssignObservationsToNearestCluster();
                }
                else
                    _isContinue = false;
            }
            return _isContinue;
        }

        private void AssignObservationsToNearestCluster()
        {
            foreach (var observation in Observations)
                observation.Color = FindNearestClusterColor(observation);
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
        
        private void UpdateCentroidSumDist()
        {
            int lastIndex = Centroids.Count - 1;
            var lastCentroid = Centroids[lastIndex];
            for (int i = 0; i < lastIndex; i++)
                _centroidSumDist.SumWith(MathUtils.CalcEuclideanDistance(Centroids[i], lastCentroid));
        }

        private ClusterUpdate FindMaxClusterUpdate()
        {
            var maxClusterUpdate = new ClusterUpdate();

            var clusters = CreateClusters();
            foreach (var centroid in Centroids)
            {
                var clusterUpdate = ComputeClusterUpdate(centroid, clusters[centroid.Color]);
                if (clusterUpdate.MaxDistance > maxClusterUpdate.MaxDistance)
                    maxClusterUpdate = clusterUpdate;
            }

            return maxClusterUpdate;
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

        private static ClusterUpdate ComputeClusterUpdate(Point centroid, IEnumerable<Point> points)
        {
            var clusterUpdate = new ClusterUpdate();

            foreach (var observation in points)
            {
                double distance = MathUtils.CalcEuclideanDistance(observation, centroid);
                if (distance > clusterUpdate.MaxDistance)
                {
                    clusterUpdate.MaxDistance = distance;
                    clusterUpdate.NewCentroid = observation;
                }
            }

            return clusterUpdate;
        }

        private void AddNewCentroid(Point centroid)
        {
            centroid.Color = UniqueColorGenerator.GenerateUniqueColor();
            Centroids.Add(centroid);
        }

        private class CentroidSumDist
        {
            private double _sum;
            private int _count;

            public void SumWith(double distance)
            {
                _sum += distance;
                _count++;
            }
            
            public double HalfAverage => _sum / _count / 2.0;
        }

        private struct ClusterUpdate
        {
            public double MaxDistance { get; set; }
            public Point NewCentroid { get; set; }
        }
    }
}