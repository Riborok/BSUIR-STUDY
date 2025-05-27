using Figures.Figures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Figures.Menu
{
    public class MenuItems : DisplayObject
    {
        public string text;

        public int fontSize;

        public string textStyle = "Arial";

        public int x;
        public int y;

        public int tR, tG, tB;

        public DisplayObject frameObj;

        public int index, parentIndex = 0;
        public int submenuCount = 0;

        public MenuItems[] subMenuItems = new MenuItems[0];

        public bool isParent = true;

        public int curShift = 70, curShiftX = 0, curShiftY = 0;

        public bool isVisible = true;

        public GameField _gameField;

        public void Clone(MenuItems other)
        {
            other.text = text;
            other.fontSize = fontSize;
            other.textStyle = textStyle;
            other.tR = tR;
            other.tG = tG;
            other.tB = tB;
        }

        public MenuItems(int x, int y, int fontSize, string text, string textStyle, int tR, int tG, int tB, DisplayObject frameObj, int index, GameField gameField)
        {
            this.frameObj = frameObj;

            this.x = x;
            this.y = y;

            this.text = text;
            this.tR = tR;
            this.tG = tG;
            this.tB = tB;

            this.index = index;

            this.fontSize = fontSize;

            _gameField = gameField;
        }

        public override void Draw(Graphics graphics)
        {
            frameObj.Draw(graphics);
        }

        public override void DrawText(Graphics graphics)
        {
            using (Font myFont = new Font(textStyle, fontSize))
            {
                Brush brush = new SolidBrush(Color.FromArgb(tR, tG, tB));
                graphics.DrawString(text, myFont, brush, new PointF(centerTextX(), centerTextY()));
            }
        }

        private int centerTextX()
        {
            if (frameObj is Triangle || frameObj is Circle || frameObj is Square)
            {
                return (int)(frameObj.sFrameX + (frameObj.fFrameX - frameObj.sFrameX) / 2 - text.Length * fontSize / 2);    
            }

            return (int)(frameObj.sFrameX + (frameObj.fFrameX - frameObj.sFrameX) / 2 - text.Length * fontSize / 3);
        }

        private int centerTextY()
        {
            return (int)(frameObj.sFrameY + (frameObj.fFrameY - frameObj.sFrameY) / 2 - fontSize);
        }

        public void changeCoords(int x, int y, int dx, int dy)
        {
            x += _gameField.borderThickness / 2 + 3;
            y += _gameField.borderThickness / 2 + 3 ;
            dx += _gameField.borderThickness / 2+ 3 ;
            dy += _gameField.borderThickness / 2 + 3;
            
            frameObj.sFrameX = x; frameObj.sFrameY = y;
            frameObj.fFrameX = x + (frameObj.fFrameX - frameObj.sFrameX);
            frameObj.fFrameY = y + (frameObj.fFrameY - frameObj.sFrameY);

            frameObj.sFrameX += dx;
            frameObj.sFrameY += dy;

            frameObj.fFrameX += dx;
            frameObj.fFrameY += dy;

            curShiftX = (int)frameObj.fFrameX;
            curShiftY = (int)frameObj.fFrameY;
        }
        
        public void changeText(string text, string textType, int fontWidth)
        {
            this.textStyle = textType;
            this.fontSize = fontWidth;
            this.text = text;
            int frWidth = (int)(this.frameObj.fFrameX - frameObj.sFrameX);
            int frHeight = (int)(this.frameObj.fFrameY - frameObj.sFrameY);

            var (width, height) = CalculateTextSize();
            width += width / 3 + 10;
            height += height / 3 + 10;

            if (frWidth < width)
                resize(width, frHeight);

            if (frHeight < height) 
                resize(frWidth, height);
        }
        
        public (int Width, int Height) CalculateTextSize()
        {
            using (var bitmap = new Bitmap(1, 1))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    var font = new Font(textStyle, fontSize);
                    var size = graphics.MeasureString(text, font);
                    return ((int)size.Width, (int)size.Height);
                }
            }
        }

        public void resize(int width, int height)
        {
            frameObj.fFrameX = frameObj.sFrameX + width;
            frameObj.fFrameY = frameObj.sFrameY + height;
        }

        public void changeBorderSize(int size)
        {
            frameObj.borderThickness = size;
        }

        public void changeFillColor(int fR, int fG, int fB)
        {
            frameObj.fR = fR;
            frameObj.fG = fG;
            frameObj.fB = fB;
        }

        public void changeBorderColor(int bR, int  bG, int bB)
        {
            frameObj.bR = bR;
            frameObj.bG = bG;
            frameObj.bB = bB;
        }

        public void changeTextColor(int tR,  int tG, int tB)
        {
            this.tR = tR;
            this.tG = tG;
            this.tB = tB;
        }

        public void changeObject(string objType)
        {
            if (objType == "rectangle")
            {

                Figures.Rectangle obj = _gameField.generateRandomRectangle();

                obj.fR = frameObj.fR;
                obj.fB = frameObj.fB;
                obj.fG = frameObj.fG;

                obj.bR = frameObj.bR;
                obj.bB = frameObj.bB;
                obj.bG = frameObj.bG;

                obj.sFrameX = frameObj.sFrameX;
                obj.sFrameY = frameObj.sFrameY;

                obj.fFrameX = frameObj.fFrameX;
                obj.fFrameY = frameObj.fFrameY;

                frameObj = obj;

            }
            else if (objType == "circle")
            {

                Circle obj = _gameField.generateRandomCircle();

                obj.fR = frameObj.fR;
                obj.fB = frameObj.fB;
                obj.fG = frameObj.fG;

                obj.bR = frameObj.bR;
                obj.bB = frameObj.bB;
                obj.bG = frameObj.bG;

                obj.sFrameX = frameObj.sFrameX;
                obj.sFrameY = frameObj.sFrameY;

                obj.fFrameX = frameObj.fFrameX;
                obj.fFrameY = frameObj.fFrameY;

                frameObj = obj;

            }
            else if (objType == "triangle")
            {

                Triangle obj = _gameField.generateRandomTriangle();

                obj.fR = frameObj.fR;
                obj.fB = frameObj.fB;
                obj.fG = frameObj.fG;

                obj.bR = frameObj.bR;
                obj.bB = frameObj.bB;
                obj.bG = frameObj.bG;

                obj.sFrameX = frameObj.sFrameX;
                obj.sFrameY = frameObj.sFrameY;

                obj.fFrameX = frameObj.fFrameX;
                obj.fFrameY = frameObj.fFrameY;

                frameObj = obj;

            }
            else if (objType == "square")
            {

                Square obj = _gameField.generateRandomSquare();

                obj.fR = frameObj.fR;
                obj.fB = frameObj.fB;
                obj.fG = frameObj.fG;

                obj.bR = frameObj.bR;
                obj.bB = frameObj.bB;
                obj.bG = frameObj.bG;

                obj.sFrameX = frameObj.sFrameX;
                obj.sFrameY = frameObj.sFrameY;

                obj.fFrameX = frameObj.fFrameX;
                obj.fFrameY = frameObj.fFrameY;

                frameObj = obj;

            }
            else if (objType == "line")
            {

                Line obj = _gameField.generateRandomLine();

                obj.fR = frameObj.fR;
                obj.fB = frameObj.fB;
                obj.fG = frameObj.fG;

                obj.bR = frameObj.bR;
                obj.bB = frameObj.bB;
                obj.bG = frameObj.bG;

                obj.sFrameX = frameObj.sFrameX;
                obj.sFrameY = frameObj.sFrameY;

                obj.fFrameX = frameObj.fFrameX;
                obj.fFrameY = frameObj.fFrameY;

                frameObj = obj;

            }
            else if (objType == "ellipse")
            {

                Ellipse obj = _gameField.generateRandomEllipse();

                obj.fR = frameObj.fR;
                obj.fB = frameObj.fB;
                obj.fG = frameObj.fG;

                obj.bR = frameObj.bR;
                obj.bB = frameObj.bB;
                obj.bG = frameObj.bG;

                obj.sFrameX = frameObj.sFrameX;
                obj.sFrameY = frameObj.sFrameY;

                obj.fFrameX = frameObj.fFrameX;
                obj.fFrameY = frameObj.fFrameY;

                frameObj = obj;

            }
        }

        public bool isAbsolute = false;

    }
}
