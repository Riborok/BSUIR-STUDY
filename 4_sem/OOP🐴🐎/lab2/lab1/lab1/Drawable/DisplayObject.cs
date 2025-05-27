using System;
using System.Drawing;
// ReSharper disable InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace lab1.Drawable
{
    public abstract class DisplayObject {
        protected Color _fill;
        protected int _thickness;
        protected Color _border;
        
        protected int _clientX1, _clientY1; // Левая верхняя точка клиентской части
        protected int _clientX2, _clientY2; // Правая нижняя точка клиентской части
        protected int _externalX1, _externalY1; // Левая верхняя точка внешней части
        protected int _externalX2, _externalY2; // Правая нижняя точка внешней части
        
        protected int _baseX, _baseY; // Точка привязки
        public long t;
        public double vLength, aLength;
        public int angle;
        
        protected DisplayObject(
            int baseX, int baseY, 
            int x1, int y1, 
            int x2, int y2, 
            Color fill, int thickness, 
            Color border
        ) {
            _baseX = baseX;
            _baseY = baseY;
            _fill = fill;
            _thickness = thickness;
            _border = border;

            int halfThickness = _thickness / 2;
            _clientX1 = x1 + halfThickness;
            _clientX2 = x2 - halfThickness;
            _clientY1 = y1 + halfThickness;
            _clientY2 = y2 - halfThickness;
            
            _externalX1 = x1 - halfThickness;
            _externalX2 = x2 + halfThickness;
            _externalY1 = y1 - halfThickness;
            _externalY2 = y2 + halfThickness;
        }
        
        public int ClientX1 => _clientX1;
        
        public int ClientX2 => _clientX2;
        
        public int ClientY1 => _clientY1;
        
        public int ClientY2 => _clientY2;
        
        public abstract void Draw(Graphics graphics);

        protected double dx, dy;
        public void Move(long currT) {
            var dt = currT - t;

            var (vx, vy) = CalcVelocity();
            var (ax, ay) = CalcAcceleration();
            vLength += aLength * dt;
            
            dx += dt * (vx + ax * dt / 2);
            dy += dt * (vy + ay * dt / 2);

            Update((int)dx, (int)dy);
            modifyDelta();
            t = currT;
        }
        
        private (double vx, double vy) CalcVelocity() {
            var radians = angle * Math.PI / 180;
            var vx = vLength * Math.Cos(radians);
            var vy = vLength * Math.Sin(radians);
            return (vx, vy);
        }

        private (double ax, double ay) CalcAcceleration() {
            var radians = angle * Math.PI / 180;
            var ax = aLength * Math.Cos(radians);
            var ay = aLength * Math.Sin(radians);
            return (ax, ay);
        }

        private void modifyDelta() {
            dx -= (int)dx;
            dy -= (int)dy;
        }

        public virtual void Update(int dx, int dy) {
            _baseX += dx;
            _clientX1 += dx;
            _clientX2 += dx;
            _externalX1 += dx;
            _externalX2 += dx;
            
            _baseY += dy;
            _clientY1 += dy;
            _clientY2 += dy;
            _externalY1 += dy;
            _externalY2 += dy;
        }
        
        public bool IsContains(DisplayObject displayObject) {
            return displayObject._externalX1 >= _clientX1 && displayObject._externalY1 >= _clientY1 &&
                   displayObject._externalX2 <= _clientX2 && displayObject._externalY2 <= _clientY2;
        }
        
        public bool IsContainsBasePointOf(DisplayObject displayObject) {
            int x = displayObject._baseX;
            int y = displayObject._baseY;
            return x >= _clientX1 && x <= _clientX2 && y >= _clientY1 && y <= _clientY2;
        }
    }
}