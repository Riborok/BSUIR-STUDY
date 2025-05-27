using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using lab1.Drawable;
using Rectangle = lab1.Drawable.Rectangle;

namespace lab1 {
	public class Game {
		public delegate void RedrawRequestHandler();
		
		private readonly PictureBox _pictureBox;
		private readonly int _movementPeriod;
		private int _i = 0;
		private GameField _gameField;
		private Timer _timer;

		public bool IsContinue;
		
		public Game(PictureBox pictureBox, int fps) {
			IsContinue = true;
			
			_pictureBox = pictureBox;
			_movementPeriod = 1000 / fps;
			
			CreateGameField();
			
			_timer = new Timer();
			_timer.Interval = _movementPeriod;
			_timer.Tick += (object sender, EventArgs e) => { _pictureBox.Invalidate(); };
			_timer.Start();
		}
		
		public void CreateGameField() {
			const int thickness = 30;
			Color fill = Color.White;
			Color borderColor = Color.GreenYellow;

			const int x = 0;
			const int y = 0;
			_gameField = new GameField(x, y, x, y, 
				_pictureBox.Width, _pictureBox.Height, fill, thickness, borderColor);
			/*CreateRandomLines(10);
			CreateRandomRectangles(10);
			CreateRandomSquares(10);
			CreateRandomTriangles(10);
			CreateRandomEllipse(10);
			CreateRandomCircle(8);*/
			CreateTestCircles();
		}

		public void Play(Graphics g) {
			if (IsContinue)
				MoveAll();
			_gameField.Draw(g);
		}

		private void MoveAll() {
			_gameField.Move(++_i * _movementPeriod);
			for (int i = 0; i < _gameField._p; i++) {
				DisplayObject displayObject = _gameField._displayObjects[i];
				if (!_gameField.IsContainsBasePointOf(displayObject)) {
					_gameField.Delete(displayObject);
					_gameField.Add(Recreate(displayObject));
				}
			}
		}
		
		private DisplayObject Recreate(DisplayObject displayObject) {
			switch (displayObject) {
				case Line _:
					return CreateRandomLine();
				case Square _:
					return CreateRandomSquare();
				case Rectangle _:
					return CreateRandomRectangle();
				case Triangle _:
					return CreateRandomTriangle();
				case Circle _:
					return CreateRandomCircle();
				case Ellipse _:
					return CreateRandomEllipse();
			}
			return null;
		}
		
		private void CreateRandomLines(int count) {
            for (int i = 0; i < count; i++)
	            _gameField.Add(CreateRandomLine());
        }
		
		private Line CreateRandomLine() {
			int thickness = GenerateThickness();
			(int x, int y) = GeneratePoint(thickness);
			(int width, int height) = GenerateSize(x, y, thickness);
                
			var res = new Line(
				GetBaseX(x, width), GetBaseY(y, height),
				x, y,
				x + width, y + height,
				GenerateRandomColor(), thickness, GenerateRandomColor()
			);
			SetSpeed(res);
			return res;
		}

        private void CreateRandomRectangles(int count) {
            for (int i = 0; i < count; i++)
	            _gameField.Add(CreateRandomRectangle());
        }
        
        private Rectangle CreateRandomRectangle() {
	        int thickness = GenerateThickness();
	        (int x, int y) = GeneratePoint(thickness);
	        (int width, int height) = GenerateSize(x, y, thickness);

	        var res = new Rectangle(
		        GetBaseX(x, width), GetBaseY(y, height),
		        x, y,
		        width, height,
		        GenerateRandomColor(), thickness, GenerateRandomColor()
	        );
	        SetSpeed(res);
	        return res;
        }
        
        private void CreateRandomSquares(int count)
        {
            for (int i = 0; i < count; i++)
	            _gameField.Add(CreateRandomSquare());
        }
        
        private Square CreateRandomSquare() {
	        int thickness = GenerateThickness();
	        (int x, int y) = GeneratePoint(thickness);
	        (int width, int height) = GenerateSize(x, y, thickness);
	        int size = Math.Min(width, height);
                
	        var res = new Square(
		        GetBaseX(x, size), GetBaseY(y, size),
		        x, y,
		        size,
		        GenerateRandomColor(), thickness, GenerateRandomColor()
	        );
	        SetSpeed(res);
	        return res;
        }
        
        private void CreateRandomTriangles(int count)
        {
            for (int i = 0; i < count; i++)
	            _gameField.Add(CreateRandomTriangle());
        }
        
        private Triangle CreateRandomTriangle() {
	        int thickness = GenerateThickness();
	        (int x1, int y1) = GeneratePoint(thickness, Indent * 20);
	        (int x2, int y2) = GeneratePoint(thickness, Indent * 20);
	        (int x3, int y3) = GeneratePoint(thickness, Indent * 20);

	        var res = new Triangle(
		        GetBaseX(x1, x2, x3),
		        GetBaseY(y1, y2, y3),
		        x1, y1, x2, y2, x3, y3,
		        GenerateRandomColor(), thickness, GenerateRandomColor()
	        );
	        SetSpeed(res);
	        return res;
        }

        private void CreateRandomEllipse(int count) {
            for (int i = 0; i < count; i++)
	            _gameField.Add(CreateRandomEllipse());
        }
        
        private Ellipse CreateRandomEllipse() {
	        int thickness = GenerateThickness();
	        (int x, int y) = GeneratePoint(thickness);
	        (int width, int height) = GenerateSize(x, y, thickness);                                                                                      

	        var res = new Ellipse(
		        GetBaseX(x, width), GetBaseY(y, height),
		        x, y,
		        width, height,
		        GenerateRandomColor(), thickness, GenerateRandomColor()
	        );
	        SetSpeed(res);
	        return res;
        }
        
        private void CreateRandomCircle(int count) {
            for (int i = 0; i < count; i++)
	            _gameField.Add(CreateRandomCircle());
        }

        private void CreateTestCircles() {
	        _gameField.Add(CreateTestCircle(0, 0.06));
	        _gameField.Add(CreateTestCircle(90 * 3 + 45, 0.06));
	        _gameField.Add(CreateTestCircle(0, 0));
        }

        private Circle CreateTestCircle(int angle, double vlength) {
	        int thickness = 4;
	        int size = 60;
	        (int x, int y) = (_gameField.ClientX1 + thickness / 2, _gameField.ClientY2 - size - thickness / 2);

	        var res = new Circle(
		        GetBaseX(x, size), GetBaseY(y, size),
		        x, y,
		        size,
		        GenerateRandomColor(), thickness, GenerateRandomColor()
	        );
	        res.t = _i * _movementPeriod;
	        res.vLength = vlength;
	        res.angle = angle;
	        return res;
        }
        
        private Circle CreateRandomCircle() {
	        int thickness = GenerateThickness();
	        (int x, int y) = GeneratePoint(thickness);
	        (int width, int height) = GenerateSize(x, y, thickness);
	        int size = Math.Min(width, height);
                
	        var res = new Circle(
		        GetBaseX(x, size), GetBaseY(y, size),
		        x, y,
		        size,
		        GenerateRandomColor(), thickness, GenerateRandomColor()
	        );
	        SetSpeed(res);
	        return res;
        }
        
        private static int GetBaseX(int x1, int x2, int x3) {
	        int center1 = (x1 + x2) / 2;
	        int center2 = (x2 + x3) / 2;
	        int center = (center1 + center2) / 2;
	        return center;
        }
        
        private static int GetBaseY(int x1, int x2, int x3) {
	        int center1 = (x1 + x2) / 2;
	        int center2 = (x2 + x3) / 2;
	        int center = (center1 + center2) / 2;
	        return center;
        }
        
        private static int GetBaseX(int x, int width) => x + width / 2; 
        
        private static int GetBaseY(int y, int height) => y + height / 2; 

        private (int x, int y) GeneratePoint(int outerThickness, int indent = Indent) {
            return (
                R.Next(_gameField.ClientX1 + outerThickness + indent, _gameField.ClientX2 - outerThickness - indent),
                R.Next(_gameField.ClientY1 + outerThickness + indent, _gameField.ClientY2 - outerThickness - indent)
            );
        }
        
        private static int GenerateThickness() => R.Next(MinThickness, MaxThickness);
        
        private (int width, int height) GenerateSize(int x, int y, int outerThickness) {
            return (R.Next(GetMaxWidth(x, outerThickness)), R.Next(GetMaxHeight(y, outerThickness)));
        }

        private int GetMaxWidth(int x, int outerThickness) {
            return (Math.Min(x - _gameField.ClientX1, _gameField.ClientX2 - x) - outerThickness - Indent) / 2;
        } 
        
        private int GetMaxHeight(int y, int outerThickness) {
            return (Math.Min(y - _gameField.ClientY1, _gameField.ClientY2 - y) - outerThickness - Indent) / 2;
        }

        private static Color GenerateRandomColor() {
            return Color.FromArgb(R.Next(256), R.Next(256), R.Next(256));
        }

        private void SetSpeed(DisplayObject displayObject) {
	        displayObject.t = _i * _movementPeriod;
	        displayObject.aLength = GenerateALength() / 1000.0;
	        displayObject.vLength = GenerateVLength() / 100.0;
	        displayObject.angle = R.Next(360 + 1);
        }
        
        private static int GenerateALength() => R.Next(10) == 0 ? 0 : R.Next(10);
        
        private static int GenerateVLength() => R.Next(1, 100);
        
        private const int MinThickness = 1;
        private const int MaxThickness = 10;
        private const int Indent = 5;
        private static readonly Random R = new Random();
	}
}