using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Syntax;

namespace Lab7
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        private Point startPoint;

        private bool isDrawingModeEnabled;

        private GeometryGroup currentGroup = new GeometryGroup();

        private List<Element> drawedElements = new List<Element>();

        private void butSynthesis_Click(object sender, EventArgs e) {
            var grammar = new HomeGrammar();
            Element home = grammar.GetHome();
            Clear();
            foreach (Line line in home.Lines)
            {
              drawedElements.Add(HomeGrammar.GetTerminalElement(line));
            }
            home.ScaleTransform((pbMain.Width - 140)/home.Length,
              (pbMain.Height - 25)/home.Height);
            currentGroup = home.GetGeometryGroup();
            pbMain.Invalidate(); 
        }

        private void butAnalysis_Click(object sender, EventArgs e) {
            var grammar = new HomeGrammar();
            RecognazingResult recognazingResult = grammar.IsHome(drawedElements);
            if (recognazingResult.IsHome)
            {
              MessageBox.Show(@"The picture matches the grammar");
            }
            else
            {
              MessageBox.Show(string.Format(@"The picture does NOT follow the grammar. Element not found: {0}", recognazingResult.ErrorElementName));
            }
        }

        private void butClear_Click(object sender, EventArgs e) {
            Clear();
        }
        
        private void Clear()
        {
            currentGroup.Clear();
            drawedElements = new List<Element>();
            pbMain.Invalidate();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e) {
              if (isDrawingModeEnabled)
              {
                    isDrawingModeEnabled = false;
                    drawedElements.Add(HomeGrammar.GetTerminalElement(new Line(GetCortanianCoordinates(startPoint), GetCortanianCoordinates(e.Location))));
                    currentGroup.Add(new Line(startPoint, e.Location));
                    pbMain.Invalidate();
              }
              else
              {
                    isDrawingModeEnabled = true;
                    startPoint = e.Location;
              }
        }

        private Point GetCortanianCoordinates(Point position)
        {
            return new Point(position.X, Height - 20 - position.Y);
        }

        private void pbMain_Paint(object sender, PaintEventArgs e) {
            currentGroup.Paint(e.Graphics);
        }
    }
}