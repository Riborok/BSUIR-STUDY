using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Security.Policy;

namespace Figures.Figures
{
    public class Square : Rectangle
    {
        public Square(float sFrameX, float sFrameY, float fFrameX, float fFrameY, int fR, int fG, int fB, int bR, int bG, int bB, int borderThickness)
            : base(sFrameX + borderThickness, sFrameY + borderThickness, fFrameX, fFrameY, fR, fG, fB, bR, bG, bB, borderThickness)
        {
            float squareSize = Math.Min(fFrameY - sFrameY, fFrameX - sFrameX);

            base.fFrameX = sFrameX + squareSize;
            base.fFrameY = sFrameY + squareSize;
        }

        public override void Draw(Graphics graphics)
        {
            using (var brush = new SolidBrush(Color.FromArgb(fR, fG, fB)))
            using (var pen = new Pen(Color.FromArgb(bR, bG, bB), borderThickness))
            {
                float x = sFrameX;
                float y = sFrameY;

                float squareSize = Math.Min(fFrameY - sFrameY, fFrameX - sFrameX);
                
                sBaseX = sFrameX + squareSize / 2;
                sBaseY = sFrameY + squareSize / 2;

                if (squareSize < 30) squareSize = 30;

                graphics.FillRectangle(brush, x, y, squareSize, squareSize);
                graphics.DrawRectangle(pen, x, y, squareSize, squareSize);
            }
        }
    }
}
