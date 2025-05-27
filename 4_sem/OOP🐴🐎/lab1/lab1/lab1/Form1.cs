using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using lab1.Drawable;
using Rectangle = lab1.Drawable.Rectangle;

namespace lab1
{
    public partial class Form1 : Form
    {
        private GameField _gameField;
        
        public Form1() => InitializeComponent();

        private void pictureBox1_Paint_1(object sender, PaintEventArgs e)
        {
            if (_gameField == null)
                return;
                
            var g = e.Graphics;
            _gameField.Draw(g);
        }
        
        private void button1_Click_1(object sender, EventArgs e)
        {
            CreateGameField();
            
            pb.Invalidate();
        }

        private void CreateGameField()
        {
            const int thickness = 10;
            Color fill = Color.White;
            Color borderColor = Color.GreenYellow;

            const int x = 100;
            const int y = 100;
            _gameField = new GameField(x, y, x, y, pb.Width - x * 2, pb.Height - y * 2, fill, thickness, borderColor);
        }
    }
}