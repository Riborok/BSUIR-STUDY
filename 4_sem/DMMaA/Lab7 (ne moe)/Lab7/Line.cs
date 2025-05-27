using System;
using System.Drawing;
using System.Windows;

namespace Syntax
{
    public class Line
    {
        public Line(Point from, Point to)
        {
            From = from;
            To = to;
        }

        public Point From { get; set; }

        public Point To { get; set; }

        public void ScaleTransform(double xScale, double yScale, Point centerPoint)
        {
            double lengthX = To.X - From.X;
            double lengthY = To.Y - From.Y;
            
            double startDeltaX = From.X - centerPoint.X;
            double startDeltaY = From.Y - centerPoint.Y;
            
            startDeltaX *= xScale;
            startDeltaY *= yScale;

            var point = From;
            point.X = (int)(centerPoint.X + startDeltaX);
            point.Y = (int)(centerPoint.Y + startDeltaY);
            From = point;
            
            lengthX *= xScale;
            lengthY *= yScale;
            
            point = To;
            point.X = (int)(From.X + lengthX);
            point.Y = (int)(From.Y + lengthY);
            To = point;
        }

        public void ShiftTransform(double xDelta, double yDelta)
        {
            var point = To;
            point.X += Convert.ToInt32(xDelta);
            point.Y += Convert.ToInt32(yDelta);
            To = point;
            
            point = From;
            point.X += Convert.ToInt32(xDelta);
            point.Y += Convert.ToInt32(yDelta);
            From = point;
        }
    }
}