using System;
using System.Collections.Generic;

namespace Lab1
{
    internal static class TableCreator
    {
        private const int ColumnsCount = 3;
        private const int SeparatorsCount = 4;
        
        private const int CellWidth = 20;
        private const string RowFormat = "|{0,-20}|{1,-20}|{2,-20}|\n";
        
        public static void DrawTable(IEnumerable<Node> nodes)
        {
            DrawHeader();
            foreach (var node in nodes)
                DrawRow(node);
        }
        
        public static void DrawHeader()
        {
            DrawSplitter();
            Console.Write(FormatRow("IP", "MAC", "Type"));
        }

        private static void DrawSplitter()
        {
            for (int i = 0; i < CellWidth * ColumnsCount + SeparatorsCount; i++)
                Console.Write("-");
            Console.WriteLine();
        }

        public static void DrawRow(Node node)
        {
            Console.Write(FormatRow(node.Ip, node.Mac, node.Name));
        }
        
        private static string FormatRow(string ip, string mac, string name)
        {
            return string.Format(RowFormat, AlignTextCenter(ip), AlignTextCenter(mac), AlignTextCenter(name));
        }
        
        private static string AlignTextCenter(string s)
        {
            return string.Format("{0," + (CellWidth + s.Length) / 2 + "}", s);
        }
    }
}