using System;
using System.Drawing;

namespace lab1.Drawable
{
    public class Triangle : DisplayObject 
    {
        private int _x1, _y1;
        private int _x2, _y2;
        private int _x3, _y3;

        public Triangle(int baseX, int baseY, int x1, int y1, int x2, int y2, int x3, int y3, Color fill, int thickness, Color border)
            : base(
                baseX, baseY,
                Math.Min(Math.Min(x1, x2), x3),
                Math.Min(Math.Min(y1, y2), y3),
                Math.Max(Math.Max(x1, x2), x3),
                Math.Max(Math.Max(y1, y2), y3),
                fill, thickness, border)
        {
            _x1 = x1;
            _y1 = y1;
            _x2 = x2;
            _y2 = y2;
            _x3 = x3;
            _y3 = y3;
        }

        public override void Draw(Graphics graphics)
        {
            using (var brush = new SolidBrush(Fill))
            using (var pen = new Pen(Border, Thickness))
            {
                var points = new[] { new Point(_x1, _y1), new Point(_x2, _y2), new Point(_x3, _y3) };
                graphics.FillPolygon(brush, points);
                graphics.DrawPolygon(pen, points);
            }
        }
        
        public override void Update(int x, int y) {
            int deltaX = x - BaseX;
            int deltaY = y - BaseY;

            _x1 += deltaX;
            _x2 += deltaX;
            _x3 += deltaX;
            _y1 += deltaY;
            _y2 += deltaY;
            _y3 += deltaY;
            
            base.Update(x, y);
        }
    }
}