using System;
using System.Collections.Generic;
using System.Drawing;
using Syntax;

namespace Lab7 {
    public class GeometryGroup
    {
        private readonly List<Line> _lines;

        public GeometryGroup()
        {
            _lines = new List<Line>();
        }

        public void Add(Line line)
        {
            _lines.Add(line);
        }

        public void Clear()
        {
            _lines.Clear();
        }

        public void Paint(Graphics g) {
            Brush brush = new SolidBrush(Color.Black);
            Pen pen = new Pen(brush);
            
            foreach (var line in _lines)
            {
                g.DrawLine(pen, line.From, line.To);        
            }
        }
    }
}