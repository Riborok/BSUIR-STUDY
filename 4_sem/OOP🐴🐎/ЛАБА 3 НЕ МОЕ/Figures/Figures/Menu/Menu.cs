using Figures.Figures;
using System;
using System.Collections.Generic;
using System.Deployment.Internal;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using Rectangle = Figures.Figures.Rectangle;

namespace Figures.Menu
{
    internal class Menu
    {

        public int curItemPos = 0;
        int currentIndex = 0;
        private int iii = 36;

        GameField _gameField;

        public MenuItems[] menuItems = new MenuItems[0];

        private const int editIShift = 500 / 2;

        private static int itemToEditIndex = 0;

        Random random = new Random(); 
        
        public int subMenuIndex = 0;

        private int fWidth, fHeight;

        bool isClicked = true;

        public Menu(GameField _gameField)
        {
            this._gameField = _gameField;

            fWidth = _gameField.windowWidth;
            fHeight = _gameField.windowHeight;

            generateEditFields(_gameField.windowWidth, _gameField.windowHeight);
            curItemPos = 0;
        }

        public void AddItem(string objType, int x, int y, int fontSize, string text, int tR, int tG, int tB)
        {
            if (objType == "rectangle")
            {
                const int widthhh = 100;
                resizeArr(menuItems);
                Rectangle obj = _gameField.generateRandomRectangle();
                obj.borderThickness = 3;
                obj.fR = 255;
                obj.fG = 255;
                obj.fB = 255;
                
                obj.bR = 0;
                obj.bG = 0;
                obj.bB = 0;
                
                x = curItemPos == 0 ? x : iii += 10 + (widthhh + obj.borderThickness / 2 + menuItems[curItemPos - 1].borderThickness / 2);
                changePos(obj, x, y);
                resizeFrame(obj, widthhh, 50);
                
                menuItems[curItemPos] = new MenuItems(x, y, fontSize, text, "Arial", tR, tG, tB, obj, currentIndex, _gameField);

                menuItems[curItemPos].fontSize = 12;
                menuItems[curItemPos].tR = 120;
                menuItems[curItemPos].tG = 120;
                menuItems[curItemPos].tB = 120;
                
                menuItems[curItemPos].curShiftX = (int)menuItems[curItemPos].frameObj.fFrameX;
                menuItems[curItemPos].curShiftY = (int)menuItems[curItemPos].frameObj.fFrameY;

                menuItems[curItemPos].isAbsolute = true;

                currentIndex++;
                curItemPos++;
            }
            else if (objType == "circle")
            {
                resizeArr(menuItems);

                Circle obj = _gameField.generateRandomCircle();
                changePos(obj, x, y);
                resizeFrame(obj, 70, 70);

                menuItems[curItemPos] = new MenuItems(x, y, fontSize, text, "Arial", tR, tG, tB, obj, currentIndex, _gameField);

                menuItems[curItemPos].curShiftX = (int)menuItems[curItemPos].frameObj.fFrameX;
                menuItems[curItemPos].curShiftY = (int)menuItems[curItemPos].frameObj.fFrameY;

                menuItems[curItemPos].isAbsolute = true;

                currentIndex++;
                curItemPos++;
            }
            else if (objType == "triangle")
            {
                resizeArr(menuItems);

                Triangle obj = _gameField.generateRandomTriangle();
                changePos(obj, x, y);
                resizeFrame(obj, 70, 70);

                menuItems[curItemPos] = new MenuItems(x, y, fontSize, text, "Arial", tR, tG, tB, obj, currentIndex, _gameField);

                menuItems[curItemPos].curShiftX = (int)menuItems[curItemPos].frameObj.fFrameX;
                menuItems[curItemPos].curShiftY = (int)menuItems[curItemPos].frameObj.fFrameY;

                menuItems[curItemPos].isAbsolute = true;

                currentIndex++;
                curItemPos++;
            }
            else if (objType == "square")
            {
                resizeArr(menuItems);

                Square obj = _gameField.generateRandomSquare();
                changePos(obj, x, y);
                resizeFrame(obj, 70, 70);

                menuItems[curItemPos] = new MenuItems(x, y, fontSize, text, "Arial", tR, tG, tB, obj, currentIndex, _gameField);

                menuItems[curItemPos].curShiftX = (int)menuItems[curItemPos].frameObj.fFrameX;
                menuItems[curItemPos].curShiftY = (int)menuItems[curItemPos].frameObj.fFrameY;

                menuItems[curItemPos].isAbsolute = true;

                currentIndex++;
                curItemPos++;
            }
            else if (objType == "line")
            {
                resizeArr(menuItems);

                Line obj = _gameField.generateRandomLine();
                changePos(obj, x, y);
                resizeFrame(obj, 100, 50);

                menuItems[curItemPos] = new MenuItems(x, y, fontSize, text, "Arial", tR, tG, tB, obj, currentIndex, _gameField);

                menuItems[curItemPos].curShiftX = (int)menuItems[curItemPos].frameObj.fFrameX;
                menuItems[curItemPos].curShiftY = (int)menuItems[curItemPos].frameObj.fFrameY;

                menuItems[curItemPos].isAbsolute = true;

                currentIndex++;
                curItemPos++;
            }
            else if (objType == "ellipse")
            {

                resizeArr(menuItems);

                Ellipse obj = _gameField.generateRandomEllipse();
                changePos(obj, x, y);
                resizeFrame(obj, 100, 50);

                menuItems[curItemPos] = new MenuItems(x, y, fontSize, text, "Arial", tR, tG, tB, obj, currentIndex, _gameField);

                menuItems[curItemPos].curShiftX = (int)menuItems[curItemPos].frameObj.fFrameX;
                menuItems[curItemPos].curShiftY = (int)menuItems[curItemPos].frameObj.fFrameY;

                menuItems[curItemPos].isAbsolute = true;

                currentIndex++;
                curItemPos++;
            }
        }

        public void AddSubmenu(int index)
        {
            for (int i = 0; i < menuItems.Length; i++)
            {
                if (menuItems[i].index == index)
                {
                    menuItems[i].isParent = true;
                    if (menuItems[i].submenuCount == 0 && menuItems[i].isAbsolute)
                    {
                        menuItems[i].curShiftX += 7;
                        menuItems[i].curShiftY += 10;
                    }
                    
                    menuItems[i].submenuCount++;

                    insertMenu(i + menuItems[i].submenuCount, AddRandomSubitem(GetObjType(menuItems[i].frameObj), menuItems[i].curShiftX, menuItems[i].curShiftY, 14, "Text", random.Next(255), random.Next(255), random.Next(255)));
                    curItemPos++;

                    menuItems[i].curShiftX = (int)menuItems[i + menuItems[i].submenuCount].frameObj.fFrameX;
                    menuItems[i].curShiftY = (int)menuItems[i + menuItems[i].submenuCount].frameObj.fFrameY;
                    menuItems[i + menuItems[i].submenuCount].isParent = false;
                    menuItems[i + menuItems[i].submenuCount].isVisible = true;
                    menuItems[i + menuItems[i].submenuCount].parentIndex = index;

                    menuItems[i + menuItems[i].submenuCount].curShiftX = (int)menuItems[i + menuItems[i].submenuCount].frameObj.fFrameX;
                    menuItems[i + menuItems[i].submenuCount].curShiftY = (int)menuItems[i + menuItems[i].submenuCount].frameObj.sFrameY;

                    menuItems[i + menuItems[i].submenuCount].index = currentIndex;

                    menuItems[i].frameObj.Clone(menuItems[i + menuItems[i].submenuCount].frameObj);
                    menuItems[i].Clone(menuItems[i + menuItems[i].submenuCount]);
                    
                    currentIndex++;

                    break;
                }
            }   
        }

        private void insertMenu(int index, MenuItems obj)
        {
            MenuItems[] newArray = new MenuItems[menuItems.Length + 1];

            Array.Copy(menuItems, newArray, index);
            newArray[index] = obj;

            Array.Copy(menuItems, index, newArray, index + 1, menuItems.Length - index);

            menuItems = newArray;
        }

        public void AddEditSubmenu(string text)
        {
            editItems[0].subMenuItems = createSubmenu(editItems[0].subMenuItems);
            editItems[0].subMenuItems[editItems[0].submenuCount] = AddRandomSubitem(objTypes[0], editItems[0].x + editItems[0].curShift, editItems[0].y + editItems[0].curShift, 14, text, random.Next(255), random.Next(255), random.Next(255));
            editItems[0].curShift += 60;
            editItems[0].subMenuItems[editItems[0].submenuCount].index = subMenuIndex;
            editItems[0].submenuCount++;
            editItems[0].subMenuItems[subMenuIndex].frameObj.fR = 255;
            editItems[0].subMenuItems[subMenuIndex].frameObj.fG = 0;
            editItems[0].subMenuItems[subMenuIndex].frameObj.fB = 0;
            editItems[0].subMenuItems[subMenuIndex].tR = 0;
            editItems[0].subMenuItems[subMenuIndex].tG = 0;
            editItems[0].subMenuItems[subMenuIndex].tB = 0;
            editItems[0].subMenuItems[subMenuIndex].frameObj.bR = 0;
            editItems[0].subMenuItems[subMenuIndex].frameObj.bG = 0;
            editItems[0].subMenuItems[subMenuIndex].frameObj.bB = 255;

            editItems[0].subMenuItems[subMenuIndex].frameObj.borderThickness = 7;

            subMenuIndex++;
        }

        private MenuItems[] createSubmenu(MenuItems[] submenu)
        {
            MenuItems[] newArray = new MenuItems[submenu.Length + 1];

            Array.Copy(submenu, newArray, submenu.Length);

            return newArray;
        }

        public void DeleteItem(int index)
        {
            for (int i = 0; i < menuItems.Length; i++)
            {
                if (menuItems[i].index == index)
                {

                    if (menuItems.Length == 1)
                    {
                        MenuItems[] newArray = new MenuItems[menuItems.Length - 1];
                        menuItems = newArray;
                        curItemPos--;
                        break;
                    }

                    if (menuItems[i].isParent)
                    {
                        int len = 1 + menuItems[i].submenuCount;

                        for (int j = i + 1; j < len; j++)
                        {
                            if (menuItems[j].isParent)
                            {
                                len += menuItems[j].submenuCount;
                            }

                        }

                        curItemPos -= len;

                        MenuItems[] newArray = new MenuItems[menuItems.Length - len];

                        int pIndex = menuItems[i].parentIndex;

                        Array.Copy(menuItems, newArray, i);
                        Array.Copy(menuItems, i + 1 + menuItems[i].submenuCount, newArray, i, menuItems.Length - i - len);

                        menuItems = newArray;

                        for (int j = 0; j < menuItems.Length; j++)
                        {
                            if (menuItems[j].index == pIndex)
                            {
                                menuItems[j].submenuCount -= 1;
                                break;
                            }
                        }
                        break;
                    }
                    else
                    {
                        MenuItems[] newArray = new MenuItems[menuItems.Length - 1];
                        int pIndex = menuItems[i].parentIndex;

                        Array.Copy(menuItems, 0, newArray, 0, i);
                        
                        Array.Copy(menuItems, i + 1, newArray, i, menuItems.Length - i - 1);

                        menuItems = newArray;

                        curItemPos--;

                        for (int j = 0; j < menuItems.Length; j++)
                        {
                            if (menuItems[j].index == pIndex)
                            {
                                menuItems[j].submenuCount -= 1;
                                break;
                            }
                        }

                        break;
                    }
                }
            }
        }

        public void resizeFrame(DisplayObject obj, int width, int height)
        {
            obj.fFrameX = obj.sFrameX + width;
            obj.fFrameY = obj.sFrameY + height;
        }

        public void changePos(DisplayObject obj, int x, int y)
        {
            obj.sFrameX = x; obj.sFrameY = y;
            obj.fFrameX = x + (obj.fFrameX - obj.sFrameX);
            obj.fFrameY = y + (obj.fFrameY - obj.sFrameY);
        }

        public void changeFrameColor(DisplayObject obj, int r, int g, int b)
        {
            obj.fR = r;
            obj.fG = g;
            obj.fB = b;
        }

        public void generateEditFields(int width, int height)
        {
            generateEditField(width, height, editIShift * 3, 250, "Add");
            AddEditSubmenu("Add in menu");
            AddEditSubmenu("Add in submenu");
            generateEditField(width, height, editIShift * 2, 250, "Delete");
            generateEditField(width, height, editIShift, 250, "Edit");
        }
        private void generateEditField(int width, int height, int shiftX, int shiftY,string text)
        {
            Rectangle obj = _gameField.generateRandomRectangle();
            changePos(obj, width - shiftX, height - shiftY);
            resizeFrame(obj, 150, 50);
            
            obj.fR = 255;
            obj.fG = 0;
            obj.fB = 0;
            
            obj.bR = 0;
            obj.bG = 0;
            obj.bB = 255;

            obj.borderThickness = 7;

            editItems[curItemPos] = new MenuItems(width - shiftX, height - shiftY, 14, text, "Arial" ,0, 0, 0, obj, currentIndex, _gameField);

            currentIndex++;
            curItemPos++;
        }

        public void onClick(int x, int y, MenuItems[] items)
        {
            
            bool isFind = false;
            
            foreach (var obj in items)
            {
                if ((x >= obj.frameObj.sFrameX && x <= obj.frameObj.fFrameX) && (y >= obj.frameObj.sFrameY && y <= obj.frameObj.fFrameY))
                {
                    if (obj.isVisible)
                    {
                        itemToEditIndex = obj.index;
                    }
                    
                    int pos = 0;

                    for (int i = 0; i < menuItems.Length; i++)
                    {
                        if (menuItems[i].index == itemToEditIndex)
                        {
                            pos = i;
                            break;
                        }
                    }

                    isClicked = !isClicked;

                    if (isClicked)
                    {
                        int len = pos + menuItems[pos].submenuCount + 1;
                        for (int i = pos + 1; i < len; i++)
                        {
                             if (menuItems[i].isParent) len += menuItems[i].submenuCount;
                            menuItems[i].isVisible = false;
                        }
                    }
                    else
                    {
                        int len = pos + menuItems[pos].submenuCount + 1;

                        int i = pos + 1;

                        while (i < len)
                        {
                            menuItems[i].isVisible = true;

                            if (menuItems[i].isParent)
                            {
                                len += menuItems[i].submenuCount;
                                i += menuItems[i].submenuCount + 1;
                                continue;
                            }
                            i++;
                        }

                        
                    }

                    isFind = true;
                    
                }
            }

            if (!isFind)
            {
                for (int i = 0; i < menuItems.Length; i++)
                {
                    if (!menuItems[i].isAbsolute)
                    {
                        menuItems[i].isVisible=false;
                    }
                }
            }

        }

        private void resizeArr(MenuItems[] arr)
        {
            MenuItems[] newArray = new MenuItems[arr.Length + 1];

            Array.Copy(arr, newArray, arr.Length);

            menuItems = newArray;
        }
        
        public void onEditClick(int x, int y, MenuItems[] editItems)
        {
            foreach (var obj in editItems)
            {
                if ((x >= obj.frameObj.sFrameX && x <= obj.frameObj.fFrameX) && (y >= obj.frameObj.sFrameY && y <= obj.frameObj.fFrameY) && obj.index == 0 && Program.isEditVisible)
                {
                    foreach (var submenu in obj.subMenuItems)
                    {
                        submenu.isVisible = !submenu.isVisible;       
                    }    
                }

                if ((x >= obj.frameObj.sFrameX && x <= obj.frameObj.fFrameX) && (y >= obj.frameObj.sFrameY && y <= obj.frameObj.fFrameY) && obj.index == 1 && Program.isEditVisible)
                {
                    DeleteItem(itemToEditIndex);
                }

                if ((x >= obj.frameObj.sFrameX && x <= obj.frameObj.fFrameX) && (y >= obj.frameObj.sFrameY && y <= obj.frameObj.fFrameY) && obj.index == 2 && Program.isEditVisible)
                {
                    Edit(itemToEditIndex);
                }
            }

            foreach (var obj in editItems[0].subMenuItems)
            {
                if ((x >= obj.frameObj.sFrameX && x <= obj.frameObj.fFrameX) && (y >= obj.frameObj.sFrameY && y <= obj.frameObj.fFrameY) && obj.index == 0 && Program.isEditVisible && obj.isVisible)
                {
                    AddItem(objTypes[0], 36, 36, random.Next(14) + 10, "Text",random.Next(255), random.Next(255), random.Next(255));
                }

                if ((x >= obj.frameObj.sFrameX && x <= obj.frameObj.fFrameX) && (y >= obj.frameObj.sFrameY && y <= obj.frameObj.fFrameY) && obj.index == 1 && Program.isEditVisible && obj.isVisible)
                {
                    AddSubmenu(itemToEditIndex);
                }
            }

        }

        private void Edit(int index)
        {
            foreach (var obj in menuItems)
            {
                if (obj.index == index)
                {
                    MenuHandler handler = new MenuHandler(obj);
                    handler.Show();
                }
            }
        }

        public MenuItems AddRandomSubitem(string objType, int x, int y, int fontSize, string text, int tR, int tG, int tB)
        {
            if (objType == "rectangle")
            {
                Rectangle obj = _gameField.generateRandomRectangle();
                changePos(obj, x, y);
                resizeFrame(obj, 150, 50);

                return new MenuItems(x, y, fontSize, text, "Arial", tR, tG, tB, obj, currentIndex, _gameField);
            }
            else if (objType == "circle")
            {
                Circle obj = _gameField.generateRandomCircle();
                changePos(obj, x, y);
                resizeFrame(obj, 70, 70);

                return new MenuItems(x, y, fontSize, text, "Arial", tR, tG, tB, obj, currentIndex, _gameField);
            }
            else if (objType == "triangle")
            {
                Triangle obj = _gameField.generateRandomTriangle();
                changePos(obj, x, y);
                resizeFrame(obj, 70, 70);

                return new MenuItems(x, y, fontSize, text, "Arial", tR, tG, tB, obj, currentIndex, _gameField);
            }
            else if (objType == "square")
            {
                Square obj = _gameField.generateRandomSquare();
                changePos(obj, x, y);
                resizeFrame(obj, 70, 70);

                return new MenuItems(x, y, fontSize, text, "Arial", tR, tG, tB, obj, currentIndex, _gameField);
            }
            else if (objType == "ellipse")
            {
                Ellipse obj = _gameField.generateRandomEllipse();
                changePos(obj, x, y);
                resizeFrame(obj, 150, 50);

                return new MenuItems(x, y, fontSize, text, "Arial", tR, tG, tB, obj, currentIndex, _gameField);
            }

            return null;
        }

        public static string[] objTypes = new string[] { "rectangle", "ellipse", "square", "circle", "triangle" };

        private string GetObjType(DisplayObject displayObject)
        {
            if (displayObject is Square)
                return objTypes[2];
            if (displayObject is Rectangle)
                return objTypes[0];
            if (displayObject is Circle)
                return objTypes[3];
            if (displayObject is Ellipse)
                return objTypes[1];
            if (displayObject is Triangle)
                return objTypes[4];
            return null;
        }
        
        public MenuItems[] editItems = new MenuItems[3];

    }
}
