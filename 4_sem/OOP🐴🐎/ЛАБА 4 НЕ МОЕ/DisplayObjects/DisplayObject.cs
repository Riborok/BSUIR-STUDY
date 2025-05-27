using System.Drawing.Drawing2D;
using oop3.Utilities;

namespace oop3.DisplayObjects
{
    public abstract class DisplayObject
    {
        public bool R;
        // new velocity vector of the object
        public double newVelModulo;
        public double newVelAlpha;


        // frame that surrounds the drawn rotated object 
        public double frameX1;
        public double frameY1;
        public double frameX2;
        public double frameY2;

        // anchor point (center of the shape)
        public double anchorX;
        public double anchorY;

        // rotation angle, in degrees 
        protected int rotAngle;

        // colors of fill of the shape
        private Color _fillColor;
        private Bitmap _fillBMP;
        private bool colorFill;
        public Color fillColor {
            get { 
                return _fillColor; 
            }
            set {
                _fillColor = value;
                colorFill = true;    
            } 
        }
        public Bitmap fillBMP
        {
            get {
                return _fillBMP;
            }
            set {
                _fillBMP = value;
                colorFill = false;
            }
        }

        // colors of stroke of the shape
        public Color strokeColor;
        public int strokeThickness;

        // velocity vector of the object
        public double velModulo;
        public double velAlpha;



        // acceleration vector of the object
        public double accModulo;
        public double accAlpha;

        // current trigger state of DisplayObject
        protected bool isClicked;
        public delegate void ClickHandler();
        public ClickHandler? clickHandler;

        // Click check on the DisplayObject
        public virtual bool Click(int mouseX, int mouseY)
        {
            isClicked = (mouseX >= frameX1) && (mouseX <= frameX2) &&
                (mouseY >= frameY1) && (mouseY <= frameY2);
            if (isClicked)
            {
                clickHandler?.Invoke();
            }
            return isClicked;            
        }

        // Constructor for a DisplayObject with default color
        private DisplayObject(int anchX, int anchY, int objWidth, int objHeight, Color? color, Bitmap bmp)
        {
            anchorX = anchX;
            anchorY = anchY;

            frameX1 = anchX - objWidth / 2;
            frameY1 = anchY - objHeight / 2;
            frameX2 = frameX1 + objWidth;
            frameY2 = frameY1 + objHeight;

            rotAngle = 0;

            velModulo = 0;
            velAlpha = 0;

            accModulo = 0;
            accAlpha = 0;

            isClicked = false;
            clickHandler = null;

            R = false;

            if (color != null)
            {
                fillColor = color.Value;
            }
            else if (bmp != null)
            {
                fillBMP = bmp;
            }
            else { 
                fillColor = Color.Black;
            }

            strokeColor = Color.Black;
            strokeThickness = 1;
        }

        // Constructor for DisplayObject with fill color
        public DisplayObject(int anchX, int anchY, int objWidth, int objHeight, Color? fill = null) :
            this(anchX, anchY, objWidth, objHeight, fill, null)
        {
        }

        // Constructor for DisplayObject with fill texture
        public DisplayObject(int anchX, int anchY, int objWidth, int objHeight, Bitmap bmp) :
            this(anchX, anchY, objWidth, objHeight, null, bmp)
        {
        }

        protected DisplayObject(int anchorX, int anchorY, int v1, double v2, Color black)
        {
            this.anchorX = anchorX;
            this.anchorY = anchorY;
        }

        // Setting the rotation angle of the object
        public void SetRotationAngle(int angle)
        {
            rotAngle = angle;
            UpdateFrame();
        }

        // Setting the stroke thickness
        public void SetStrokeThickness(int thickness)
        {
            strokeThickness = thickness;
        }

        // Acquiring relevant fill brush for drawing the object
        protected Brush GetFillBrush()
        {
            if (colorFill)
                return new SolidBrush(fillColor);
            else
                return new TextureBrush(fillBMP);
        }

        // Acquiring relevant stroke brush for drawing the object
        protected Brush GetStrokeBrush()
        {
            return new SolidBrush(strokeColor);
        }

        // Each time rotation angle changes, the frame that wraps the object
        // must be updated. This method must provide the update of the frame
        // that wraps the object.
        protected abstract void UpdateFrame();

        // Set the frame relative to the anchor point based on the 
        // point collection that describes the object.  
        protected void SetFrameFromPoints(double rotPointX, double rotPointY, double[] pointsX, double[] pointsY)
        {

            int? minX = null, minY = null, maxX = null, maxY = null;

            // Acquiring min and max coordinates of rotated points
            int tempX, tempY;
            double angle = VectorCalculations.Rad(rotAngle);
            for (int i = 0; i < pointsX.Length; i++)
            {
                (tempX, tempY) = VectorCalculations.getVector((int)rotPointX, (int)rotPointY, (int)pointsX[i], (int)pointsY[i]);
                (tempX, tempY) = VectorCalculations.rotateVector(tempX, tempY, angle);
                minX = minX == null ? tempX : tempX < minX ? tempX : minX;
                maxX = maxX == null ? tempX : tempX > maxX ? tempX : maxX;
                minY = minY == null ? tempY : tempY < minY ? tempY : minY;
                maxY = maxY == null ? tempY : tempY > maxY ? tempY : maxY;
            }

            // calculating frame constraints
            frameX1 = rotPointX + (minX.HasValue ? minX.Value : 0);
            frameY1 = rotPointY + (minY.HasValue ? minY.Value : 0);
            frameX2 = rotPointX + (maxX.HasValue ? maxX.Value : 0);
            frameY2 = rotPointY + (maxY.HasValue ? maxY.Value : 0);
        }

        // Each method must implement Draw method using GDI
        // object Graphics that allows for that action
        public abstract void Draw(Graphics g);

        // Preserve current transformation matrix in order to
        // not affect it by the rotations made while drawing
        // the object
        protected GraphicsState MatrixRotate(Graphics g)
        {
            GraphicsState prevState = g.Save();

            Matrix m = new Matrix(g.TransformElements);
            m.RotateAt(rotAngle, new PointF((float)anchorX, (float)anchorY));
            g.Transform = m;
            return prevState;
        }

        // Check if the object is in the bounds of a frame
        public bool IsInBounds(double borderX1, double borderY1, double borderX2, double borderY2)
        {
            return
                frameX1 > borderX1 &&
                frameY1 > borderY1 &&
                frameX2 < borderX2 &&
                frameY2 < borderY2;
        }

        // Check if the frames of the objects collide
        public bool FrameCollision(DisplayObject obj) {
            return !(
                // intersection of x coords
                (this.frameX2<obj.frameX1||this.frameX1>obj.frameX2)
                ||
                // intersection of y coords
                (this.frameY2<obj.frameY1||this.frameY1>obj.frameY2)
                );
        }

        // Move the object 
        public void Move(double time)
        {
            double angleVel = (velAlpha * Math.PI / 180);
            double angleAcc = (accAlpha * Math.PI / 180);

            
            double shiftX = (time * (Math.Cos(angleAcc) * accModulo * time / 2 + Math.Cos(angleVel) * velModulo));
            double shiftY = (time * (Math.Sin(angleAcc) * accModulo * time / 2 + Math.Sin(angleVel) * velModulo));

            //int deltaX = deltaTime * (accX * deltaTime / 2 + velX);
            //int deltaY = deltaTime * (accY * deltaTime / 2 + velY);

            // accelerated mode recalculate vector with trigonometry
            //velX += accX * deltaTime;
            //velY += accY * deltaTime;

            ShiftObject(shiftX, shiftY);
        }

        // Shift coordinates of the object by a given value
        protected abstract void ShiftCoords(double deltaX, double deltaY);

        // Shift anchor and frame of the object, then - shift
        // the coordinates of the object itself
        public void ShiftObject(double deltaX, double deltaY)
        {
            anchorX += deltaX;
            anchorY += deltaY;

            frameX1 += deltaX;
            frameY1 += deltaY;
            frameX2 += deltaX;
            frameY2 += deltaY;
            ShiftCoords(deltaX, deltaY);
        }

        //fast collision check
        public bool FramesCollision(DisplayObject obj)
        {
            return (
                // Upper Left Corner
                (this.frameX1 >= obj.frameX1 && this.frameX1 <= obj.frameX2 && this.frameY1 >= obj.frameY1 && this.frameY1 <= obj.frameY2) ||
                // Upper Right Corner
                (this.frameX2 >= obj.frameX1 && this.frameX2 <= obj.frameX2 && this.frameY1 >= obj.frameY1 && this.frameY1 <= obj.frameY2) ||
                // Bottom Left Corner
                (this.frameX1 >= obj.frameX1 && this.frameX1 <= obj.frameX2 && this.frameY2 >= obj.frameY1 && this.frameY2 <= obj.frameY2) ||
                // Bottom Right Corner
                (this.frameX2 >= obj.frameX1 && this.frameX2 <= obj.frameX2 && this.frameY2 >= obj.frameY1 && this.frameY2 <= obj.frameY2)
            );
        }

        public double GetCTForBorders(double borX1, double borY1, double borX2, double borY2) {
            double timeX = -1;
            double timeY = -1;

            double vx = velModulo * Math.Cos(velAlpha * Math.PI / 180);
            if (vx != 0) { 
                double distX = vx < 0 ? (borX1 - frameX1) : (borX2 - frameX2);
                timeX = distX / vx;
            }

            double vy = velModulo * Math.Sin(velAlpha * Math.PI / 180);
            if (vy != 0) { 
                double distY = vy == 0 ? -1 : (vy < 0 ? (borY1 - frameY1) : (borY2 - frameY2));
                timeY = distY / vy;
            }

            if (timeX * timeY > 0)
            {
                if (timeX < 0)
                {
                    return timeY;       // both are -1 or so; the obj isn't moving
                }
                else
                {
                    return timeX < timeY ? timeX : timeY;
                }
            }
            else { 
                return timeX < timeY ? timeY : timeX;       // ! different condition !
            }
            
        }
    }
}
