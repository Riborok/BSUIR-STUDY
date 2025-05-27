using System;

namespace Syntax
{
    public class RecognazingResult
    {
        public RecognazingResult(string errorElementName, bool isHome)
        {
            ErrorElementName = errorElementName;
            IsHome = isHome;
        }

        public String ErrorElementName { get; set; }
        public bool IsHome { get; set; }
    }
}