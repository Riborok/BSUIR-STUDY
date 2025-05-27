using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using oop3.DisplayObjects;

namespace oop2
{
    internal class Game
    { 

        private GameField gameField;
        public Game(GameField field) {
            gameField = field;
        }
        public void MoveObjects(double time) { 
            gameField.MoveObjects(time);
        }

        public void DrawGame(Graphics g) {
            gameField.Draw(g);
        }
        
        public bool HandleClick(int x, int y) {
            return gameField.Click(x, y);
        }
    }
}
