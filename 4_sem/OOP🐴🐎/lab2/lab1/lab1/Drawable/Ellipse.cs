using System.Drawing;

namespace lab1.Drawable
{
    public class Ellipse : DisplayObject
    {
        protected int _x1, _y1;
        protected int _x2, _y2;
        
        public Ellipse(int baseX, int baseY, int x, int y, int width, int height, Color fill, int thickness, Color border)
            : base(baseX, baseY, x, y, x + width, y + height, fill, thickness, border)
        {
            _x1 = x;
            _y1 = y;
            _x2 = x + width;
            _y2 = y + height;
        }

        public override void Draw(Graphics graphics)
        {
            using (var brush = new SolidBrush(_fill))
            using (var pen = new Pen(_border, _thickness))
            {
                int width = _x2 - _x1;
                int height = _y2 - _y1;
                graphics.FillEllipse(brush, _x1, _y1, width, height);
                graphics.DrawEllipse(pen, _x1, _y1, width, height);
            }
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