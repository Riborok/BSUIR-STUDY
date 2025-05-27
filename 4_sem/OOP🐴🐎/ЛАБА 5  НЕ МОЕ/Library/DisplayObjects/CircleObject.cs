using System;
using System.Drawing;

namespace oop3.DisplayObjects
{
    public class CircleObject : EllipseObject
    {
        int mass;
        
        public CircleObject(int centerX, int centerY, int radius, Color? fill = null) :
            base(centerX, centerY, radius, radius, fill)
        {
            mass = radius * radius;
        }

        public CircleObject(int centerX, int centerY, int radius, Bitmap bmp) :
            base(centerX, centerY, radius, radius, bmp)
        { 
            mass = radius * radius;
        }


        public double GetCTForBorders(double borX1, double borY1, double borX2, double borY2) {
            double timeX = -1;
            double timeY = -1;

            double vx = velModulo * Math.Cos(velAlpha * Math.PI / 180);
            if (vx != 0) { 
                double distX = vx < 0 ? (borX1 - frameX1) : (borX2 - frameX2);
                timeX = distX / vx;
            }

            double vy = velModulo * Math.Sin(velAlpha * Math.PI / 180);
            if (vy != 0) { 
                double distY = vy == 0 ? -1 : (vy < 0 ? (borY1 - frameY1) : (borY2 - frameY2));
                timeY = distY / vy;
            }

            if (timeX * timeY > 0)
            {
                if (timeX < 0)
                {
                    return timeY;       // both are -1 or so; the obj isn't moving
                }
                else
                {
                    return timeX < timeY ? timeX : timeY;
                }
            }
            else { 
                return timeX < timeY ? timeY : timeX;     
            }
            
        }

        public double GetCTForCircle(CircleObject obj) {
            double vx1 = velModulo * Math.Cos(velAlpha * Math.PI / 180);
            double vy1 = velModulo * Math.Sin(velAlpha * Math.PI / 180);
            double vx2 = obj.velModulo * Math.Cos(obj.velAlpha * Math.PI / 180);
            double vy2 = obj.velModulo * Math.Sin(obj.velAlpha * Math.PI / 180);

            double x1 = centerX;
            double y1 = centerY;
            double r1 = radiusX;

            double x2 = obj.centerX;
            double y2 = obj.centerY;
            double r2 = obj.radiusX;
            double vrelx = vx2 - vx1;
            double vrely = vy2 - vy1;

            double xrel = x2 - x1;
            double yrel = y2 - y1;

            // Quadratic coefficients
            double a = vrelx * vrelx + vrely * vrely;
            double b = 2 * (xrel * vrelx + yrel * vrely);
            double c = xrel * xrel + yrel * yrel - (r1 + r2) * (r1 + r2);

            // Check for collision
            double discriminant = b * b - 4 * a * c;

            if (discriminant < 0)
            {
                // No real roots, no collision
                return -1;
            }
            else
            {
                double t1 = (-b - Math.Sqrt(discriminant)) / (2 * a);
                double t2 = (-b + Math.Sqrt(discriminant)) / (2 * a);
                
                double result = -1;
                if (t1 * t2 > 0)
                {
                    if (t1 < 0)
                    {
                        result = t1;    
                    }
                    else
                    {
                        result = t1 < t2 ? t1 : t2;
                    }
                }
                else
                {
                    result = t1 < t2 ? t2 : t1;     
                }
                return result;
            }
        }

        public bool CheckCircleCollision(CircleObject other) {
            double dx = this.centerX - other.centerX;
            double dy = this.centerY - other.centerY;
            double d =Math.Sqrt(dx*dx+dy*dy);
            return d <= (other.radiusX + this.radiusX);            
        }

        public void WallCollide(double borX1, double borY1, double borX2, double borY2) {
            double probX1 = frameX1 - borX1;        // must be < 0 to collide
            double probX2 = borX2 - frameX2;        // must be < 0 to collide
            double x = probX1 < probX2 ? probX1 : probX2;

            double probY1 = frameY1 - borY1;
            double probY2 = borY2 - frameY2;
            double y = probY1 < probY2 ? probY1 : probY2;

            if (x < y)
            {
                // process left-right collision
                velAlpha = 180 - velAlpha;
                if (velAlpha < 0) {
                    velAlpha += 360;
                }
            }
            else {
                velAlpha = 360 - velAlpha;
            }
        }

        public void CircleCollide(CircleObject obj) {
            double dx = obj.centerX - this.centerX;
            double dy = obj.centerY - this.centerY;

            double phiRad = Math.Atan2(dy, dx);
            double theta1Rad = this.velAlpha * Math.PI / 180;
            double theta2Rad = obj.velAlpha * Math.PI / 180;

            if (phiRad < 0) { phiRad += Math.PI; }
            int m1 = this.mass;
            int m2 = obj.mass;

            var temp1 = this.velModulo * Math.Cos(theta1Rad - phiRad) * (m1 - m2);
            var temp2 = 2 * m2 * obj.velModulo * Math.Cos(theta2Rad - phiRad);
            var temp3 = m1 + m2;

            var vx = ((temp1+temp2)/temp3)*Math.Cos(phiRad)+this.velModulo*Math.Sin(theta1Rad-phiRad)*Math.Cos(phiRad+Math.PI/2);
            var vy = ((temp1+temp2)/temp3)*Math.Sin(phiRad)+this.velModulo*Math.Sin(theta1Rad-phiRad)*Math.Sin(phiRad+Math.PI/2);
            
            this.newVelModulo = Math.Sqrt(vx * vx + vy * vy);
            this.newVelAlpha = Math.Atan2(vy, vx) * 180 / Math.PI;
            if (this.velAlpha < 0) {
                this.velAlpha += 360;
            }
            
            this.R = true;
        }
    }
}
