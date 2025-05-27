using Figures.Figures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Figures
{
    internal class Game
    {

        private readonly int _movementPeriod;

        Random _random = new Random();

        public GameField _gameField;

        private int sFieldX, sFieldY, fFieldX, fFieldY;

        private const int minV = -3;
        private const int maxV = 3;
        private const int minA = -1;
        private const int maxA = 1;

        public Stopwatch _stopwatch = new Stopwatch();

        public Game(GameField gameField, int fps, int sFieldX, int sFieldY, int fFieldX, int fFieldY, int thickness)
        {
            this.sFieldX = sFieldX;
            this.sFieldY = sFieldY;
            this.fFieldX = fFieldX;
            this.fFieldY = fFieldY;

            _gameField = gameField;

            _movementPeriod = 1000 / fps;

            addObject("square", 100, 100);
            addObject("rectangle", 100, 100);
            addObject("circle", 100, 100);

            setInitParams();

            changeBasePositions();

            //Раскомментировать для теста
            //initObjects();

            _stopwatch.Start();
        }

        public void Play()
        {
            int dt = (int)_stopwatch.ElapsedMilliseconds;

            if (dt >= _movementPeriod)
            {
                checkCollision(dt);

                //_stopwatch.Restart();
            }

            Application.DoEvents();
        }

        private void resetParams(DisplayObject obj)
        {
            obj.v = genRandSpeed();

            obj.a = genRandAcceleration();

            obj.alpha = _random.Next(360);

            obj.t0 = (int)_stopwatch.ElapsedMilliseconds;
        }

        private void checkCollision(int dt)
        {

            int winWidth = fFieldX - sFieldX, winHeight = fFieldY - sFieldY;

            foreach (var obj in _gameField.objectsToRender)
            {
                if (obj.sBaseX < sFieldX + 15 || obj.sBaseX > fFieldX - 15) {
                    _gameField.getRandomPos(obj);
                    resetParams(obj);
                    changeBasePoint(obj, "center");
                }
                else if (obj.sBaseY < sFieldY + 15 || obj.sBaseY > fFieldY - 15)
                {
                    _gameField.getRandomPos(obj);
                    resetParams(obj);
                    changeBasePoint(obj, "center");
                }
                else
                {
                    int curTime = (int)_stopwatch.ElapsedMilliseconds;

                    if (curTime - obj.t0 < 50)
                    {
                        continue;
                    }

                    obj.Move((curTime - obj.t0) / 50);
                    obj.t0 = curTime;
                }
            }

        }

        private void addObject(string type)
        {
            switch (type)
            {
                case "square":
                    // действия для объекта класса Square
                    resize(_gameField.objectsToRender);
                    _gameField.generateRandomSquares(1);
                    break;
                case "triangle":
                    resize(_gameField.objectsToRender);
                    _gameField.generateRandomTriangles(1);
                    break;
                case "circle":
                    resize(_gameField.objectsToRender);
                    _gameField.generateRandomCircles(1);
                    break;
                case "line":
                    resize(_gameField.objectsToRender);
                    _gameField.generateRandomLines(1);
                    break;
                case "rectangle":
                    resize(_gameField.objectsToRender);
                    _gameField.generateRandomRectangles(1);
                    break;
                case "ellipse":
                    resize(_gameField.objectsToRender);
                    _gameField.generateRandomEllipses(1);
                    break;
                default:
                    // действия для других типов объектов
                    break;
            }
        }

        private void removeObject(int index)
        {
            if (index < 0 || index > _gameField.objectsToRender.Length)
            {
                return;
            }

            DisplayObject[] newArray = new DisplayObject[_gameField.objectsToRender.Length - 1];

            // Копируем данные из старого массива в новый, исключая удаленный элемент
            Array.Copy(_gameField.objectsToRender, 0, newArray, 0, index);
            Array.Copy(_gameField.objectsToRender, index + 1, newArray, index, _gameField.objectsToRender.Length - index - 1);

            _gameField.objectsToRender = newArray;
        }

        private void addObject(string type, int x, int y)
        {
            switch (type)
            {
                case "square":
                    resize(_gameField.objectsToRender);
                    _gameField.generateRandomSquares(1);
                    changePos(_gameField.objectsToRender[_gameField.objectsToRender.Length - 1], x, y);
                    break;
                case "triangle":
                    resize(_gameField.objectsToRender);
                    _gameField.generateRandomTriangles(1);
                    changePos(_gameField.objectsToRender[_gameField.objectsToRender.Length - 1], x, y);
                    break;
                case "circle":
                    resize(_gameField.objectsToRender);
                    _gameField.generateRandomCircles(1);
                    changePos(_gameField.objectsToRender[_gameField.objectsToRender.Length - 1], x, y);
                    break;
                case "line":
                    resize(_gameField.objectsToRender);
                    _gameField.generateRandomLines(1);
                    changePos(_gameField.objectsToRender[_gameField.objectsToRender.Length - 1], x, y);
                    break;
                case "rectangle":
                    resize(_gameField.objectsToRender);
                    _gameField.generateRandomRectangles(1);
                    changePos(_gameField.objectsToRender[_gameField.objectsToRender.Length - 1], x, y);
                    break;
                case "ellipse":
                    resize(_gameField.objectsToRender);
                    _gameField.generateRandomEllipses(1);
                    changePos(_gameField.objectsToRender[_gameField.objectsToRender.Length - 1], x, y);
                    break;
                default:
                    break;
            }
        }

        private void changePos(DisplayObject obj, int xPos, int yPos)
        {
            int width = (int)(obj.fFrameX - obj.sFrameX);
            int height = (int)(obj.fFrameY - obj.sFrameY);

            obj.sFrameX = xPos;
            obj.sFrameY = yPos;

            obj.fFrameX = xPos + width; 
            obj.fFrameY = yPos + height;
        }

        public void initObjects()
        {

            int winWidth = fFieldX - sFieldX, winHeight = fFieldY - sFieldY;

            foreach (var obj in _gameField.objectsToRender)
            {
                obj.sFrameX = 100;
                obj.fFrameX = 200;

                obj.sFrameY = 100;
                obj.fFrameY = 200;

                obj.sBaseX = 150;
                obj.sBaseY = 150;

                //obj.v = genRandSpeed();

                obj.alpha = _random.Next(180);

                //obj.t0 = (int)_stopwatch.ElapsedMilliseconds;
                    
                //obj.aY = genRandAcceleration(); 
                //obj.aX = genRandAcceleration();
            }

            _gameField.objectsToRender[0].v = 1;
            _gameField.objectsToRender[1].v = 1;

            _gameField.objectsToRender[0].alpha = 45;
            _gameField.objectsToRender[1].alpha = 0;

            _gameField.objectsToRender[0].a = 0;
            _gameField.objectsToRender[1].a = 0;

        }

        private void setInitParams()
        {
            foreach (var obj in _gameField.objectsToRender)
            {
                resetParams(obj);
            }
        }

        private void changeBasePositions()
        {
            foreach (var obj in _gameField.objectsToRender)
            {
                changeBasePoint(obj, "center");
            }
        }

        public void changeBasePoint(DisplayObject obj, string position)
        {

            switch (position)
            {
                case "center":
                    int width = (int)(obj.fFrameX - obj.sFrameX);
                    int height = (int)(obj.fFrameY - obj.sFrameY);

                    obj.sBaseX = obj.sFrameX + width / 2;
                    obj.sBaseY = obj.sFrameY + height / 2;
                    break;
                case "up_left":
                    obj.sBaseX = obj.sFrameX;
                    obj.sBaseY = obj.sFrameY;
                    break;
                case "up_right":
                    width = (int)(obj.fFrameX - obj.sFrameX);

                    obj.sBaseX = obj.sFrameX + width;
                    obj.sBaseY = obj.sFrameY;
                    break;
                case "down_left":
                    height = (int)(obj.fFrameY - obj.sFrameY);

                    obj.sBaseX = obj.sFrameX;
                    obj.sBaseY = obj.sFrameY + height;
                    break;
                case "down_right":
                    width = (int)(obj.fFrameX - obj.sFrameX);
                    height = (int)(obj.fFrameY - obj.sFrameY);

                    obj.sBaseX = obj.sFrameX + width;
                    obj.sBaseY = obj.sFrameY + height;
                    break;
            }
        }

        private void resize(DisplayObject[] arr) {
            // Создаем новый массив с увеличенным размером на 1
            DisplayObject[] newArray = new DisplayObject[arr.Length + 1];

            // Копируем данные из старого массива в новый
            Array.Copy(arr, newArray, arr.Length);

            _gameField.objectsToRender = newArray;
        }

        public int genRandSpeed()
        {
            int speed = _random.Next(minV, maxV);

            return speed == 0 ? 2 : speed;
        }

        public int genRandAcceleration()
        {
            return _random.Next(minA, maxA);
        }

    }
}

