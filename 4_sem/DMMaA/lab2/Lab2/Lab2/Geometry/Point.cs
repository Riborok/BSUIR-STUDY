using System.Drawing;

namespace Lab2.Geometry
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Color Color { get; set; }

        public Point(int x, int y, Color color)
        {
            X = x;
            Y = y;
            Color = color;
        }
    }

}