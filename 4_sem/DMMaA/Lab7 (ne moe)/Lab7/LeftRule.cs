using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;

namespace Syntax
{
    public class LeftRule : Rule
    {
        private const int randomDelta = 3;

        public LeftRule(ElementType startElementType, ElementType firstArgumentType,
            ElementType secondArgumentType)
            : base(startElementType, firstArgumentType, secondArgumentType)
        {
        }

        public override Element TransformConnect(Element first, Element second)
        {
            second.ShiftTransform(first.Length + Random.Next(1, 10), 0);
            return Connect(first, second);
        }

        public override Element Connect(Element first, Element second)
        {
            var resultLines = new List<Line>(first.Lines);
            resultLines.AddRange(second.Lines);
            var startPosition = new Point(first.StartPosition.X,
                Math.Max(first.StartPosition.Y, second.StartPosition.Y));
            var endPosition = new Point(second.EndPosition.X,
                Math.Min(first.EndPosition.Y, second.EndPosition.Y));
            var connect = new Element(StartElementType, resultLines, startPosition, endPosition);
            return connect;
        }


        public override bool IsRulePare(Element first, Element second)
        {
            if (first.ElementType.Name != FirstArgumentType.Name ||
                second.ElementType.Name != SecondArgumentType.Name) return false;
            return first.EndPosition.X - randomDelta < second.StartPosition.X;
        }
    }
}