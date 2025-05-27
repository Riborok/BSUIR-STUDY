using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab5 {
    public partial class frmMain : Form
    {
        private int PlotCenterX = 300;
        private int PlotCenterY = 200;
        private const int PlotScale = 15;
        private const int PlotObjectSize = 8;
        private const int PlotStartingX = 0;
        private int PlotEndingX = 900;
        private Potential _potential = new Potential();
        private List<TestingObject> _objs = new List<TestingObject>();
        public frmMain()
        {
            InitializeComponent();
            _objs.Add(new TestingObject(-1,0,1));
            _objs.Add(new TestingObject(1,1,1));
            _objs.Add(new TestingObject(2,0,2));
            _objs.Add(new TestingObject(1,-2,2));
            ShowItems();
        }
        
        public bool ShowItems() {
            _objs.Sort((x, y) => x.Class.CompareTo(y.Class));

            if (_objs.Count == 0) {
                textBox2.Text = textBox1.Text = string.Empty;
                return false;
            }

            var sb1 = new StringBuilder();
            var sb2 = new StringBuilder();
            
            foreach (var obj in _objs)
            {
                string valuesString = "(" + obj.X + "," + obj.Y + ")";
                var sb = obj.Class == 2 ? sb2 : sb1;
                sb.Append(valuesString);
                sb.AppendLine();
            }
            textBox1.Text = sb1.ToString();
            textBox2.Text = sb2.ToString();
            return textBox1.Text.Length > 0 && textBox2.Text.Length > 0;
        }

        private float GetFuncVal(int[] arr, float x)
        {
            return (float)(-1 * arr[1] * x + -1 * arr[0]) / (float)(arr[3] * x + arr[2]);
        }
        
        private void btnAdd_Click(object sender, EventArgs e) {
            Add(1);
        }
        
        private void btnAdd2_Click(object sender, EventArgs e) {
            Add(2);
        }

        private void Add(int classNum) {
            if (int.TryParse(textBox3.Text, out int x) && int.TryParse(textBox4.Text, out int y)) {
                _objs.Add(new TestingObject(x, y, classNum));
                ShowItems();
            }
        }

        private void Draw(Graphics g, int[] arr) {
            PlotCenterX = panel2.Width / 2;
            PlotCenterY = panel2.Height / 2;
            PlotEndingX = panel2.Width;
            
            g.Clear(Color.DarkGray);
            Pen pen = new Pen(Color.Black, 2);
            g.DrawLine(pen,0,PlotCenterY,PlotEndingX,PlotCenterY ) ;
            g.DrawLine(pen, PlotCenterX, 0, PlotCenterX, panel2.Height);

            Brush penClass1 =  new SolidBrush(Color.White);
            Brush penClass2 =  new SolidBrush(Color.Black);
            foreach (var obj in _objs)
            {
                g.FillEllipse(obj.Class == 1 ? penClass1 : penClass2, 
                    obj.X * PlotScale + PlotCenterX- PlotObjectSize / 2,-1 *obj.Y * PlotScale + PlotCenterY - PlotObjectSize / 2,
                    PlotObjectSize,PlotObjectSize);
            }

            var func = new Pen(Color.Orange);
            func.Width = 3f;
            float X = PlotStartingX;
            float step = 0.05f;
            var points = new List<(float, float)>();
            while (X <= PlotEndingX)
            {
                float xVal = (X - PlotCenterX) / (float)PlotScale;
                float tempX = xVal * PlotScale + PlotCenterX - PlotObjectSize / 4;
                float tempY = -1 * GetFuncVal(arr, xVal) * PlotScale + PlotCenterY - PlotObjectSize / 4;
                points.Add((tempX, tempY));
                X += step; 
            }
            for (int i = 0; i < points.Count - 1; i++) {
                var p1 = points[i];
                var p2 = points[i+1];
                if (Math.Abs(p1.Item2 - points[i + 1].Item2) < 500)
                {
                    g.DrawLine(func, p1.Item1, p1.Item2, p2.Item1, p2.Item2);
                }
            }
        }
        private void btnSolve_Click(object sender, EventArgs e)
        {
            if (!ShowItems())
            {
                MessageBox.Show(@"Both classes should have at least 1 item");
                return;
            }
            _potential.LearnCollection(_objs);
            int[] arr = _potential.OutSolvingFucn();
            // y = [1] * x - [0] / [3]x + [2]
            tbFunc.Text = $@"y = ({-1 * arr[1]} * x + {-1 * arr[0]} ) / ({arr[3]} * x + {arr[2]})";

            Graphics g = panel2.CreateGraphics();
            Draw(g,arr);
        }

        private void button1_Click(object sender, EventArgs e) {
            _objs.Clear();
            ShowItems();
        }
    }
}