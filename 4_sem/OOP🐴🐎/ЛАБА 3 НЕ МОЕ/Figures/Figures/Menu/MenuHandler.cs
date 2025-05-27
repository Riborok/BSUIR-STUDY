using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Figures.Menu
{
    public partial class MenuHandler : Form
    {
        private int childFormNumber = 0;

        public int tR, tG, tB;

        public int fR, fG, fB;

        public int bR, bG, bB;

        public string text, textStyle;

        public int textSize;

        public int borderWidth;

        public int X, Y;

        public int dx, dy;

        public string objType;

        public int width, height;

        private void Edit_Click(object sender, EventArgs e)
        {
            text = inputText.Text;

            if (String.IsNullOrEmpty(text))
            {
                text = "Text";
            }

            textStyle = fontStyle.Text;

            if (String.IsNullOrEmpty(textStyle))
            {
                textStyle = "Arial";
            }

            if (String.IsNullOrEmpty(inputTextSize.Text))
            {
                textSize = 14;
            }
            else
            {
                int.TryParse(inputTextSize.Text, out textSize);

                if (textSize == 0) textSize = 5;
            }

            if (String.IsNullOrEmpty(inputBorderWidth.Text))
            {
                borderWidth = 5;
            }
            else
            {
                int.TryParse(inputBorderWidth.Text, out borderWidth);

                if (borderWidth == 0)
                {
                    bR = fR;
                    bG = fG;
                    bB = fB;
                }

            }

            objType = figureType.Text;

            if (String.IsNullOrEmpty(objType))
            {
                objType = "rectangle";
            }

            if (String.IsNullOrEmpty(inputX.Text))
            {
                X = menuItem.x;
            }
            else
            {
                int.TryParse(inputX.Text, out X);

                if (X == 0) X = menuItem.x;
            }

            if (String.IsNullOrEmpty(inputY.Text))
            {
                Y = menuItem.y;
            }
            else
            {
                int.TryParse(inputY.Text, out Y);

                if (Y == 0) Y = menuItem.y;
            }

            if (String.IsNullOrEmpty(inputWidth.Text))
            {
                width = (int)(menuItem.frameObj.fFrameX - menuItem.frameObj.sFrameX);
            }
            else
            {
                int.TryParse(inputWidth.Text, out width);

                if (width == 0) width = (int)(menuItem.frameObj.fFrameX - menuItem.frameObj.sFrameX);
            }

            if (String.IsNullOrEmpty(inputHeight.Text))
            {
                height = (int)(menuItem.frameObj.fFrameY - menuItem.frameObj.sFrameY);
            }
            else
            {
                int.TryParse(inputHeight.Text, out height);

                if (height == 0) height = (int)(menuItem.frameObj.fFrameY - menuItem.frameObj.sFrameY);
            }

            if (String.IsNullOrEmpty(inputDx.Text))
            {
                dx = 0;
            }
            else
            {
                int.TryParse(inputDx.Text, out dx);
            }

            if (String.IsNullOrEmpty(inputDy.Text))
            {
                dy = 0;
            }
            else
            {
                int.TryParse(inputDy.Text, out dy);
            }

            menuItem.changeBorderColor(bR, bG, bB);
            
            menuItem.changeCoords(X, Y, dx, dy);
            menuItem.resize(width, height);
            menuItem.changeFillColor(fR, fG, fB);
            menuItem.changeTextColor(tR, tG, tB);
            menuItem.changeText(text, textStyle, textSize);
            menuItem.changeObject(objType);
            menuItem.changeBorderSize(borderWidth);

            menuItem.curShiftX = (int)menuItem.frameObj.fFrameX;
            menuItem.curShiftY = (int)menuItem.frameObj.fFrameY;
        }

        private MenuItems menuItem;

        public MenuHandler(MenuItems menuItem)
        {
            InitializeComponent();

            this.menuItem = menuItem;
            
            fontColorBox.BackColor = Color.FromArgb(menuItem.tR, menuItem.tG, menuItem.tB);
            fColorBox.BackColor = Color.FromArgb(menuItem.frameObj.fR, menuItem.frameObj.fG, menuItem.frameObj.fB);
            bColorBox.BackColor = Color.FromArgb(menuItem.frameObj.bR, menuItem.frameObj.bG, menuItem.frameObj.bB);

            bR = bColorBox.BackColor.R;
            bG = bColorBox.BackColor.G;
            bB = bColorBox.BackColor.B;
            fR = fColorBox.BackColor.R;
            fG = fColorBox.BackColor.G;
            fB = fColorBox.BackColor.B;
            tR = fontColorBox.BackColor.R;
            tG = fontColorBox.BackColor.G;
            tB = fontColorBox.BackColor.B;
            
            inputText.Text = menuItem.text.ToString();
            inputTextSize.Text = menuItem.fontSize.ToString();
            inputWidth.Text = (menuItem.frameObj.fFrameX - menuItem.frameObj.sFrameX).ToString();
            inputHeight.Text = (menuItem.frameObj.fFrameY - menuItem.frameObj.sFrameY).ToString();
            inputX.Text = (menuItem.frameObj.sFrameX - (35)).ToString();
            inputY.Text = (menuItem.frameObj.sFrameY - (35)).ToString();
            inputBorderWidth.Text = (menuItem.frameObj.borderThickness).ToString();
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            Form childForm = new Form();
            childForm.MdiParent = this;
            childForm.Text = "Окно " + childFormNumber++;
            childForm.Show();
        }

        private void MenuHandler_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog(this) == DialogResult.OK)
            {
                bColorBox.BackColor = colorDialog1.Color;

                Color color = bColorBox.BackColor;

                bR = bColorBox.BackColor.R;
                bG = bColorBox.BackColor.G;
                bB = bColorBox.BackColor.B;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog(this) == DialogResult.OK)
            {
                fColorBox.BackColor = colorDialog1.Color;

                Color color = bColorBox.BackColor;
                fR = fColorBox.BackColor.R;
                fG = fColorBox.BackColor.G;
                fB = fColorBox.BackColor.B;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog(this) == DialogResult.OK)
            {
                fontColorBox.BackColor = colorDialog1.Color;

                Color color = bColorBox.BackColor;
                tR = fontColorBox.BackColor.R;
                tG = fontColorBox.BackColor.G;
                tB = fontColorBox.BackColor.B;
            }
        }
    }
}
