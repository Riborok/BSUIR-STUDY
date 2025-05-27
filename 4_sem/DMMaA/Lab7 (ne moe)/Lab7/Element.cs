using System;
using System.Windows;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using Lab7;

namespace Syntax
{
    public class Element
    {
        public Element(ElementType elementType)
        {
            ElementType = elementType;
            Lines = new List<Line>();
        }

        public Element(ElementType elementType, Line line)
        {
            ElementType = elementType;
            Lines = new List<Line> {line};
            StartPosition = new Point(Math.Min(line.From.X, line.To.X),
                Math.Max(line.From.Y, line.To.Y));
            EndPosition = new Point(Math.Max(line.From.X, line.To.X),
                Math.Min(line.From.Y, line.To.Y));
        }

        public Element(ElementType elementType, IEnumerable<Line> lines, Point startPoint,
            Point endPoint)
        {
            StartPosition = startPoint;
            EndPosition = endPoint;
            ElementType = elementType;
            Lines = lines;
        }

        public ElementType ElementType { get; private set; }

        public Point StartPosition { get; set; }

        public Point EndPosition { get; set; }

        public IEnumerable<Line> Lines { get; private set; }

        public double Length
        {
            get { return Math.Abs(EndPosition.X - StartPosition.X); }
        }

        public double Height
        {
            get { return Math.Abs(EndPosition.Y - StartPosition.Y); }
        }

        public void ScaleTransform(double xScale, double yScale)
        {
            double deltaX = EndPosition.X - StartPosition.X;
            double deltaY = EndPosition.Y - StartPosition.Y;
            deltaX *= xScale;
            deltaY *= yScale;
            var point = EndPosition;
            point.X = Convert.ToInt32(StartPosition.X + deltaX);
            point.Y = Convert.ToInt32(StartPosition.Y + deltaY);
            EndPosition = point;
            foreach (Line line in Lines)
            {
                line.ScaleTransform(xScale, yScale, StartPosition);
            }
        }

        public void ShiftTransform(double xDelta, double yDelta)
        {
            var point = StartPosition;
            point.X += Convert.ToInt32(xDelta);
            point.Y += Convert.ToInt32(yDelta);
            StartPosition = point;

            point = EndPosition;
            point.X += Convert.ToInt32(xDelta);
            point.Y += Convert.ToInt32(yDelta);
            EndPosition = point;
            
            foreach (Line line in Lines)
            {
                line.ShiftTransform(xDelta, yDelta);
            }
        }

        public GeometryGroup GetGeometryGroup()
        {
            var result = new GeometryGroup();
            foreach (Line line in Lines)
            {
                result.Add(new Line(GetScreenPoint(line.From),
                    GetScreenPoint(line.To)));
            }
            return result;
        }

        private Point GetScreenPoint(Point point)
        {
            return new Point(point.X, StartPosition.Y - point.Y);
        }
    }
}