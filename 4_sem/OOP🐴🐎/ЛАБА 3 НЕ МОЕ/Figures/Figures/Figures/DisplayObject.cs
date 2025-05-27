using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.ComponentModel.Com2Interop;

namespace Figures.Figures
{
    public abstract class DisplayObject
    {
        public float sFrameX; //начальная координата клиентской области
        public float sFrameY;
        public float fFrameX; //конечная координата клиентской области
        public float fFrameY;

        public float sBaseX; //начальная координата точки привязки
        public float sBaseY;
        public float fBaseX; //конечная координата точки привязки
        public float fBaseY;

        public int fR; //Цвет клиентской части
        public int fG;
        public int fB;

        public int bR; //Цвет обводки
        public int bG;
        public int bB;

        protected float bX1; //Левая верхняя точка обводки 
        protected float bY1;
        protected float bX2; //Правая нижняя точка обводки
        protected float bY2;

        public int borderThickness; //Толщина обводки

        public double v, a;

        public double vX, vY; // Скорости по осям X и Y
        public double aX, aY; // Ускорения по осям X и Y

        public int alpha;
        public int t0;

        bool flag = false;
        
        public void Clone(DisplayObject other)
        {
            other.fR = this.fR;
            other.fG = this.fG;
            other.fB = this.fB;
            other.bR = this.bR;
            other.bG = this.bG;
            other.bB = this.bB;
            other.borderThickness = this.borderThickness;
            other.alpha = this.alpha;
            other.flag = this.flag;
        }

        protected DisplayObject(float sFrameX, float sFrameY, float fFrameX, float fFrameY, int fR, int fG, int fB, int bR, int bG, int bB, int borderThickness, float sBaseX, float sBaseY)
        {
            //Координаты клиентской части
            this.sFrameX = sFrameX;
            this.sFrameY = sFrameY;
            this.fFrameX = fFrameX;
            this.fFrameY = fFrameY;

            //Координаты точки привязки
            this.sBaseX = sBaseX;
            this.sBaseY = sBaseY;

            this.fR = fR;
            this.fG = fG;
            this.fB = fB;

            this.bG = bG;
            this.bB = bB;
            this.bR = bR;

            bX1 = sFrameX + borderThickness;
            bX2 = fFrameX - borderThickness;
            bY1 = sFrameY + borderThickness;
            bY2 = fFrameY - borderThickness;

            this.borderThickness = borderThickness;    
        }

        protected DisplayObject() { }

        public void Move(double dt)
        {
            if (Control.ModifierKeys == Keys.Shift)
            {
                flag = !flag;
            }
   
            if (!flag)
            {
                vX = (v * Math.Cos(alpha * Math.PI / 180.0));
                vY = (v * Math.Sin(alpha * Math.PI / 180.0));

                aX = (a * Math.Cos(alpha * Math.PI / 180.0));
                aY = (a * Math.Sin(alpha * Math.PI / 180.0));

                double dx = dt * (vX + aX * dt / 2);
                double dy = dt * (vY + aY * dt / 2);

                vX += aX * dt;
                vY += aY * dt;

                v = vX / Math.Cos(alpha * Math.PI / 180.0);

                Update((float)dx, (float)dy);
            }
            
        }

        public virtual void Update(float dx, float dy)
        {
            sFrameX += dx;
            sFrameY += dy;
            fFrameX += dx;
            fFrameY += dy;

            sBaseX += dx;
            sBaseY += dy;
            fBaseX += dx;
            fBaseY += dy;
        }

        public abstract void Draw(Graphics graphics);

        public abstract void DrawText(Graphics graphics);
    }
}
