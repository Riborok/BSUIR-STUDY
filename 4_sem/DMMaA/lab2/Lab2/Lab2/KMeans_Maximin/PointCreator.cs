using System;
using System.Drawing;
using Point = Lab2.Geometry.Point;

namespace Lab2.KMeans_Maximin
{
    public static class PointCreator
    {
        private const int Indent = 15;
        public static Point CreateRandomPoint(Random random, Color color, Size size)
        {
            int x = random.Next(Indent, size.Width - Indent);
            int y = random.Next(Indent, size.Height - Indent);
            return new Point(x, y, color);
        }
    }
}