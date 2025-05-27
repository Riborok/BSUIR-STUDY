using System.Drawing.Drawing2D;

namespace oop3.DisplayObjects
{
    internal class RectangleObject : DisplayObject
    {
        // Rectangle coordinates
        protected double rectX1, rectY1, rectX2, rectY2;

        // Initializer for all constructors of rectangle
        private void InitializeCoords(int topLeftX, int topLeftY, int bottomRightX, int bottomRightY)
        {
            rectX1 = topLeftX;
            rectY1 = topLeftY;
            rectX2 = bottomRightX;
            rectY2 = bottomRightY;
        }

        // Constructor for rectangle with fill color
        public RectangleObject(int topLeftX, int topLeftY, int bottomRightX, int bottomRightY, Color? fill = null) :
        base((topLeftX + bottomRightX) / 2, (topLeftY + bottomRightY) / 2, bottomRightX - topLeftX, bottomRightY - topLeftY, fill)
        {
            InitializeCoords(topLeftX, topLeftY, bottomRightX, bottomRightY);
        }

        // Constructor for rectangle with texture filling
        public RectangleObject(int topLeftX, int topLeftY, int bottomRightX, int bottomRightY, Bitmap bmp) :
        base((topLeftX + bottomRightX) / 2, (topLeftY + bottomRightY) / 2, bottomRightX - topLeftY, bottomRightY - topLeftY, bmp)
        {
            InitializeCoords(topLeftX, topLeftY, bottomRightX, bottomRightY);
        }

        protected override void UpdateFrame()
        {
            // ideally there must be a point around which the rotation will be completed
            // and so the anchor point must be recalculated beforehand
            // form the point collection 
            double [] coordsX = [rectX1, rectX2, rectX1, rectX2];
            double [] coordsY = [rectY1, rectY1, rectY2, rectY2];

            // pass it to the method
            SetFrameFromPoints(anchorX, anchorY, coordsX, coordsY);
        }

        public override void Draw(Graphics g)
        {
            Brush strokeBrush = GetStrokeBrush();
            Brush fillBrush = GetFillBrush();

            GraphicsState prevState = MatrixRotate(g);
            Rectangle rect = new Rectangle((int)rectX1, (int)rectY1, (int)(rectX2 - rectX1), (int)(rectY2 - rectY1));
            g.FillRectangle(fillBrush, rect);
            g.DrawRectangle(new Pen(strokeBrush, strokeThickness), rect);

            g.Restore(prevState);
            //g.DrawRectangle(new Pen(strokeBrush), new Rectangle(frameX1, frameY1, frameX2 - frameX1, frameY2 - frameY1));
        }

        protected override void ShiftCoords(double deltaX, double deltaY)
        {
            rectX1 += deltaX;
            rectY1 += deltaY;
            rectX2 += deltaX;
            rectY2 += deltaY;
        }
    }
}
