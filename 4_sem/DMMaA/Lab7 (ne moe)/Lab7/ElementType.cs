using System;

namespace Syntax
{
    public class ElementType
    {
        public ElementType(string name)
        {
            Name = name;
        }

        public String Name { get; private set; }
    }
}