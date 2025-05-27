using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Game = Figures.Game;

namespace Figures.Figures
{
    public class Triangle : DisplayObject
    {
        public Triangle(float sFrameX, float sFrameY, float fFrameX, float fFrameY, int fR, int fG, int fB, int bR, int bG, int bB, int borderThickness)
            : base(sFrameX + borderThickness, sFrameY + borderThickness, fFrameX, fFrameY, fR, fG, fB, bR, bG, bB, borderThickness, sFrameX + (fFrameX - sFrameX) / 2, sFrameY + (fFrameY - sFrameY) / 2)
        {
            float frameSize = Math.Min(fFrameY - sFrameY, fFrameX - sFrameX);

            base.fFrameX = sFrameX + frameSize;
            base.fFrameY = sFrameY + frameSize;
        }

        private Point[] getTrianglePoints()
        {
            float frameSize = Math.Min(fFrameY - sFrameY, fFrameX - sFrameX);

            fFrameX = sFrameX + frameSize;
            fFrameY = sFrameY + frameSize;

            Point firstPoint = new Point((int)sFrameX, (int)(sFrameY + frameSize));
            Point secondPoint = new Point((int)(sFrameX + frameSize), (int)(sFrameY + frameSize));
            Point thirdPoint = new Point((int)(sFrameX + frameSize / 2), (int)sFrameY);

            sBaseX = sFrameX + frameSize / 2;
            sBaseY = sFrameY + frameSize / 2;

            return new Point[] {firstPoint, secondPoint, thirdPoint};
        }

        public override void Draw(Graphics graphics)
        {
            using (var brush = new SolidBrush(Color.FromArgb(fR, fG, fB)))
            using (var pen = new Pen(Color.FromArgb(bR, bG, bB), borderThickness))
            {
                Point[] trianglePoints = getTrianglePoints();
                
                graphics.FillPolygon(brush, trianglePoints);
                graphics.DrawPolygon(pen, trianglePoints);
            }
        }

        public virtual void Update(int dx, int dy)
        {

        }

        public override void DrawText(Graphics graphics)
        {

        }
    }
}
