using System.Drawing;

namespace Lab1.Geometry
{
    public class PointDrawer
    {
        private readonly Point[] _points;
        private readonly int _radius;

        public PointDrawer(Point[] points, int radius)
        {
            _points = points;
            _radius = radius;
        }

        public void DrawPoints(Graphics graphics)
        {
            foreach (var point in _points)
                DrawPoint(graphics, point);
        }

        private void DrawPoint(Graphics graphics, Point p)
        {
            int diameter = _radius * 2;
            int x = p.X - _radius;
            int y = p.Y - _radius;

            using (Brush brush = new SolidBrush(p.Color))
                graphics.FillEllipse(brush, x, y, diameter, diameter);
        }
    }
}