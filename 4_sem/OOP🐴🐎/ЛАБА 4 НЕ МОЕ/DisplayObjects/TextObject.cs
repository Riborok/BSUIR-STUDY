using oop3.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oop3.DisplayObjects
{
    internal class TextObject:DisplayObject
    {
        public string textData;
        public FontFamily fontFamily;
        public int fontSize;

        public double boxX1, boxY1, boxX2, boxY2;

        public TextObject(string text, FontFamily? textFontFamily, int? textFontSize, Color? fill, int frameX1, int frameY1, int frameX2, int frameY2) : 
            base((frameX1+frameX2)/2, (frameY1 + frameY2) / 2, (frameX2 - frameX1), (frameY2-frameY1), fill)
        {
            textData = text;
            boxX1 = frameX1; boxY1 = frameY1;
            boxX2 = frameX2; boxY2 = frameY2;
            fontFamily = textFontFamily == null ? FontFamily.GenericMonospace : textFontFamily;
            fontSize = textFontSize == null ? 14 : textFontSize.Value;
        }

        protected override void UpdateFrame()
        {
            double[] coordsX = [boxX1, boxX2, boxX1, boxX2];
            double[] coordsY = [boxY1, boxY1, boxY2, boxY2];

            // pass it to the method
            SetFrameFromPoints(anchorX, anchorY, coordsX, coordsY);
        }

        public override void Draw(Graphics g)
        {
            Brush fillBrush = GetFillBrush();

            GraphicsState prevState = MatrixRotate(g);
            RectangleF box = new RectangleF((float)boxX1, (float)boxY1, (float)(boxX2 - boxX1), (float)(boxY2 - boxY1));
            Font font = new Font(fontFamily, fontSize);
            StringFormat drawFormat = new StringFormat();
            drawFormat.Alignment = StringAlignment.Center;
            drawFormat.LineAlignment = StringAlignment.Center;

            g.DrawString(textData, font, fillBrush, box, drawFormat);

            g.Restore(prevState);

            //Brush strokeBrush = GetStrokeBrush();
            //g.DrawRectangle(new Pen(strokeBrush), new Rectangle(frameX1, frameY1, frameX2 - frameX1, frameY2 - frameY1));
        }

        protected override void ShiftCoords(double deltaX, double deltaY)
        {
            boxX1 += deltaX;
            boxY1 += deltaY;
            boxX2 += deltaX;
            boxY2 += deltaY;
        }

    }
}
