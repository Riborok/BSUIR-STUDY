using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Figures.Figures
{
    public class Line : DisplayObject
    {
        public Line(float sFrameX, float sFrameY, float fFrameX, float fFrameY, int fR, int fG, int fB, int bR, int bG, int bB, int borderThickness)
            : base(sFrameX + borderThickness, sFrameY + borderThickness, fFrameX, fFrameY, fR, fG, fB, bR, bG, bB, borderThickness, 0, 0)
        {
        }

        public override void Draw(Graphics graphics)
        {
            float startPointX = sFrameX, startPointY = sFrameY;

            float endPointX = fFrameX, endPointY = fFrameY;

            using (var pen = new Pen(Color.FromArgb(fR, fG, fB), borderThickness))
                graphics.DrawLine(pen, startPointX, startPointY, endPointX, endPointY);       
        }

        public override void DrawText(Graphics graphics)
        {
        }

        public virtual void Update(int dx, int dy)
        {
           
        }
    }
}
