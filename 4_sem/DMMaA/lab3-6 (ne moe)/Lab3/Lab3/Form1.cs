using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabN3
{
    public partial class Form1 : Form
    {
        private double prob1 = 0;
        private double prob2 = 0;
        private const int PointCount = 5000;
        const int Range = 600;
        const int Start1 = 0;
        const int Start2 = 200;
        private List<int> _points1 = new List<int>();
        private List<int> _points2 = new List<int>();
        private Random _rand= new Random();
        
        private void Solve(Graphics e)
        {
            Brush br = new SolidBrush(Color.White);
            e.FillRectangle(br, 0, 0, 2000, 2000);
            prob1 = (double)numProb1.Value;
            prob2 = (double)numProb2.Value;
            double M1 = 0;
            double M2 = 0;
            int temp;
            for (int i = 0; i < PointCount; i++)
            {
                temp = _rand.Next(Start1, Start1 + Range);
                _points1.Add(temp);
                M1 += temp;
                temp = _rand.Next(Start2, Start2 + Range);
                _points2.Add(temp);
                M2 += temp;
            }

            M1 /= PointCount;
            M2 /= PointCount;

            double sigma1 = 0;
            double sigma2 = 0;
            for (int i = 0; i < PointCount; i++)
            {
                sigma1 += Math.Pow(_points1[i] - M1,2);
                sigma2 += Math.Pow(_points2[i] - M2,2);
            }

            sigma1 = Math.Sqrt(sigma1 / PointCount);
            sigma2 = Math.Sqrt(sigma2 / PointCount);

            var plot1 = new double[3 *Range];
            var plot2 = new double[3 * Range];
            int Mid = 0;
            for (int i = -100; i < 3 * Range - 400; i++)
            {
                plot1[i + 100] = Math.Exp(-0.5*Math.Pow((i - M1)/sigma1, 2)) / (sigma1*Math.Sqrt(2*Math.PI))* prob1;
                plot2[i + 100] = Math.Exp(-0.5*Math.Pow((i - M2)/sigma2, 2)) / (sigma2*Math.Sqrt(2*Math.PI))* prob2;
                if (Math.Abs(plot1[i + 100] * 15000 - plot2[i + 100]* 15000) < 0.1 && Mid == 0)
                {
                    Mid = i + 100;
                }
            }

            double sum = plot1.Sum();
      
            Brush brush1 = new SolidBrush(Color.Blue);
            Brush brush2 = new SolidBrush(Color.Green);
            for (int i = -100; i < 3 * Range - 400; i++)
            {
                e.FillEllipse(brush1, i + 100, 450 -(float)plot1[i + 100] * 150000, 2, 2 );
                e.FillEllipse(brush2,  i + 100, 450 -(float)plot2[i + 100] * 150000, 2, 2 );
            }

            Pen pen = new Pen(Color.Black, 1);
            e.DrawLine(pen,0,452,1400,452 ) ;
            e.DrawLine(pen, 100, 0, 100, 1000);
            Pen midpen = new Pen(Color.Red, 1);
            e.DrawLine(midpen,Mid + 3, 0, Mid + 3, 10000);
            
            
            double error1 = plot2.Take((int)Mid).Sum();
            double error2 = plot1.Skip((int) Mid).Sum();
        //    error1 /= prob1;
        //    error2 /= prob2;
            numLT.Text = error1.ToString();
            numSP.Text = error2.ToString();
            numSum.Text = (error1 + error2).ToString();

        }
        public Form1()
        {
            InitializeComponent();
        }

        private void numProb1_ValueChanged(object sender, EventArgs e)
        {
            numProb2.Value = 1 - numProb1.Value;
        }

        private void numProb2_ValueChanged(object sender, EventArgs e)
        {
            numProb1.Value = 1 - numProb2.Value;
        }

        private void btnSolve_Click(object sender, EventArgs e)
        {
            
            Solve(pnlPlot.CreateGraphics());
            
        }
    }
}