using System;

namespace Lab1.KMeans
{
    public static class BoundsChecker
    {
        private const int MinObservationsCount = 1000;
        private const int MaxObservationsCount = 100_000;
        private const int MinCentroidsCount = 2;
        private const int MaxCentroidsCount = 20;

        public static int CheckObservationsCount(int observationsCount)
        {
            return Math.Max(MinObservationsCount, Math.Min(MaxObservationsCount, observationsCount));
        }

        public static int CheckCentroidsCount(int centroidsCount)
        {
            return Math.Max(MinCentroidsCount, Math.Min(MaxCentroidsCount, centroidsCount));
        }
    }
}