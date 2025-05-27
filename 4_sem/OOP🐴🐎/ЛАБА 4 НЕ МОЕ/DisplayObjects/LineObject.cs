
using System.Drawing.Drawing2D;

namespace oop3.DisplayObjects
{
    internal class LineObject : DisplayObject
    {
        // Line coordinates
        protected double x1, x2, y1, y2;

        // Initializer for all constructors of line
        private void InitializeCoords(int X1, int Y1, int X2, int Y2)
        {
            x1 = X1; x2 = X2; y1 = Y1; y2 = Y2;
        }


        // Constructor for line with fill color 
        public LineObject(int X1, int Y1, int X2, int Y2, Color? fill = null) :
            base((X1 + X2) / 2, (Y1 + Y2) / 2, Math.Abs(X1 - X2), Math.Abs(Y1 - Y2),
                fill)
        {
            InitializeCoords(X1, Y1, X2, Y2);
        }

        // Constructor for line with texture filling
        public LineObject(int X1, int Y1, int X2, int Y2, Bitmap bmp) :
            base((X1 + X2) / 2, (Y1 + Y2) / 2, Math.Abs(X1 - X2), Math.Abs(Y1 - Y2),
                bmp)
        {
            InitializeCoords(X1, Y1, X2, Y2);
        }

        protected override void UpdateFrame()
        {
            // pass it to the method
            double[] pointsX = [x1, x2];
            double[] pointsY = [y1, y2];

            SetFrameFromPoints(anchorX, anchorY, pointsX, pointsY);
        }

        public override void Draw(Graphics g)
        {
            Brush strokeBrush = GetStrokeBrush();
            Brush fillBrush = GetFillBrush();

            GraphicsState prevState = MatrixRotate(g);

            g.DrawLine(new Pen(fillBrush, strokeThickness), new Point((int)x1, (int)y1), new Point((int)x2, (int)y2));

            g.Restore(prevState);
            //g.DrawRectangle(new Pen(strokeBrush), new Rectangle(frameX1, frameY1, frameX2 - frameX1, frameY2 - frameY1));
        }

        protected override void ShiftCoords(double deltaX, double deltaY)
        {
            x1 += deltaX;
            y1 += deltaY;

            x2 += deltaX;
            y2 += deltaY;
        }
    }
}
