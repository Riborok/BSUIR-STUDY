using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Figures.Menu;

namespace Figures.Figures
{
    public class GameField : Rectangle
    {
        Random random;

        internal int gamingFieldWidth;
        internal int gamingFieldHeight;

        public DisplayObject[] objectsToRender = new DisplayObject[3];
        
        int curPos = 0;

        internal int windowWidth;
        internal int windowHeight;

        const int screenShift = 30;

        public GameField(int sFrameX, int sFrameY, int fFrameX, int fFrameY, int fR, int fG, int fB, int bR, int bG, int bB, int borderThickness, int wWidth, int wHeight)
            : base(sFrameX - borderThickness, sFrameY - borderThickness, fFrameX, fFrameY, fR, fG, fB, bR, bG, bB, borderThickness)
        {
            random = new Random();

            windowWidth = wWidth;
            windowHeight = wHeight;

            gamingFieldWidth = windowWidth - screenShift * 2;
            gamingFieldHeight = windowHeight - screenShift * 2;

            generateAllObjects();

        }

        public override void Draw(Graphics graphics)
        {
            base.Draw(graphics);       
        }

        public void Add(DisplayObject obj)
        {
            objectsToRender[curPos] = obj;
            curPos++;
        }

        private int[] getRandomSize(int displayWidth, int displayHeight, int basePointX, int basePointY, int halfThickness)
        {
            int maxWidth = Math.Min(displayWidth - basePointX, basePointX - halfThickness);
            int maxHeight = Math.Min(displayHeight - basePointY, basePointY - halfThickness);

            return new int[] { maxWidth, maxHeight };
        }

        private int[] getRandomPos(int displayWidth, int displayHeight, int thickness)
        {
            int xPos = random.Next(screenShift + thickness * 2, displayWidth - thickness * 2 - screenShift);
            int yPos = random.Next(screenShift + thickness * 2, displayHeight - thickness * 2 - screenShift);

            return new int[] { xPos, yPos };
        }

        private Color getRandomColor()
        {
            return Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
        }

        private int[] getRGBColor()
        {
            return new int[] { random.Next(256), random.Next(256), random.Next(256) };
        }

        public void generateRandomCircles(int objectsNum)
        {
            for (int i = 0; i < objectsNum; i++)
            {
                int[] basePos = getRandomPos(gamingFieldWidth, gamingFieldHeight, 5);

                int[] size = getRandomSize(gamingFieldWidth, gamingFieldHeight, basePos[0], basePos[1], 5);

                int framePosX = basePos[0] + size[0];
                int framePosY = basePos[1] + size[1];

                int[] fillColor = getRGBColor();
                int[] borderColor = getRGBColor();

                int borderThickness = random.Next(3, 10);

                Circle circle = new Circle(basePos[0], basePos[1], framePosX, framePosY, fillColor[0], fillColor[1], fillColor[2], borderColor[0], borderColor[1], borderColor[2], borderThickness);
                Add(circle);
            }
        }

        public void generateRandomEllipses(int objectsNum)
        {
            for (int i = 0; i < objectsNum; i++)
            {
                int borderThickness = random.Next(3, 10);

                int[] basePos = getRandomPos(gamingFieldWidth, gamingFieldHeight, borderThickness);

                int[] size = getRandomSize(gamingFieldWidth, gamingFieldHeight, basePos[0], basePos[1], borderThickness);

                int framePosX = basePos[0] + size[0];
                int framePosY = basePos[1] + size[1];

                int[] fillColor = getRGBColor();
                int[] borderColor = getRGBColor();

                Ellipse ellipse = new Ellipse(basePos[0], basePos[1], framePosX, framePosY, fillColor[0], fillColor[1], fillColor[2], borderColor[0], borderColor[1], borderColor[2], borderThickness);
                Add(ellipse);
            }
        }

        public void getRandomPos(DisplayObject obj)
        {
            int[] pos = getRandomPos(gamingFieldWidth, gamingFieldHeight, borderThickness);

            float sizeX = obj.fFrameX - obj.sFrameX;
            float sizeY = obj.fFrameY - obj.sFrameY;

            obj.sFrameX = pos[0];
            obj.fFrameX = obj.sFrameX + sizeX;

            obj.sFrameY = pos[1];
            obj.fFrameY = obj.sFrameY + sizeY;
        }

        public void generateRandomLines(int objectsNum)
        {
            for (int i = 0; i < objectsNum; i++)
            {
                int borderThickness = random.Next(3, 10);

                int[] basePos = getRandomPos(gamingFieldWidth, gamingFieldHeight, borderThickness);

                int[] size = getRandomSize(gamingFieldWidth, gamingFieldHeight, basePos[0], basePos[1], 5);

                int framePosX = basePos[0] + size[0];
                int framePosY = basePos[1] + size[1];

                int[] fillColor = getRGBColor();
                int[] borderColor = getRGBColor();

                Line line = new Line(basePos[0], basePos[1], framePosX, framePosY, fillColor[0], fillColor[1], fillColor[2], borderColor[0], borderColor[1], borderColor[2], borderThickness);
                Add(line);
            }
        }

        public void generateRandomRectangles(int objectsNum)
        {
            for (int i = 0; i < objectsNum; i++)
            {
                int[] basePos = getRandomPos(gamingFieldWidth, gamingFieldHeight, 5);

                int[] size = getRandomSize(gamingFieldWidth, gamingFieldHeight, basePos[0], basePos[1], 5);

                int framePosX = basePos[0] + size[0];
                int framePosY = basePos[1] + size[1];

                
                int[] fillColor = new int[] { 0, 0, 0 };
                int[] borderColor = new int[] { 255, 255, 255 };


                int borderThickness = 3;

                Rectangle rectangle = new Rectangle(basePos[0], basePos[1], framePosX, framePosY, fillColor[0], fillColor[1], fillColor[2], borderColor[0], borderColor[1], borderColor[2], borderThickness);
                Add(rectangle);
            }
        }

        public void generateRandomSquares(int objectsNum)
        {
            for (int i = 0; i < objectsNum; i++)
            {
                
                int[] basePos = getRandomPos(gamingFieldWidth, gamingFieldHeight, 5);

                int[] size = getRandomSize(gamingFieldWidth, gamingFieldHeight, basePos[0], basePos[1], 5);
                
                int framePosX = basePos[0] + size[0];
                int framePosY = basePos[1] + size[1];

                int[] fillColor = getRGBColor();
                int[] borderColor = getRGBColor();

                int borderThickness = random.Next(3, 10);

                Square square = new Square(basePos[0], basePos[1], framePosX, framePosY, fillColor[0], fillColor[1], fillColor[2], borderColor[0], borderColor[1], borderColor[2], borderThickness);
                Add(square);
            }
        }

        public void generateRandomTriangles(int objectsNum)
        {
            for (int i = 0; i < objectsNum; i++)
            {
                int borderThickness = random.Next(3, 10);

                int[] basePos = getRandomPos(gamingFieldWidth, gamingFieldHeight, borderThickness);

                int[] size = getRandomSize(gamingFieldWidth, gamingFieldHeight, basePos[0], basePos[1], borderThickness);

                int framePosX = basePos[0] + size[0];
                int framePosY = basePos[1] + size[1];

                int[] fillColor = getRGBColor();
                int[] borderColor = getRGBColor();

                Triangle triangle = new Triangle(basePos[0], basePos[1], framePosX, framePosY, fillColor[0], fillColor[1], fillColor[2], borderColor[0], borderColor[1], borderColor[2], borderThickness);
                Add(triangle);
            }
        }

        //Функции создания фигуры

        public Rectangle generateRandomRectangle()
        {
            int[] basePos = getRandomPos(gamingFieldWidth, gamingFieldHeight, 5);

            int[] size = getRandomSize(gamingFieldWidth, gamingFieldHeight, basePos[0], basePos[1], 5);

            int framePosX = basePos[0] + size[0];
            int framePosY = basePos[1] + size[1];

            int[] fillColor = getRGBColor();
            int[] borderColor = getRGBColor();

            int borderThickness = random.Next(3, 10);

            Rectangle rectangle = new Rectangle(basePos[0], basePos[1], framePosX, framePosY, fillColor[0], fillColor[1], fillColor[2], borderColor[0], borderColor[1], borderColor[2], borderThickness);
            
            return rectangle;
        }

        public Circle generateRandomCircle()
        {
            int[] basePos = getRandomPos(gamingFieldWidth, gamingFieldHeight, 5);

            int[] size = getRandomSize(gamingFieldWidth, gamingFieldHeight, basePos[0], basePos[1], 5);

            int framePosX = basePos[0] + size[0];
            int framePosY = basePos[1] + size[1];

            int[] fillColor = getRGBColor();
            int[] borderColor = getRGBColor();

            int borderThickness = random.Next(3, 10);

            Circle circle = new Circle(basePos[0], basePos[1], framePosX, framePosY, fillColor[0], fillColor[1], fillColor[2], borderColor[0], borderColor[1], borderColor[2], borderThickness);

            return circle;
        }

        public Line generateRandomLine()
        {
            int[] basePos = getRandomPos(gamingFieldWidth, gamingFieldHeight, 5);

            int[] size = getRandomSize(gamingFieldWidth, gamingFieldHeight, basePos[0], basePos[1], 5);

            int framePosX = basePos[0] + size[0];
            int framePosY = basePos[1] + size[1];

            int[] fillColor = getRGBColor();
            int[] borderColor = getRGBColor();

            int borderThickness = random.Next(3, 10);

            Line line = new Line(basePos[0], basePos[1], framePosX, framePosY, fillColor[0], fillColor[1], fillColor[2], borderColor[0], borderColor[1], borderColor[2], borderThickness);

            return line;
        }

        public Square generateRandomSquare()
        {
            int[] basePos = getRandomPos(gamingFieldWidth, gamingFieldHeight, 5);

            int[] size = getRandomSize(gamingFieldWidth, gamingFieldHeight, basePos[0], basePos[1], 5);

            int framePosX = basePos[0] + size[0];
            int framePosY = basePos[1] + size[1];

            int[] fillColor = getRGBColor();
            int[] borderColor = getRGBColor();

            int borderThickness = random.Next(3, 10);

            Square square = new Square(basePos[0], basePos[1], framePosX, framePosY, fillColor[0], fillColor[1], fillColor[2], borderColor[0], borderColor[1], borderColor[2], borderThickness);

            return square;
        }

        public Triangle generateRandomTriangle()
        {
            int[] basePos = getRandomPos(gamingFieldWidth, gamingFieldHeight, 5);

            int[] size = getRandomSize(gamingFieldWidth, gamingFieldHeight, basePos[0], basePos[1], 5);

            int framePosX = basePos[0] + size[0];
            int framePosY = basePos[1] + size[1];

            int[] fillColor = getRGBColor();
            int[] borderColor = getRGBColor();

            int borderThickness = random.Next(3, 10);

            Triangle triangle = new Triangle(basePos[0], basePos[1], framePosX, framePosY, fillColor[0], fillColor[1], fillColor[2], borderColor[0], borderColor[1], borderColor[2], borderThickness);

            return triangle;
        }

        public Ellipse generateRandomEllipse()
        {
            int[] basePos = getRandomPos(gamingFieldWidth, gamingFieldHeight, 5);

            int[] size = getRandomSize(gamingFieldWidth, gamingFieldHeight, basePos[0], basePos[1], 5);

            int framePosX = basePos[0] + size[0];
            int framePosY = basePos[1] + size[1];

            int[] fillColor = getRGBColor();
            int[] borderColor = getRGBColor();

            int borderThickness = random.Next(3, 10);

            Ellipse ellipse = new Ellipse(basePos[0], basePos[1], framePosX, framePosY, fillColor[0], fillColor[1], fillColor[2], borderColor[0], borderColor[1], borderColor[2], borderThickness);

            return ellipse;
        }


        public virtual void Update(int dx, int dy)
        {
        
        }

        private void generateAllObjects()
        {
            //generateRandomRectangles(1);
            //generateRandomEllipses(1);
            //generateRandomTriangles(1);
            generateRandomCircles(3);
            //generateRandomSquares(1);
            //generateRandomLines(1);
        }


    }
}

