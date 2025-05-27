namespace oop3.DisplayObjects
{
    internal class SquareObject : RectangleObject
    {

        // Constructor for square with fill color
        public SquareObject(int topLeftX, int topLeftY, int size, Color? fill = null) :
            base(topLeftX, topLeftY, topLeftX + size, topLeftY + size, fill)
        { }

        // Constructor for rectangle with texture filling
        public SquareObject(int topLeftX, int topLeftY, int size, Bitmap bmp) :
            base(topLeftX, topLeftY, topLeftX + size, topLeftY + size, bmp)
        { }
    }
}