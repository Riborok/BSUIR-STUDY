using System.Drawing;

namespace lab1.Drawable
{
    public class Ellipse : DisplayObject
    {
        public Ellipse(int baseX, int baseY, int x, int y, int width, int height, Color fill, int thickness, Color border)
            : base(baseX, baseY, x, y, x + width, y + height, fill, thickness, border)
        {
        }

        public override void Draw(Graphics graphics)
        {
            using (var brush = new SolidBrush(Fill))
            using (var pen = new Pen(Border, Thickness))
            {
                int width = X2 - X1;
                int height = Y2 - Y1;
                graphics.FillEllipse(brush, X1, Y1, width, height);
                graphics.DrawEllipse(pen, X1, Y1, width, height);
            }
        }
    }
}