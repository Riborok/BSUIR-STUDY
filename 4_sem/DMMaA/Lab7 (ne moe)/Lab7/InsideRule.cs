using System;
using System.Collections.Generic;

namespace Syntax
{
    public class InsideRule : Rule
    {
        private const int randomDelta = 3;

        public InsideRule(ElementType startElementType, ElementType firstArgumentType,
            ElementType secondArgumentType)
            : base(startElementType, firstArgumentType, secondArgumentType)
        {
        }

        public override Element TransformConnect(Element first, Element second)
        {
            second.ScaleTransform(first.Length/second.Length + 0.8, first.Height/second.Height + 0.8);
            first.ShiftTransform(
                second.StartPosition.X +
                Random.Next((int) (Math.Abs(first.Length - second.Length)*0.5), (int) (Math.Abs(first.Length - second.Length)*0.8)) -
                first.StartPosition.X,
                second.EndPosition.Y +
                Random.Next((int)(Math.Abs(first.Height - second.Height) * 0.5), (int)(Math.Abs(first.Height - second.Height) * 0.8)) -
                first.EndPosition.Y);
            return Connect(first, second);
        }

        public override Element Connect(Element first, Element second)
        {
            var resultLines = new List<Line>(first.Lines);
            resultLines.AddRange(second.Lines);
            var connect = new Element(StartElementType, resultLines, second.StartPosition,
                second.EndPosition);
            return connect;
        }

        public override bool IsRulePare(Element first, Element second)
        {
            if (first.ElementType.Name != FirstArgumentType.Name ||
                second.ElementType.Name != SecondArgumentType.Name) return false;
            return first.StartPosition.X > second.StartPosition.X - randomDelta &&
                   first.StartPosition.Y - randomDelta < second.StartPosition.Y
                   && first.EndPosition.X - randomDelta < second.EndPosition.X &&
                   first.EndPosition.Y > second.EndPosition.Y - randomDelta;
        }
    }
}