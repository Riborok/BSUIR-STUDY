using System.Data;
using System.Drawing;

namespace lab1.Drawable
{
    public abstract class DisplayObject {
        protected int X1, Y1; // Левая верхняя точка рамки
        protected int X2, Y2; // Правая нижняя точка рамки
        
        protected Color Fill;
        protected int Thickness;
        protected Color Border;
        protected int BaseX, BaseY; // Точка привязки

        protected DisplayObject(
            int baseX, int baseY, 
            int x1, int y1, 
            int x2, int y2, 
            Color fill, int thickness, 
            Color border
        ) {
            BaseX = baseX;
            BaseY = baseY;
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
            Fill = fill;
            Thickness = thickness;
            Border = border;
        }
        
        public int InFrameX1 => X1 + Thickness / 2;
        
        public int InFrameX2 => X2 - Thickness / 2;
        
        public int InFrameY1 => Y1 + Thickness / 2;
        
        public int InFrameY2 => Y2 - Thickness / 2;
        
        public abstract void Draw(Graphics graphics);

        public virtual void Update(int x, int y) {
            int deltaX = x - BaseX;
            int deltaY = y - BaseY;

            BaseX = x;
            BaseY = y;

            X1 += deltaX;
            X2 += deltaX;
            Y1 += deltaY;
            Y2 += deltaY;
        }
    }
}