using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Figures.Figures
{
    public class Ellipse : DisplayObject
    {
        public Ellipse(float sFrameX, float sFrameY, float fFrameX, float fFrameY, int fR, int fG, int fB, int bR, int bG, int bB, int borderThickness)
            : base(sFrameX + borderThickness, sFrameY + borderThickness, fFrameX, fFrameY, fR, fG, fB, bR, bG, bB, borderThickness, sFrameX + (fFrameX - sFrameX) / 2, sFrameY + (fFrameY - sFrameY) / 2)
        {
        }

        public override void Draw(Graphics graphics)
        {
            using (var brush = new SolidBrush(Color.FromArgb(fR, fG, fB)))
            using (var pen = new Pen(Color.FromArgb(bR, bG, bB), borderThickness))
            {
                float x = sFrameX + borderThickness * 2;
                float y = sFrameY + borderThickness * 2;

                float sizeX = fFrameX - x;
                float sizeY = fFrameY - y;

                graphics.FillEllipse(brush, x, y, sizeX, sizeY);
                graphics.DrawEllipse(pen, x, y, sizeX, sizeY);
            }
        }

        public override void DrawText(Graphics graphics)
        {
        }

        public virtual void Update(int dx, int dy)
        {
            
        }
    }
}
