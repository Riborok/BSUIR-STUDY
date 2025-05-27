using System;

namespace Syntax
{
    public abstract class Rule
    {
        protected Rule(ElementType startElementType, ElementType firstArgumentType,
            ElementType secondArgumentType)
        {
            SecondArgumentType = secondArgumentType;
            FirstArgumentType = firstArgumentType;
            StartElementType = startElementType;
            Random = new Random();
        }

        protected Random Random { get; private set; }

        public ElementType StartElementType { get; private set; }

        public ElementType FirstArgumentType { get; private set; }

        public ElementType SecondArgumentType { get; private set; }

        public abstract Element TransformConnect(Element first, Element second);

        public abstract Element Connect(Element first, Element second);

        public abstract bool IsRulePare(Element first, Element second);
    }
}