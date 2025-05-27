using System;

namespace Lab2.Geometry
{
    public static class MathUtils
    {
        public static double CalcEuclideanDistance(Point a, Point b)
        {
            int deltaX = b.X - a.X;
            int deltaY = b.Y - a.Y;
            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }
    }
}