using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Figures.Figures
{
    public class Circle : Ellipse
    {
        public Circle(float sFrameX, float sFrameY, float fFrameX, float fFrameY, int fR, int fG, int fB, int bR, int bG, int bB, int borderThickness)
            : base(sFrameX + borderThickness, sFrameY + borderThickness, fFrameX, fFrameY, fR, fG, fB, bR, bG, bB, borderThickness)
        {
            float circleSize = Math.Min(fFrameY - sFrameY, fFrameX - sFrameX);

            base.fFrameX = sFrameX + circleSize;
            base.fFrameY = sFrameY + circleSize;
        }

        public override void Draw(Graphics graphics)
        {

            using (var brush = new SolidBrush(Color.FromArgb(fR, fG, fB)))
            using (var pen = new Pen(Color.FromArgb(bR, bG, bB), borderThickness))
            {
                float x = sFrameX;
                float y = sFrameY;

                float circleSize = Math.Min(fFrameY - sFrameY, fFrameX - sFrameX);

                sBaseX = sFrameX + circleSize / 2;
                sBaseY = sFrameY + circleSize / 2;

                if (circleSize < 30) circleSize = 30;

                graphics.FillEllipse(brush, x, y, circleSize, circleSize);
                graphics.DrawEllipse(pen, x, y, circleSize, circleSize);
            }
        }
    }
}
