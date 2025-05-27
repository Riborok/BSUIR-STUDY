using System;
using System.Collections.Generic;
using System.Drawing;

namespace lab1.Drawable
{
	public class GameField : DisplayObject {
		private DisplayObject[] _displayObjects = new DisplayObject[60];
		private int _p = 0;
		
		public GameField(int baseX, int baseY, int x, int y, int width, int height, Color fill, int thickness, Color border)
			: base(baseX, baseY, x, y, x + width, y + height, fill, thickness, border)
		{
			CreateRandomLines(10);
			CreateRandomRectangles(10);
			CreateRandomSquares(10);
			CreateRandomTriangles(10);
			CreateRandomEllipse(10);
			CreateRandomCircle(10);
		}

		public void Add(DisplayObject o) {
			_displayObjects[_p++] = o;
		}
		
		public void Delete(DisplayObject o) {
			int indexToRemove = Array.IndexOf(_displayObjects, o);
			for (int i = indexToRemove; i < _displayObjects.Length - 1; i++)
				_displayObjects[i] = _displayObjects[i + 1];
			_p--;
		}
		
		public override void Draw(Graphics graphics)
		{
			using (var brush = new SolidBrush(Fill))
			using (var pen = new Pen(Border, Thickness))
			{
				int width = X2 - X1;
				int height = Y2 - Y1;
				graphics.FillRectangle(brush, X1, Y1, width, height);
				graphics.DrawRectangle(pen, X1, Y1, width, height);
			}

			for (int i = 0; i < _displayObjects.Length; i++) 
				_displayObjects[i].Draw(graphics);
		}
		
		private void CreateRandomLines(int count)
        {
            for (int i = 0; i < count; i++)
            {
                int thickness = GenerateThickness();
                (int x, int y) = GeneratePoint(thickness);
                (int width, int height) = GenerateSize(x, y, thickness);
                
                var displayObject = new Line(
                    x, y,
                    x, y,
                    x + width, y + height,
                    GenerateRandomColor(), thickness, GenerateRandomColor()
                );
                _displayObjects[_p++] = displayObject;
            }
        }

        private void CreateRandomRectangles(int count)
        {
            for (int i = 0; i < count; i++)
            {
                int thickness = GenerateThickness();
                (int x, int y) = GeneratePoint(thickness);
                (int width, int height) = GenerateSize(x, y, thickness);

                var displayObject = new Rectangle(
                    x, y,
                    x, y,
                    width, height,
                    GenerateRandomColor(), thickness, GenerateRandomColor()
                );
                _displayObjects[_p++] = displayObject;
            }
        }
        
        private void CreateRandomSquares(int count)
        {
            for (int i = 0; i < count; i++)
            {
                int thickness = GenerateThickness();
                (int x, int y) = GeneratePoint(thickness);
                (int width, int height) = GenerateSize(x, y, thickness);

                var displayObject = new Square(
                    x, y,
                    x, y,
                    Math.Min(width, height),
                    GenerateRandomColor(), thickness, GenerateRandomColor()
                );
                _displayObjects[_p++] = displayObject;
            }
        }
        
        private void CreateRandomTriangles(int count)
        {
            for (int i = 0; i < count; i++)
            {
                int thickness = GenerateThickness();
                (int x1, int y1) = GeneratePoint(thickness, Indent * 5);
                (int x2, int y2) = GeneratePoint(thickness, Indent * 5);
                (int x3, int y3) = GeneratePoint(thickness, Indent * 5);

                var displayObject = new Triangle(
                    Math.Min(Math.Min(x1, x2), x3),
                    Math.Min(Math.Min(y1, y2), y3),
                    x1, y1, x2, y2, x3, y3,
                    GenerateRandomColor(), thickness, GenerateRandomColor()
                );
                _displayObjects[_p++] = displayObject;
            }
        }
        
        private void CreateRandomEllipse(int count)
        {
            for (int i = 0; i < count; i++)
            {
                int thickness = GenerateThickness();
                (int x, int y) = GeneratePoint(thickness);
                (int width, int height) = GenerateSize(x, y, thickness);                                                                                      

                var displayObject = new Ellipse(
                    x, y,
                    x, y,
                    width, height,
                    GenerateRandomColor(), thickness, GenerateRandomColor()
                );
                _displayObjects[_p++] = displayObject;
            }
        }
        
        private void CreateRandomCircle(int count)
        {
            for (int i = 0; i < count; i++)
            {                
                int thickness = GenerateThickness();
                (int x, int y) = GeneratePoint(thickness);
                (int width, int height) = GenerateSize(x, y, thickness);

                var displayObject = new Circle(
                    x, y,
                    x, y,
                    Math.Min(width, height),
                    GenerateRandomColor(), thickness, GenerateRandomColor()
                );
                _displayObjects[_p++] = displayObject;
            }
        }

        private (int x, int y) GeneratePoint(int outerThickness, int indent = Indent) {
            return (
                R.Next(InFrameX1 + outerThickness + indent, InFrameX2 - outerThickness - indent),
                R.Next(InFrameY1 + outerThickness + indent, InFrameY2 - outerThickness - indent)
            );
        }
        
        private static int GenerateThickness() => R.Next(MinThickness, MaxThickness);
        
        private (int width, int height) GenerateSize(int x, int y, int outerThickness) {
            return (R.Next(GetMaxWidth(x, outerThickness)), R.Next(GetMaxHeight(y, outerThickness)));
        }

        private int GetMaxWidth(int x, int outerThickness) {
            return Math.Min(x - InFrameX1, InFrameX2 - x) - outerThickness - Indent;
        } 
        
        private int GetMaxHeight(int y, int outerThickness) {
            return Math.Min(y - InFrameY1, InFrameY2 - y) - outerThickness - Indent;
        }

        private static Color GenerateRandomColor() {
            return Color.FromArgb(R.Next(256), R.Next(256), R.Next(256));
        }
        
        private const int MinThickness = 1;
        private const int MaxThickness = 10;
        private const int Indent = 5;
        private static readonly Random R = new Random();
	}

}