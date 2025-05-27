using System.Drawing;
using System.Windows;

namespace Syntax
{
    internal class TerminalElementType : ElementType
    {
        private readonly Line standartElementLine;

        public TerminalElementType(string name, Line standartElementLine)
            : base(name)
        {
            this.standartElementLine = standartElementLine;
        }

        public Element StandartElement
        {
            get
            {
                return new Element(this,
                    new Line(new Point(standartElementLine.From.X, standartElementLine.From.Y),
                        new Point(standartElementLine.To.X, standartElementLine.To.Y)));
            }
        }
    }
}