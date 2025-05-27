
using System.Drawing.Drawing2D;

namespace oop3.DisplayObjects
{
    internal class TriangleObject : DisplayObject
    {
        // Triangle coordinates
        protected double[] coordsX, coordsY;

        private void InitalizeCoords(int point1X, int point1Y, int point2X, int point2Y, int point3X, int point3Y)
        {
            coordsX = [point1X, point2X, point3X];
            coordsY = [point1Y, point2Y, point3Y];
        }

        // Constructor for triangle defined by 3 points and fill color
        public TriangleObject(int point1X, int point1Y, int point2X, int point2Y, int point3X, int point3Y, Color? fill = null) :
            this(SetBaseProperties(point1X, point1Y, point2X, point2Y, point3X, point3Y), fill)
        {
            InitalizeCoords(point1X, point1Y, point2X, point2Y, point3X, point3Y);
        }

        // Constructor for triangle defined by width and height and fill color
        public TriangleObject(int topLeftX, int topLeftY, int width, int height, Color? fill=null) :
            this(topLeftX, topLeftY + height, topLeftX + width / 2, topLeftY, topLeftX + width, topLeftY + height, fill)
        { }

        // Wrapper for constructor with a fill color
        private TriangleObject(Tuple<int, int, int, int> baseProperties, Color? fill=null) :
            base(baseProperties.Item1, baseProperties.Item2, baseProperties.Item3, baseProperties.Item4, fill)
        { }



        // Constructor for triangle defined by 3 points and a texture
        public TriangleObject(int point1X, int point1Y, int point2X, int point2Y, int point3X, int point3Y, Bitmap bmp) :
            this(SetBaseProperties(point1X, point1Y, point2X, point2Y, point3X, point3Y), bmp)
        {
            InitalizeCoords(point1X, point1Y, point2X, point2Y, point3X, point3Y);
        }

        // Constructor for triangle defined by width and height and a texture
        public TriangleObject(int topLeftX, int topLeftY, int width, int height, Bitmap bmp) :
            this(topLeftX, topLeftY + height, topLeftX + width / 2, topLeftY, topLeftX + width, topLeftY + height, bmp)
        { }

        // Wrapper for constructor with a texture
        private TriangleObject(Tuple<int, int, int, int> baseProperties, Bitmap bmp) :
            base(baseProperties.Item1, baseProperties.Item2, baseProperties.Item3, baseProperties.Item4, bmp)
        { }

        // Finding 
        private static Tuple<int, int, int, int> SetBaseProperties(int point1X, int point1Y, int point2X, int point2Y, int point3X, int point3Y)
        {
            int minX = Math.Min(point1X, Math.Min(point2X, point3X));
            int maxX = Math.Max(point1X, Math.Max(point2X, point3X));
            int minY = Math.Min(point1Y, Math.Min(point2Y, point3Y));
            int maxY = Math.Max(point1Y, Math.Max(point2Y, point3Y));

            return Tuple.Create((minX + maxX) / 2, (minY + maxY) / 2, maxX - minX, maxY - minY);
        }

        protected override void UpdateFrame()
        {
            SetFrameFromPoints(anchorX, anchorY, coordsX, coordsY);
        }

        public override void Draw(Graphics g)
        {
            Brush strokeBrush = GetStrokeBrush();
            Brush fillBrush = GetFillBrush();

            GraphicsState prevState = MatrixRotate(g);
            Point[] polygonPoints = new Point[] {
                new Point((int)coordsX[0],(int)coordsY[0]),
                new Point((int)coordsX[1],(int)coordsY[1]),
                new Point((int)coordsX[2],(int)coordsY[2])
            };
            g.FillPolygon(fillBrush, polygonPoints);
            g.DrawPolygon(new Pen(strokeBrush, strokeThickness), polygonPoints);

            g.Restore(prevState);
            //g.DrawRectangle(new Pen(strokeBrush), new Rectangle(frameX1, frameY1, frameX2 - frameX1, frameY2 - frameY1));
        }

        protected override void ShiftCoords(double deltaX, double deltaY)
        {
            for (int i = 0; i < coordsX.Length; i++)
            {
                coordsX[i] += deltaX;
                coordsY[i] += deltaY;
            }
        }
    }
}
