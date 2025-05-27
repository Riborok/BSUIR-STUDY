using System;
using System.Drawing;

namespace lab1.Drawable
{
    public class Line : DisplayObject
    {
        private int _x1, _y1;
        private int _x2, _y2;
        public Line(int baseX, int baseY, int x1, int y1, int x2, int y2, Color fill, int thickness, Color border)
            : base(
                baseX, baseY,
                Math.Min(x1, x2),
                Math.Min(y1, y2),
                Math.Max(x1, x2),
                Math.Max(y1, y2),
                fill, thickness, border)
        {
            _x1 = x1;
            _y1 = y1;
            _x2 = x2;
            _y2 = y2;
        }

        public override void Draw(Graphics graphics)
        {
            using (var pen = new Pen(_border, _thickness))
                graphics.DrawLine(pen, _x1, _y1, _x2, _y2);
        }
        
        public override void Update(int dx, int dy) {
            base.Update(dx, dy);
            _x1 += dx;
            _x2 += dx;
            _y1 += dy;
            _y2 += dy;
        }
    }
}