using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Microsoft.SqlServer.Server;

namespace LabN6
{
    public partial class frmMain : Form
    {
        private bool _solved = false;
        private const int HalvedRangeCap = 7;
        private const int PlotWidth = 900;
        private const float PlotHeight = 400;//445;
        private Hierarchy _solver = new Hierarchy(5);
        private int _baseDrawn = 1;
        public frmMain()
        {
            InitializeComponent();
        }
        
        private void ShowDistances(List<List<int>> data)
        {
            dgvDistances.Rows.Clear();
            dgvDistances.ColumnCount = data[0].Count;
            for (int i = 0; i < data[0].Count; i++)
            {
                dgvDistances.Columns[i].Name = (i + 1).ToString();
            }

            
            for (int i = 0; i < data.Count; i++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgvDistances);
                for (int j = 0; j < data[i].Count; j++)
                {
                    row.Cells[j].Value = data[i][j];
                }
                dgvDistances.Rows.Add(row);
            }
            
            for (int i = 0; i < data.Count; i++)
            {
                dgvDistances.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }
            
            foreach (DataGridViewColumn column in dgvDistances.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        private void cbDistances_CheckedChanged(object sender, EventArgs e)
        {
            dgvDistances.Visible ^= true;
            // This rerandom as well which is bad
            //if (_solved) DrawAll();
        }

        private float Draw(Graphics g,bool[] avalaible, float[] pos, int current, ref List<Pair> data, float r, float w, float oldX, int prevMark)
        {
            Font font = new Font("Times New Roman", 10);
            Brush brush = new SolidBrush(Color.Black);
            Brush valBrush = new SolidBrush(Color.Black);
            Pair temp = null;
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].Mark == current)
                {
                    temp = data[i];
                }
            }

            float rX = 0, rY = 0;
            if (!avalaible[temp.X])
            {
                rX = Draw(g, avalaible, pos, temp.X, ref data, r, w, 0, -1);
            }


            if (!avalaible[temp.Y])
            {
                rY = Draw(g, avalaible, pos, temp.Y, ref data, r, w, 0, -1);
            }

            if (oldX != 0)
            {
                if (temp.X == prevMark)
                {
                    rX = oldX;
                }
                else
                {
                    rY = oldX;
                }
            }
            
            if (pos[temp.X] == 0)
            {
                pos[temp.X] = w * _baseDrawn;
                _baseDrawn++;
                g.DrawString($"x{temp.X + 1}", font, brush, pos[temp.X] - (float)PlotWidth / 200 , PlotHeight  + PlotHeight / 50);

            }
            
            if (pos[temp.Y] == 0)
            {
                pos[temp.Y] = w * _baseDrawn;
                _baseDrawn++;
                g.DrawString($"x{temp.Y + 1 }", font, brush, pos[temp.Y] - (float)PlotWidth / 200 , PlotHeight + PlotHeight / 50);

            }

            pos[current] = (pos[temp.X] + pos[temp.Y]) / 2;
            Pen pen = new Pen(Color.Black, 2);
            float pairY = PlotHeight * 0.9F * (temp.R / r);
          
            g.DrawLine(pen, pos[temp.X], PlotHeight - pairY, pos[temp.X], PlotHeight - rX);
            g.DrawLine(pen, pos[temp.Y], PlotHeight - pairY, pos[temp.Y], PlotHeight - rY);
            g.DrawLine(pen, pos[temp.X], PlotHeight - pairY, pos[temp.Y], PlotHeight - pairY);
            g.DrawString($"H{temp.Mark + 1 - (avalaible.Length / 2 + 1)}", font, brush, pos[current] - (float)PlotWidth / 200, PlotHeight - pairY + PlotHeight / 50);
            g.DrawString($"{temp.R}", font, valBrush,0F, PlotHeight - pairY - PlotHeight / 50);
            data.Remove(temp);
            avalaible[temp.Mark] = true;
            return pairY;
        }

        private void DrawAll() {
            if (!int.TryParse(textBox1.Text, out int val))
                return;
            
            _baseDrawn = 1;
            _solver.SetN(val);
            ShowDistances(_solver.GetRanges());
            List<Pair> data = _solver.Solve(cbMaximized.Checked);
            float maxRange = data[data.Count - 1].R;
            float maxWidth = PlotWidth;
            if (data.Count <= HalvedRangeCap)
            {
                maxWidth /= 2;
            }
            
            int actualSize = val * 2 - 1;
            
            bool[] avalaible = new bool[actualSize];
            float[] position = new float[actualSize];
            for (int i = 0; i < val; i++)
            {
                avalaible[i] = true;
            }

            float step = maxWidth / (val + 1);
            int current;
            int next = data[0].Mark;
            Graphics g = panel2.CreateGraphics();
            g.Clear(Color.White);
            float prevX = 0;
            int prevMark = -1;
            while (data.Count > 0)
            {
                current = next;
                for (int i = 0; i < data.Count; i++)
                {
                    if (data[i].X == current || data[i].Y == current)
                    {
                        next = data[i].Mark;
                        break;
                    } 
                }

                Pen pen = new Pen(Color.Black);
                g.DrawLine(pen, -100, PlotHeight, 2500, PlotHeight);
                prevX = Draw(g, avalaible, position, current, ref data, maxRange, step, prevX, prevMark);
                prevMark = current;
            }
        }
        private void btnSolve_Click(object sender, EventArgs e)
        {
            _solved = true;
            DrawAll();
        }
    }
}
