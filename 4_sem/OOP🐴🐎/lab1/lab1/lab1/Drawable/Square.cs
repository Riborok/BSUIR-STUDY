using System.Drawing;

namespace lab1.Drawable
{
    public class Square : Rectangle
    {
        public Square(int baseX, int baseY, int x, int y, int size, Color fill, int thickness, Color border)
            : base(baseX, baseY, x, y, size, size, fill, thickness, border)
        {
        }
    }
}