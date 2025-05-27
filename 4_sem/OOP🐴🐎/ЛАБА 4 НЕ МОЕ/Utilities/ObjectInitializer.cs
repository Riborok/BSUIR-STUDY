using oop3.DisplayObjects;

namespace oop3.Utilities
{
    internal class ObjectInitializer
    {
        private static Random rand = new Random();

        private static readonly int WIDTH_MIN = 50;
        private static readonly int WIDTH_MAX = 150;
        private static readonly int HEIGHT_MIN = 50;
        private static readonly int HEIGHT_MAX = 150;
        public static int generateWidth() => rand.Next(WIDTH_MIN, WIDTH_MAX);
        public static int generateHeight() => rand.Next(HEIGHT_MIN, HEIGHT_MAX);
        public static int generateAngle() => (int)(rand.NextDouble() * 360);

        private static readonly int velModuloMin = 50;
        private static readonly int velModuloMax = 300;
        private static readonly int accModuloMin = 20;
        private static readonly int accModuloMax = 100;

        public static (int, int) generateVelocityVector() => (rand.Next(velModuloMin, velModuloMax), generateAngle());
        public static (int, int) generateAccelerationVector() => (rand.Next(accModuloMin, accModuloMax), generateAngle());


        private static int actualW = 0, actualH = 0;

        delegate DisplayObject genObject();
        private static readonly int amntOfObjects = 42;
        private static bool textureMode = false;
        private static int textureIndices = 10;
        private static string txtrDir = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\textures\"));
        private static Bitmap[] txtrBmps = new Bitmap[textureIndices];

        //private static genObject[] genDelegates = new genObject[] { genRandomRect, genRandomSquare, genRandomEllipse, genRandomCircle, genRandomLine, genRandomTriangle };
        //private static genObject[] genDelegates = new genObject[] { genRandomRect };
        private static genObject[] genDelegates = new genObject[] { genRandomCircle };
        public static (GameField, DisplayObject[]) generateDrawField(int leftTopX, int leftTopY, int bottomRightX, int bottomRightY, int thickness, bool acceleration)
        {

            GameField CDrawField = new GameField(leftTopX, leftTopY, bottomRightX, bottomRightY, thickness);
            CDrawField.fillColor = Color.FromArgb(255, 255, 218);
            CDrawField.strokeColor = Color.FromArgb(20, 20, 210);
            //CDrawField.setRotationAngle(Math.PI / 6);

            InitializeGenerators(bottomRightX - leftTopX, bottomRightY - leftTopY, thickness);

            DisplayObject[] objects = genObjects(genDelegates, acceleration);
            objects = genObjectsCircles(); 

            return (CDrawField, objects);

        }
        private static DisplayObject[] genObjectsCircles() {
            DisplayObject[] objects = new DisplayObject[amntOfObjects];
            CircleObject obj;
    
            for (int j = 0; j < amntOfObjects; j++)
            {
                // acquire current generation 
                bool isValid = false;
                obj = genRandomCircle() as CircleObject;
                while (!isValid) {
                    isValid = true;
                    obj = genRandomCircle() as CircleObject;
                    for (int i = 0; i < j; i++) {
                        if (isValid) {
                            isValid = !obj.CheckCircleCollision(objects[i] as CircleObject);
                        }
                    }
                }

                if (textureMode)
                {
                    obj.fillBMP = (txtrBmps[j % textureIndices]);
                }
                else
                {
                    Color clr = getRandomColor();
                    obj.fillColor = Color.FromArgb(clr.R, clr.G, clr.B);
                }

                //field.AddObject(obj);
                objects[j] = obj;
                (int velocityModulo, int velocityAngle) = generateVelocityVector();
                obj.velModulo = velocityModulo;
                obj.velAlpha = velocityAngle;
/*
                if (isAccelerated)
                {
                    (int accelerationModulo, int accelerationAngle) = generateAccelerationVector();
                    obj.accModulo = accelerationModulo;
                    obj.accAlpha = accelerationAngle;
                }*/

            }
            
            return objects;
        }

        private static DisplayObject[] genObjects(genObject[] generationArr, bool isAccelerated)
        {
            DisplayObject[] objects = new DisplayObject[generationArr.Length * amntOfObjects];
            DisplayObject obj;
            for (int i = 0; i < generationArr.Length; i++)
            {
                for (int j = 0; j < amntOfObjects; j++)
                {
                    // acquire current generation 
                    obj = generationArr[i]();
                    if (textureMode)
                    {
                        obj.fillBMP = (txtrBmps[j % textureIndices]);
                    }
                    else
                    {
                        Color clr = getRandomColor();
                        obj.fillColor = Color.FromArgb(clr.R, clr.G, clr.B);
                    }

                    //field.AddObject(obj);
                    objects[i * amntOfObjects + j] = obj;
                    (int velocityModulo, int velocityAngle) = generateVelocityVector();
                    obj.velModulo = velocityModulo;
                    obj.velAlpha = velocityAngle;

                    if (isAccelerated)
                    {
                        (int accelerationModulo, int accelerationAngle) = generateAccelerationVector();
                        obj.accModulo = accelerationModulo;
                        obj.accAlpha = accelerationAngle;
                    }

                }
            }
            return objects;
        }

        public static void InitializeGenerators(int fieldWidth, int fieldHeight, int borderThick)
        {
            actualW = fieldWidth - 2 * borderThick;
            actualH = fieldHeight - 2 * borderThick;

            for (int i = 0; i < textureIndices; i++)
            {
               // txtrBmps[i] = new Bitmap(Path.Combine(txtrDir, "txtr" + i % textureIndices + ".bmp"));
            }
        }

        /*
        public static void InitializeGenerators(int fieldWidth, int fieldHeight, int borderThick)
        {
            fieldW = fieldWidth;
            fieldH = fieldHeight;
            borderT = borderThick;

            actualW = fieldW - 2 * borderT;
            actualH = fieldH - 2 * borderT;

            for (int i = 0; i < textureIndices; i++)
            {
                txtrBmps[i] = new Bitmap(System.IO.Path.Combine(txtrDir, "txtr" + (i % textureIndices) + ".bmp"));
            }
        }
        public static void InitializeGenerators(int absTopLeftX, int absTopLeftY, int fieldWidth, int fieldHeight, int borderThick)
        {
            topLeftX = absTopLeftX;
            topLeftY = absTopLeftY;
            InitializeGenerators(fieldWidth,fieldHeight, borderThick);
        }
        */

        public static DisplayObject genRandomRect()
        {
            int width = generateWidth();
            int height = generateHeight();
            Point p = generateTopLeftRel(width, height);
            int topLeftX = p.X;
            int topLeftY = p.Y;
            RectangleObject ro = new RectangleObject(topLeftX, topLeftY, topLeftX + width, topLeftY + height);
            ro.SetRotationAngle(generateAngle());
            return ro;
        }

        public static DisplayObject genRandomSquare()
        {
            int width = generateWidth();
            Point p = generateTopLeftRel(width, width);
            int topLeftX = p.X;
            int topLeftY = p.Y;

            SquareObject so = new SquareObject(topLeftX, topLeftY, width);
            so.SetRotationAngle(generateAngle());
            return so;
        }
        public static DisplayObject genRandomEllipse()
        {
            int width = generateWidth();
            int height = generateHeight();
            Point p = generateAnchorRel(width, height);
            int centerX = p.X;
            int centerY = p.Y;
            EllipseObject eo = new EllipseObject(centerX, centerY, width / 2, height / 2);
            eo.SetRotationAngle(generateAngle());
            return eo;
        }

        public static DisplayObject genRandomCircle()
        {
            int width = generateWidth();
            Point p = generateAnchorRel(width, width);
            int centerX = p.X;
            int centerY = p.Y;
            CircleObject co = new CircleObject(centerX, centerY, width / 2);
            co.SetRotationAngle(generateAngle());
            return co;
        }

        public static DisplayObject genRandomTriangle()
        {
            int width = generateWidth();
            int height = generateHeight();
            Point p = generateTopLeftRel(width, height);
            int topLeftX = p.X;
            int topLeftY = p.Y;
            TriangleObject to = new TriangleObject(topLeftX, topLeftY, width, height);
            to.SetRotationAngle(generateAngle());
            return to;
        }
        public static DisplayObject genRandomLine()
        {
            int width = generateWidth();
            int height = generateHeight();
            Point anc = generateTopLeftRel(width, height);
            Point p1 = new Point(anc.X, anc.Y);
            Point p2 = new Point(anc.X + width, anc.Y + height);
            int x1 = anc.X;
            int y1 = anc.Y;
            int x2 = anc.X + width;
            int y2 = anc.Y + height;
            LineObject lo = new LineObject(x1, y1, x2, y2);
            lo.SetStrokeThickness(5);
            lo.SetRotationAngle(generateAngle());
            return lo;
        }


        /// <summary>
        /// Given a width and a height, generates an anchor that will satisfy the quality of not being cut off by the window frame
        /// </summary>
        /// <param name="width">Width of the not-rotated frame of the object</param>
        /// <param name="height">Height of the not-rotated frame of the object</param>
        /// <returns>Anchor point</returns>
        //
        public static Point generateAnchorRel(double width, double height)
        {
            // acquiring left top corner coordinates
            //
            // these coordinates must be calculated based on the idea
            // that the shape can be cut by window frame because of the 
            // position of the left top corner. So the top left corner
            // must be generated in bounds that include the case in which
            // the rotation is such that it is maximum length in any direction
            double maxBump;
            // as we calculate the rotation from the center of the object, the
            // maxBump will be calculated as half of the actual hypotenuse of
            // the frame
            maxBump = Math.Sqrt(Math.Pow(width, 2) + Math.Pow(height, 2)) / 2;

            // that way, the minimum coordinate value for X is maxBump, while
            // the minimum is fieldWidth-maxBump. Same applies to Y coordinate
            Point anchor = new Point();
            anchor.X = (int)Math.Floor(rand.NextDouble() * (actualW - 2 * maxBump) + maxBump);
            anchor.Y = (int)Math.Floor(rand.NextDouble() * (actualH - 2 * maxBump) + maxBump);
            return anchor;
        }

/*        public static Point generateAnchorAbs(double width, double height)
        {
            Point p = generateAnchorRel(width, height);
            p.X += topLeftX + borderT;
            p.Y += topLeftY + borderT;
            return p;
        }*/
        public static Point generateTopLeftRel(int width, int height)
        {
            Point topLeft = generateAnchorRel(width, height);
            topLeft.X -= width / 2;
            topLeft.Y -= height / 2;
            return topLeft;
        }
/*        public static Point generateTopLeftAbs(int width, int height)
        {
            Point topLeft = generateTopLeftRel(width, height);
            topLeft.X += topLeftX;
            topLeft.Y += topLeftY;
            return topLeft;
        }*/

        public static Color getRandomColor()
        {
            return Color.FromArgb(255, (byte)rand.Next(), (byte)rand.Next(), (byte)rand.Next());
        }
    }
}
