
using System.Drawing.Drawing2D;
using oop3.Utilities;

namespace oop3.DisplayObjects
{
    internal class EllipseObject : DisplayObject
    {
        // radius properties of the ellipse
        protected double radiusX, radiusY;

        // ellipse center coordinates
        protected double centerX, centerY;

        // Initializer for all constructors of ellipse
        private void InitializeCoords(int centerCoordX, int centerCoordY, int radX, int radY)
        {
            centerX = centerCoordX;
            centerY = centerCoordY;
            radiusX = radX;
            radiusY = radY;
        }

        // Constructor for ellipse with fill color
        public EllipseObject(int centerX, int centerY, int radiusX, int radiusY, Color? fill = null) :
        base(centerX, centerY, radiusX * 2, radiusY * 2, fill)
        {
            InitializeCoords(centerX, centerY, radiusX, radiusY);
        }

        // Constructor for ellipse with texture filling
        public EllipseObject(int centerX, int centerY, int radiusX, int radiusY, Bitmap bmp) :
        base(centerX, centerY, radiusX * 2, radiusY * 2, bmp)
        {
            InitializeCoords(centerX, centerY, radiusX, radiusY);
        }

        protected override void UpdateFrame()
        {
            // for now the center is the anchor point
            // if there will be a change, the calculations
            // for the  point of rotation must be made
            // (there will be another field)

            double angle = VectorCalculations.Rad(rotAngle);

            int shiftX = (int)Math.Sqrt(
                Math.Pow(radiusX * Math.Cos(angle), 2) +
                Math.Pow(radiusY * Math.Sin(angle), 2)
                );
            int shiftY = (int)Math.Sqrt(
                Math.Pow(radiusX * Math.Sin(angle), 2) +
                Math.Pow(radiusY * Math.Cos(angle), 2)
                );

            frameX1 = centerX - shiftX;
            frameY1 = centerY - shiftY;
            frameX2 = frameX1 + shiftX * 2;
            frameY2 = frameY1 + shiftY * 2;
        }

        public override void Draw(Graphics g)
        {
            Brush strokeBrush = GetStrokeBrush();
            Brush fillBrush = GetFillBrush();

            GraphicsState prevState = MatrixRotate(g);
            Rectangle ellipse = new Rectangle((int)(centerX - radiusX), (int)(centerY - radiusY), (int)(radiusX * 2), (int)(radiusY * 2));
            g.FillEllipse(fillBrush, ellipse);
            g.DrawEllipse(new Pen(strokeBrush, strokeThickness), ellipse);

            g.Restore(prevState);
            //g.DrawRectangle(new Pen(strokeBrush), new Rectangle(frameX1, frameY1, frameX2 - frameX1, frameY2 - frameY1));
        }

        protected override void ShiftCoords(double deltaX, double deltaY)
        {
            centerX += deltaX;
            centerY += deltaY;
        }
    }
}
