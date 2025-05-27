using System;
using System.Windows.Forms;

namespace lab1
{
    public partial class Form1 : Form {
        private Game _game;
        
        public Form1() {
            InitializeComponent();
            _game = new Game(pb, 100);
        }

        private void button1_Click(object sender, EventArgs e) {
            _game.CreateGameField();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            _game.IsContinue = false;
        }

        private void pb_Paint(object sender, PaintEventArgs e) {
            _game.Play(e.Graphics);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.ShiftKey) {
                _game.IsContinue = !_game.IsContinue;
            }
        }

        private void button1_KeyDown(object sender, KeyEventArgs e) {
            Form1_KeyDown(sender, e);
        }
    }
}