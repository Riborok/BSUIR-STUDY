using Microsoft.VisualBasic;
using oop3.DisplayObjects;
using oop3.Menu;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace oop3.Utilities
{
    internal class TemplateGenerator
    {
        public enum Primitive
        {
            P_Rectangle,
            P_Square,
            P_Ellipse,
            P_Circle,
            P_Triangle
        }

        public enum SubMenuDirection { 
            Dir_Default,
            Dir_Horizontal,
            Dir_Vertical,
            Dir_Ladder
        }

        public Primitive activePrimitive;
        public int templateSizeX;
        public int templateSizeY;
        public Color viewFill;
        public Color viewStroke;
        public int viewStrokeThickness;

        public SubMenuDirection direction;  // auto
        public int offsetX;         // auto
        public int offsetY;         // auto

        public FontFamily fontFamily;   // not modifiable
        public Color fontColor;
        public int fontSize;
        public string textData;     // auto


        public TemplateGenerator(
            Primitive? primitive = null, int? sizeX = null, int? sizeY = null,
            Color? vFill = null, Color? vStroke = null, int? vStrokeTh = null,
            Color? fColor = null, int? fSize = null 
            )
        {
            activePrimitive = primitive == null ? Primitive.P_Rectangle : primitive.Value;
            templateSizeX = sizeX == null ? 100 : sizeX.Value;
            templateSizeY = sizeY == null ? 100 : sizeY.Value;
            viewFill = vFill == null? Color.Red: vFill.Value;
            viewStroke = vStroke == null ? Color.Black : vStroke.Value;
            viewStrokeThickness = vStrokeTh == null ? 1 : vStrokeTh.Value;

            direction = SubMenuDirection.Dir_Horizontal;
            offsetX = 0;
            offsetY = 0;

            fontFamily = FontFamily.GenericMonospace;
            fontColor = fColor == null ? Color.Black : fColor.Value;
            fontSize = fSize == null ? 14 : fSize.Value;
            textData = "";
        }
        public static string GetPrimitiveString(Primitive primitive)
        {
            string result = "Unknown";
            switch (primitive)
            {
                case Primitive.P_Rectangle:
                    result = "Rectangle";
                    break;
                case Primitive.P_Square:
                    result = "Square";
                    break;
                case Primitive.P_Ellipse:
                    result = "Ellipse";
                    break;
                case Primitive.P_Circle:
                    result = "Circle";
                    break;
                case Primitive.P_Triangle:
                    result = "Triangle";
                    break;
            }
            return result;
        }

        public static Menu.Menu GenerateMenu() {
            TemplateGenerator template = new TemplateGenerator(Primitive.P_Rectangle, 200, 100);

            int sizeX = 200;
            int sizeY = 100;
            template.templateSizeX = sizeX;
            template.templateSizeY = sizeY;
            template.offsetX = 20;
            template.offsetY = 10;


            template.activePrimitive = Primitive.P_Rectangle;
            template.direction = SubMenuDirection.Dir_Vertical;
            MenuItem item1 = template.GetTemplatePrimitive("File");
            template.direction = SubMenuDirection.Dir_Vertical;
            MenuItem item2 = template.GetTemplatePrimitive("Edit");
            template.direction = SubMenuDirection.Dir_Ladder;
            MenuItem item3 = template.GetTemplatePrimitive("View");

            template.activePrimitive = Primitive.P_Rectangle;
            template.viewFill = Color.Blue;
            MenuItem item11 = template.GetTemplatePrimitive("New");
            MenuItem item12 = template.GetTemplatePrimitive("Open");
            MenuItem item13 = template.GetTemplatePrimitive("Save");

            template.activePrimitive = Primitive.P_Ellipse;
            template.viewFill = Color.Cyan;
            MenuItem item21 = template.GetTemplatePrimitive("Copy");
            MenuItem item22 = template.GetTemplatePrimitive("Paste");
            MenuItem item23 = template.GetTemplatePrimitive("Edit");

            template.activePrimitive = Primitive.P_Rectangle;
            template.viewFill = Color.LightBlue;
            template.viewStroke = Color.DarkGray;
            template.viewStrokeThickness = 10;
            MenuItem item31 = template.GetTemplatePrimitive("Add");
            MenuItem item32 = template.GetTemplatePrimitive("Remove");
            MenuItem item33 = template.GetTemplatePrimitive("Minimize");

            template.activePrimitive = Primitive.P_Ellipse;
            template.viewFill = Color.DarkRed;
            template.fontColor = Color.White;
            template.viewStroke = Color.Black;
            template.viewStrokeThickness = 1;

            MenuItem item131 = template.GetTemplatePrimitive("Save as");
            MenuItem item132 = template.GetTemplatePrimitive("Save to");
            MenuItem item133 = template.GetTemplatePrimitive("Export");


            Menu.Menu testMenu = new Menu.Menu(100, 100);


            item133.clickHandler = () => { MessageBox.Show(item133.GetStringDisplay()); };
            item31.clickHandler = () => { 
                item2.AddItem(template.GetTemplatePrimitive("Test"));
                MessageBox.Show("Item added!");
            };
            item32.clickHandler = () => {
                if (item2.DeleteItem(item2.itemCount - 1)) { 
                    MessageBox.Show("Item removed!");
                }
            };
            item33.clickHandler = () => { MessageBox.Show(item33.GetStringDisplay()); };

            item13.AddItem(item131);
            item13.AddItem(item132);
            item13.AddItem(item133);

            item1.AddItem(item11);
            item1.AddItem(item12);
            item1.AddItem(item13);

            item2.AddItem(item21);
            item2.AddItem(item22);
            item2.AddItem(item23);

            item3.AddItem(item31);
            item3.AddItem(item32);
            item3.AddItem(item33);

            int offsX, offsY;
            template.direction = SubMenuDirection.Dir_Horizontal;
            template.offsetX = 30;
            (offsX, offsY) = template.GenerateOffsets();
            testMenu.offsetX = offsX;
            testMenu.offsetY = offsY;

            testMenu.AddItem(item1);
            testMenu.AddItem(item2);
            testMenu.AddItem(item3);

            return testMenu;
        }

        public static TemplateGenerator TemplateFromMenuItem(MenuItem item) {
            TemplateGenerator tg = new TemplateGenerator();
            DisplayObject display = item.view;
            TextObject text = item.text;
            tg.templateSizeX = (int)(display.frameX2 - display.frameX1);
            tg.templateSizeY = (int)(display.frameY2 - display.frameY1);
            tg.activePrimitive = GetPrimitive(display);
            tg.viewFill = display.fillColor;
            tg.viewStroke = display.strokeColor;
            tg.viewStrokeThickness = display.strokeThickness;
            tg.fontColor = text.fillColor;
            tg.fontSize = text.fontSize;
            tg.textData = text.textData;
            return tg;
        }
        private static Primitive GetPrimitive(DisplayObject display) {
            Primitive primitive = Primitive.P_Rectangle;
            if (display is RectangleObject)
                primitive = Primitive.P_Rectangle;
            else if (display is EllipseObject)
                primitive = Primitive.P_Ellipse;
            else if (display is TriangleObject)
                primitive = Primitive.P_Triangle;
            else if (display is CircleObject)
                primitive = Primitive.P_Circle;
            else if (display is SquareObject)
                primitive = Primitive.P_Square;
            return primitive;
        }

        private static void ItemEditFromTemplate(Menu.Menu itemEditor, TemplateGenerator template) {
            itemEditor.items[0].text.textData = $"Primitive type\n{GetPrimitiveString(template.activePrimitive)}";
            itemEditor.items[1].text.textData = $"Size\nW:{template.templateSizeX}\nH:{template.templateSizeY}";
            itemEditor.items[2].text.textData = $"Fill\n{template.viewFill.R}, {template.viewFill.G}, {template.viewFill.B}";
            itemEditor.items[3].text.textData = $"Stroke\n{template.viewStroke.R}, {template.viewStroke.G}, {template.viewStroke.B}";
            itemEditor.items[4].text.textData = $"Stroke thickness\n{template.viewStrokeThickness}";
            itemEditor.items[5].text.textData = $"Font color\n{template.fontColor.R}, {template.fontColor.G}, {template.fontColor.B}";
            itemEditor.items[6].text.textData = $"Font size\n{template.fontSize}";
            itemEditor.items[7].text.textData = $"Text data\n{template.textData}";
        }

        public static Menu.Menu GenItemEdit(TemplateGenerator itemTemplate, MenuItem item)
        {
            TemplateGenerator menuTemplate = new TemplateGenerator(Primitive.P_Rectangle, 160, 100, Color.Azure, null, null, null, 14);
            menuTemplate.offsetX = 20;
            menuTemplate.offsetY = 10;


            MenuItem PrimitiveType = menuTemplate.GetTemplatePrimitive();
            PrimitiveType.offsetX = 0;
            PrimitiveType.offsetY = -110;

            menuTemplate.activePrimitive = Primitive.P_Rectangle;
            MenuItem PrimitiveItem1 = menuTemplate.GetTemplatePrimitive("");
            PrimitiveItem1.clickHandler = () => {
                itemTemplate.activePrimitive = Primitive.P_Rectangle;
                DisplayObject v = itemTemplate.GetTemplatePrimitive().view;
                v.ShiftObject(item.view.frameX1 - v.frameX1, item.view.frameY1 - v.frameY1);
                item.view = v;
                item.frameX1 = v.frameX1;
                item.frameX2 = v.frameX2;
                item.frameY1 = v.frameY1;
                item.frameY2 = v.frameY2;
                PrimitiveType.text.textData = $"Primitive type\n{GetPrimitiveString(itemTemplate.activePrimitive)}";
            };
            PrimitiveType.AddItem(PrimitiveItem1);


            menuTemplate.activePrimitive = Primitive.P_Ellipse;
            MenuItem PrimitiveItem3 = menuTemplate.GetTemplatePrimitive("");
            PrimitiveItem3.clickHandler = () => {
                itemTemplate.activePrimitive = Primitive.P_Ellipse;
                DisplayObject v = itemTemplate.GetTemplatePrimitive().view;
                v.ShiftObject(item.view.frameX1 - v.frameX1, item.view.frameY1 - v.frameY1);
                item.view = v;
                item.frameX1 = v.frameX1;
                item.frameX2 = v.frameX2;
                item.frameY1 = v.frameY1;
                item.frameY2 = v.frameY2;
                PrimitiveType.text.textData = $"Primitive type\n{GetPrimitiveString(itemTemplate.activePrimitive)}";
            };
            PrimitiveType.AddItem(PrimitiveItem3);

            menuTemplate.activePrimitive = Primitive.P_Triangle;
            MenuItem PrimitiveItem5 = menuTemplate.GetTemplatePrimitive("");
            PrimitiveItem5.clickHandler = () => {
                itemTemplate.activePrimitive = Primitive.P_Triangle;
                DisplayObject v = itemTemplate.GetTemplatePrimitive().view;
                v.ShiftObject(item.view.frameX1 - v.frameX1, item.view.frameY1 - v.frameY1);
                item.view = v;
                item.frameX1 = v.frameX1;
                item.frameX2 = v.frameX2;
                item.frameY1 = v.frameY1;
                item.frameY2 = v.frameY2;
                PrimitiveType.text.textData = $"Primitive type\n{GetPrimitiveString(itemTemplate.activePrimitive)}";
            };
            PrimitiveType.AddItem(PrimitiveItem5);

            menuTemplate.activePrimitive = Primitive.P_Rectangle;



            MenuItem PrimitiveSize = menuTemplate.GetTemplatePrimitive();
            PrimitiveSize.clickHandler = () =>
            {
                int[]? data;
                if (GetInts("Enter shape size (width height)", 2, out data))
                {
                    if (data != null)
                    {
                        itemTemplate.templateSizeX = data[0];
                        itemTemplate.templateSizeY = data[1];
                        DisplayObject v = itemTemplate.GetTemplatePrimitive().view;
                        v.ShiftObject(item.view.frameX1 - v.frameX1, item.view.frameY1 - v.frameY1);
                        item.view = v;
                        item.frameX1 = v.frameX1;
                        item.frameX2 = v.frameX2;
                        item.frameY1 = v.frameY1;
                        item.frameY2 = v.frameY2;
                        TextObject t = itemTemplate.GetTemplatePrimitive().text;
                        t.ShiftObject(item.view.frameX1 - t.frameX1, item.view.frameY1 - t.frameY1);
                        item.text = t;

                        PrimitiveSize.text.textData = $"Size\nW:{data[0]}\nH:{data[1]}";
                    }
                    else
                    {
                        MessageBox.Show("Invalid data");
                    }
                }
            };
            MenuItem PrimitiveFill = menuTemplate.GetTemplatePrimitive();
            PrimitiveFill.view.fillColor = menuTemplate.viewFill;
            PrimitiveFill.clickHandler = () =>
            {
                int[]? data;
                if (GetInts("Enter fill color (R G B)", 3, out data))
                {
                    if (data != null)
                    {
                        itemTemplate.viewFill = Color.FromArgb(data[0], data[1], data[2]);
                        //DisplayObject v = itemTemplate.GetTemplatePrimitive().view;
                        //v.ShiftObject(item.view.frameX1 - v.frameX1, item.view.frameY1 - v.frameY1);
                        //item.view = v;
                        item.view.fillColor = itemTemplate.viewFill;
                        PrimitiveFill.text.textData = $"Fill\n{itemTemplate.viewFill.R}, {itemTemplate.viewFill.G}, {itemTemplate.viewFill.B}";
                    }
                    else
                    {
                        MessageBox.Show("Invalid data");
                    }
                }
            };
            MenuItem PrimitiveStroke = menuTemplate.GetTemplatePrimitive();
            PrimitiveStroke.fillColor = menuTemplate.viewStroke;
            PrimitiveStroke.clickHandler = () =>
            {
                int[]? data;
                if (GetInts("Enter stroke color (R G B)", 3, out data))
                {
                    if (data != null)
                    {
                        itemTemplate.viewStroke = Color.FromArgb(data[0], data[1], data[2]);
                        //DisplayObject v = itemTemplate.GetTemplatePrimitive().view;
                        //v.ShiftObject(item.view.frameX1 - v.frameX1, item.view.frameY1 - v.frameY1);
                        //item.view = v;
                        item.view.strokeColor = itemTemplate.viewStroke;
                        PrimitiveStroke.text.textData = $"Stroke\n{itemTemplate.viewStroke.R}, {itemTemplate.viewStroke.G}, {itemTemplate.viewStroke.B}";
                    }
                    else
                    {
                        MessageBox.Show("Invalid data");
                    }
                }
            };
            MenuItem PrimitiveStrokeThick = menuTemplate.GetTemplatePrimitive();
            PrimitiveStrokeThick.clickHandler = () =>
            {
                int[]? data;
                if (GetInts("Enter stroke thickness", 1, out data))
                {
                    if (data != null)
                    {
                        itemTemplate.viewStrokeThickness = data[0];
                        //DisplayObject v = itemTemplate.GetTemplatePrimitive().view;
                        //v.ShiftObject(item.view.frameX1 - v.frameX1, item.view.frameY1 - v.frameY1);
                        //item.view = v;
                        item.view.strokeThickness = itemTemplate.viewStrokeThickness;
                        PrimitiveStrokeThick.text.textData = $"Stroke thickness\n{data[0]}";
                    }
                    else
                    {
                        MessageBox.Show("Invalid data");
                    }
                }
            };
            MenuItem FontColor = menuTemplate.GetTemplatePrimitive();
            FontColor.view.fillColor = menuTemplate.viewFill;
            FontColor.clickHandler = () =>
            {
                int[]? data;
                if (GetInts("Enter font color (R G B)", 3, out data))
                {
                    if (data != null)
                    {
                        itemTemplate.fontColor = Color.FromArgb(data[0], data[1], data[2]);
                        TextObject t = itemTemplate.GetTemplatePrimitive().text;
                        t.ShiftObject(item.view.frameX1 - t.frameX1, item.view.frameY1 - t.frameY1);
                        item.text = t;
                        FontColor.text.textData = $"Font color\n{itemTemplate.fontColor.R}, {itemTemplate.fontColor.G}, {itemTemplate.fontColor.B}";
                    }
                    else
                    {
                        MessageBox.Show("Invalid data");
                    }
                }
            };
            MenuItem FontSize = menuTemplate.GetTemplatePrimitive();
            FontSize.clickHandler = () =>
            {
                int[]? data;
                if (GetInts("Enter font size", 1, out data))
                {
                    if (data != null)
                    {
                        itemTemplate.fontSize = data[0];
                        TextObject t = itemTemplate.GetTemplatePrimitive().text;
                        t.ShiftObject(item.view.frameX1 - t.frameX1, item.view.frameY1 - t.frameY1);
                        item.text = t;
                        FontSize.text.textData = $"Font size\n{data[0]}";
                    }
                    else
                    {
                        MessageBox.Show("Invalid data");
                    }
                }
            };
            MenuItem TextData = menuTemplate.GetTemplatePrimitive();
            TextData.clickHandler = () =>
            {
                itemTemplate.textData = Interaction.InputBox("Enter text data", "", itemTemplate.textData);
                TextObject t = itemTemplate.GetTemplatePrimitive().text;
                t.ShiftObject(item.view.frameX1 - t.frameX1, item.view.frameY1 - t.frameY1);
                item.text = t;
                TextData.text.textData = $"Text data\n{itemTemplate.textData}";
            };


            Menu.Menu menu = new Menu.Menu(50, 800);

            int offsx, offsy;
            menuTemplate.direction = SubMenuDirection.Dir_Horizontal;
            (offsx, offsy) = menuTemplate.GenerateOffsets();
            menu.offsetX = offsx;
            menu.offsetY = offsy;
/*
            menu.AddItem(AddToMenu);
            menu.AddItem(RemoveFromMenu);*/
            menu.AddItem(PrimitiveType);
            menu.AddItem(PrimitiveSize);
            menu.AddItem(PrimitiveFill);
            menu.AddItem(PrimitiveStroke);
            menu.AddItem(PrimitiveStrokeThick);
            menu.AddItem(FontColor);
            menu.AddItem(FontSize);
            menu.AddItem(TextData);

            menuTemplate.direction = SubMenuDirection.Dir_Vertical;
            menuTemplate.offsetX = 0;
            menuTemplate.offsetY = 100;

            ItemEditFromTemplate(menu, itemTemplate);
            return menu;
        }


        public static (Menu.Menu, TemplateGenerator template) GenMenuEdit(Menu.Menu editedMenu) {
            
            TemplateGenerator template = new TemplateGenerator(Primitive.P_Rectangle, 160, 100, Color.Azure,null,null,null,10);
            template.offsetX = 20;
            template.offsetY = 10;
            template.textData = "Test";

            MenuItem AddToMenu = template.GetTemplatePrimitive("Add item");
            AddToMenu.clickHandler = () =>
            {
                if (editedMenu.lastClickedItem != null)
                {
                    MenuItem item = editedMenu.lastClickedItem;
                    while (item.lastClickedItem != null) {
                        item = item.lastClickedItem;
                    }
                    item.AddItem(template.GetTemplatePrimitive());
                }
                else { 
                    editedMenu.AddItem(template.GetTemplatePrimitive());
                }
            };

            MenuItem RemoveFromMenu = template.GetTemplatePrimitive("Delete item");
            RemoveFromMenu.clickHandler = () =>
            {
                if (editedMenu.lastClickedItem != null)
                {
                    MenuItem item = editedMenu.lastClickedItem;
                    if (item.lastClickedItem != null)
                    {
                        while (item.lastClickedItem.lastClickedItem != null)
                        {
                            item = item.lastClickedItem;
                        }
                        MenuItem temp = item.lastClickedItem;
                        item.lastClickedItem = null;

                        item.DeleteItem(temp);
                    }
                    else {
                        editedMenu.lastClickedItem = null;
                        editedMenu.DeleteItem(item);
                    }
                }
                else {
                    MessageBox.Show("No item selected");
                }
            };


            MenuItem PrimitiveType = template.GetTemplatePrimitive($"Primitive type\n{GetPrimitiveString(template.activePrimitive)}");
            PrimitiveType.offsetX = 0;
            PrimitiveType.offsetY = -110;

            template.activePrimitive = Primitive.P_Rectangle;
            MenuItem PrimitiveItem1 = template.GetTemplatePrimitive("");
            PrimitiveItem1.clickHandler = () => { 
                template.activePrimitive = Primitive.P_Rectangle;
                PrimitiveType.text.textData = $"Primitive type\n{GetPrimitiveString(template.activePrimitive)}";
            };
            PrimitiveType.AddItem(PrimitiveItem1);


            template.activePrimitive = Primitive.P_Ellipse;
            MenuItem PrimitiveItem3 = template.GetTemplatePrimitive("");
            PrimitiveItem3.clickHandler = () => { 
                template.activePrimitive = Primitive.P_Ellipse;
                PrimitiveType.text.textData = $"Primitive type\n{GetPrimitiveString(template.activePrimitive)}";
            };
            PrimitiveType.AddItem(PrimitiveItem3);

            template.activePrimitive = Primitive.P_Triangle;
            MenuItem PrimitiveItem5 = template.GetTemplatePrimitive("");
            PrimitiveItem5.clickHandler = () => { 
                template.activePrimitive = Primitive.P_Triangle;
                PrimitiveType.text.textData = $"Primitive type\n{GetPrimitiveString(template.activePrimitive)}";
            };
            PrimitiveType.AddItem(PrimitiveItem5);
            
            template.activePrimitive = Primitive.P_Rectangle;



            MenuItem PrimitiveSize = template.GetTemplatePrimitive($"Size\nW:{template.templateSizeX}\nH:{template.templateSizeY}");
            PrimitiveSize.clickHandler = () =>
            {
                int[]? data;
                if (GetInts("Enter shape size (width height)", 2, out data))
                {
                    if (data != null) { 
                        template.templateSizeX = data[0];
                        template.templateSizeY = data[1];
                        PrimitiveSize.text.textData = $"Size\nW:{data[0]}\nH:{data[1]}";
                    }
                    else
                    {
                        MessageBox.Show("Invalid data");
                    }
                }
            };
            MenuItem PrimitiveFill = template.GetTemplatePrimitive($"Fill\n{template.viewFill.R}, {template.viewFill.G}, {template.viewFill.B}");
            PrimitiveFill.view.fillColor = template.viewFill;
            PrimitiveFill.clickHandler = () =>
            {
                int[]? data;
                if(GetInts("Enter fill color (R G B)", 3, out data))
                {
                    if (data != null) { 
                        template.viewFill = Color.FromArgb(data[0], data[1], data[2]);
                        PrimitiveFill.text.textData = $"Fill\n{template.viewFill.R}, {template.viewFill.G}, {template.viewFill.B}"; ;
                    }
                    else {
                        MessageBox.Show("Invalid data");
                    }
                }
            };
            MenuItem PrimitiveStroke = template.GetTemplatePrimitive($"Stroke\n{template.viewStroke.R}, {template.viewStroke.G}, {template.viewStroke.B}");
            PrimitiveStroke.fillColor = template.viewStroke;
            PrimitiveStroke.clickHandler = () =>
            {
                int[]? data; 
                if( GetInts("Enter stroke color (R G B)", 3, out data)) 
                {
                    if (data != null) { 
                        template.viewStroke = Color.FromArgb(data[0], data[1], data[2]);
                        PrimitiveStroke.text.textData = $"Stroke\n{template.viewStroke.R}, {template.viewStroke.G}, {template.viewStroke.B}";
                    }
                    else
                    {
                        MessageBox.Show("Invalid data");
                    }
                }
            };
            MenuItem PrimitiveStrokeThick = template.GetTemplatePrimitive($"Stroke Thickness\n{template.viewStrokeThickness}");
            PrimitiveStrokeThick.clickHandler = () =>
            {
                int[]? data;
                if (GetInts("Enter stroke thickness", 1, out data))
                {
                    if (data != null)
                    {
                        template.viewStrokeThickness = data[0];
                        PrimitiveStrokeThick.text.textData = $"Stroke thickness\n{data[0]}";
                    }
                    else
                    {
                        MessageBox.Show("Invalid data");
                    }
                }
            };
            MenuItem FontColor = template.GetTemplatePrimitive($"Font color\n{template.fontColor.R}, {template.fontColor.G}, {template.fontColor.B}");
            FontColor.view.fillColor = template.viewFill;
            FontColor.clickHandler = () =>
            {
                int[]? data;
                if(GetInts("Enter font color (R G B)", 3, out data))
                {
                    if (data != null) { 
                        template.fontColor = Color.FromArgb(data[0], data[1], data[2]);
                        FontColor.text.textData = $"Font color\n{template.fontColor.R}, {template.fontColor.G}, {template.fontColor.B}";
                    }
                    else
                    {
                        MessageBox.Show("Invalid data");
                    }
                }
            };
            MenuItem FontSize = template.GetTemplatePrimitive($"Font size\n{template.fontSize}");
            FontSize.clickHandler = () =>
            {
                int[]? data;
                if(GetInts("Enter font size", 1, out data))
                {
                    if (data != null) { 
                        template.fontSize = data[0];
                        FontSize.text.textData = $"Font size\n{data[0]}";
                    }
                    else
                    {
                        MessageBox.Show("Invalid data");
                    }
                }
            };
            MenuItem TextData = template.GetTemplatePrimitive($"Text data\n{template.textData}");
            TextData.clickHandler = () =>
            {
                template.textData = Interaction.InputBox("Enter text data","", template.textData);
                TextData.text.textData = $"Text data\n{template.textData}";
            };


            Menu.Menu menu = new Menu.Menu(50,800);

            int offsx, offsy;
            template.direction = SubMenuDirection.Dir_Horizontal;
            (offsx,offsy)=template.GenerateOffsets();
            menu.offsetX = offsx;
            menu.offsetY = offsy;

            menu.AddItem(AddToMenu);
            menu.AddItem(RemoveFromMenu);
            menu.AddItem(PrimitiveType);
            menu.AddItem(PrimitiveSize);
            menu.AddItem(PrimitiveFill);
            menu.AddItem(PrimitiveStroke);
            menu.AddItem(PrimitiveStrokeThick);
            menu.AddItem(FontColor);
            menu.AddItem(FontSize);
            menu.AddItem(TextData);

            template.direction = SubMenuDirection.Dir_Horizontal;
            template.offsetX = 30;
            template.offsetY = 30;
            (offsx,offsy)=template.GenerateOffsets();
            editedMenu.offsetX = offsx;
            editedMenu.offsetY = offsy;

            template.direction = SubMenuDirection.Dir_Vertical;
            template.offsetX = 0;
            template.offsetY = 100;

            return (menu, template);
        }

        public static bool GetInts(string title, int intCount, out int[]? result)
        {
            result = new int[intCount];
            string res = Interaction.InputBox(title);
            if (res == "") {
                return false;
            }
            string[] arr = res.Split(" ");
            if (arr.Length != intCount)
            {
                result = null;
                return true;
            }
            else
            {
                bool valid;
                valid = true;
                int i = 0;
                while (valid && i < intCount)
                {
                    valid = int.TryParse(arr[i], out result[i]);
                    i++;
                }
                if (!valid) {
                    result = null; 
                }
                return true;
            }
        }

        public (int, int) GenerateOffsets() {
            int offsX = 0, offsY = 0;
            switch (direction)
            {
                case (SubMenuDirection.Dir_Vertical):
                    offsX = 0;
                    offsY = templateSizeY + offsetY;
                    break;

                case (SubMenuDirection.Dir_Horizontal):
                    offsX = templateSizeX + offsetX;
                    offsY = 0;
                    break;

                case (SubMenuDirection.Dir_Ladder):
                    offsX = templateSizeX + offsetX;
                    offsY = templateSizeY + offsetY;
                    break;

                case (SubMenuDirection.Dir_Default):
                default:
                    offsX = offsetX;
                    offsY = offsetY;
                    break;
            }
            return (offsX, offsY);
        }

        public MenuItem GetTemplatePrimitive(string? textData = null)
        {
            textData = textData == null ? this.textData : textData;
            int frameX1 = - (templateSizeX / 2);
            int frameY1 = - (templateSizeY / 2);
            int frameX2 = (templateSizeX / 2);
            int frameY2 = (templateSizeY / 2);

            DisplayObject displayObject;
            TextObject text = new TextObject(textData, fontFamily, fontSize, fontColor,
                frameX1, frameY1, frameX2, frameY2
                );
            switch (activePrimitive)
            {
                case Primitive.P_Rectangle:
                    displayObject = new RectangleObject(frameX1, frameY1, frameX2, frameY2,viewFill);
                    break;
                case Primitive.P_Square:
                    displayObject = new SquareObject(frameX1,frameY1,templateSizeX, viewFill);
                    break;
                case Primitive.P_Ellipse:
                    displayObject = new EllipseObject(0, 0, templateSizeX/2,templateSizeY/2, viewFill);
                    break;
                case Primitive.P_Circle:
                    displayObject = new CircleObject(0, 0, templateSizeX/2, viewFill);
                    break;
                case Primitive.P_Triangle:
                    displayObject = new TriangleObject(frameX1, frameY1, templateSizeX, templateSizeY, viewFill);
                    break;
                default:
                    displayObject = new RectangleObject(frameX1, frameY1, frameX2, frameY2, viewFill);
                    break;
            }
            displayObject.SetStrokeThickness(viewStrokeThickness);
            displayObject.strokeColor = viewStroke;

            MenuItem item = new MenuItem(displayObject, text);

            int offsX, offsY;
            (offsX, offsY) = GenerateOffsets();
            item.offsetX = offsX;
            item.offsetY = offsY;

            return item;
        }

    }
}
