using oop3.DisplayObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oop3.Menu
{
    class MenuItem : DisplayObject
    {
        public DisplayObject view;
        public TextObject text;

        public int offsetX;
        public int offsetY;

        private bool _subMenuVisible;
        public bool subMenuVisible {
            get { 
                return _subMenuVisible;
            }
            set {
                if (!value) {
                    HideSubMenu();
                }
                _subMenuVisible = value;
            } 
        }

        protected int capacity;
        public int itemCount;
        public MenuItem[] subMenuItems;
        public MenuItem lastClickedItem;

        public MenuItem(DisplayObject viewObj, TextObject textObj) :
            base((int)viewObj.anchorX, (int)viewObj.anchorY, (int)(viewObj.frameX2 - viewObj.frameX1), viewObj.frameY2 - viewObj.frameY1, Color.Black)
        {
            view = viewObj;
            text = textObj;

            offsetX = 0;
            offsetY = 100;

            _subMenuVisible = false;

            capacity = 10;
            itemCount = 0;
            subMenuItems = new MenuItem[capacity];

            clickHandler = () => { Debug.WriteLine(text.textData); };
        }
        public override bool Click(int mouseX, int mouseY)
        {
            int i = itemCount - 1;
            bool clickHandled = false;
            // check the children first, as they're drawn later
            if (subMenuVisible) { 
                while (!clickHandled && i >= 0)
                {
                    clickHandled = subMenuItems[i].Click(mouseX, mouseY);
                    i--;
                }            
            }

            if (clickHandled)
            {
                i++;
                for (int j = 0; j < itemCount; j++)
                {
                    if (i == j) continue;
                    //if (items[j].subMenuVisible)
                    //{
                    subMenuItems[j].subMenuVisible = false;
                    //}
                }
                lastClickedItem = subMenuItems[i];
            }
            else
            {
                lastClickedItem = null;
                clickHandled = base.Click(mouseX, mouseY);
                if (clickHandled)
                {
                    subMenuVisible = !subMenuVisible;
                }
                else {
                    subMenuVisible = false;
                }
            }

            return clickHandled;
        }

        public void HideSubMenu() {
            for (int i = 0; i < itemCount; i++) {
                MenuItem currItem = subMenuItems[i];
                currItem.subMenuVisible = false;
            }
        }

        public int? AddItem(MenuItem item)
        {
            if (itemCount != capacity)
            {
                // add the item, including its offset
                // shift the coordinates according to the last item
                double topLeftX = view.frameX1 + offsetX;
                double topLeftY = view.frameY1 + offsetY;

                if (itemCount != 0)
                {
                    topLeftX = subMenuItems[itemCount - 1].frameX1 + offsetX;
                    topLeftY = subMenuItems[itemCount - 1].frameY1 + offsetY;
                }

                // shift coords according to the new point
                item.ShiftObject(topLeftX- item.frameX1, topLeftY- item.frameY1);

                subMenuItems[itemCount] = item;
                return itemCount++;
            }
            return null;
        }

        public bool DeleteItem(int index)
        {
            if (index >= 0 && index < itemCount)
            {
                for (int i = index; i < itemCount - 1; i++)
                {
                    subMenuItems[i] = subMenuItems[i + 1];
                }
                itemCount--;
                return true;
            }
            return false;

        }
        public bool DeleteItem(MenuItem obj)
        {
            int i = 0;
            bool flagFound = false;
            while (i < itemCount && !flagFound)
            {
                flagFound = subMenuItems[i] == obj;
                i++;
            }
            if (flagFound)
            {
                i--;
                return DeleteItem(i);
            }
            else
            {
                return false;
            }
        }

        public string GetStringDisplay() {
            return text.textData;
        }

        public override void Draw(Graphics g)
        {
            // the drawing occurs relative to the menu
            view.Draw(g);
            text.Draw(g);
            if (subMenuVisible) { 
                for (int i = 0; i < itemCount; i++)
                {
                    subMenuItems[i].Draw(g);
                }            
            }
        }

        protected override void ShiftCoords(double deltaX, double deltaY)
        {
            view.ShiftObject(deltaX, deltaY);
            text.ShiftObject(deltaX, deltaY);
            frameX1 = view.frameX1; frameX2 = view.frameX2;
            frameY1 = view.frameY1; frameY2 = view.frameY2;
            
            for(int i=0; i < itemCount; i++)
            {
                subMenuItems[i].ShiftObject(deltaX, deltaY);
            }
        }

        protected override void UpdateFrame()
        {
            view.SetRotationAngle(rotAngle);
            text.SetRotationAngle(rotAngle);
            frameX1 = view.frameX1; frameX2 = view.frameX2;
            frameY1 = view.frameY1; frameY2 = view.frameY2;
        }
    }
}
