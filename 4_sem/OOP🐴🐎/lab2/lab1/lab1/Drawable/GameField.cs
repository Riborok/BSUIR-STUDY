using System;
using System.Drawing;

namespace lab1.Drawable
{
	public class GameField : Rectangle {
		public DisplayObject[] _displayObjects = new DisplayObject[60];
		public int _p = 0;
		
		public GameField(int baseX, int baseY, int x, int y, int width, int height, Color fill, int thickness, Color border)
			: base(baseX, baseY, x, y, width, height, fill, thickness, border)
		{
		}

		public void Add(DisplayObject o) {
			_displayObjects[_p++] = o;
		}
		
		public void Delete(DisplayObject o) {
			int indexToRemove = Array.IndexOf(_displayObjects, o);
			if (indexToRemove < 0) 
				return; 

			Array.Copy(_displayObjects, indexToRemove + 1, _displayObjects, 
				indexToRemove, _displayObjects.Length - indexToRemove - 1);
			_displayObjects[_displayObjects.Length - 1] = null;
			_p--;
		}
		
		public override void Draw(Graphics graphics) {
			base.Draw(graphics);
			for (int i = 0; i < _p; i++) 
				_displayObjects[i].Draw(graphics);
		}

		public void Move(long t) {
			for (int i = 0; i < _p; i++) {
				DisplayObject displayObject = _displayObjects[i];
				displayObject.Move(t);
			}
		}
	}
}