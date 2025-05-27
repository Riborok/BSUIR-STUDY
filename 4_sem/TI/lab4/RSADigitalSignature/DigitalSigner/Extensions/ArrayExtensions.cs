using System;
using System.Linq;
using System.Text;

namespace DigitalSigner.Extensions {
    public static class ArrayExtensions {
        private static readonly Encoding Encoding = Encoding.GetEncoding("Windows-1251");
        public static T GetLast<T>(this T[] array, T defaultValue) {
            var length = array.Length - 1;
            return length >= 0 ? array[length] : defaultValue;
        }

        public static void SetLast<T>(this T[] array, T value) => array[array.Length - 1] = value;

        public static T[] Append<T>(this T[] array, T value) {
            var newArray = new T[array.Length + 1];
            Array.Copy(array, newArray, array.Length);
            newArray[array.Length] = value;
            return newArray;
        }
        
        public static byte[] ToByteArray(this string[] lines) => ToByteArray(lines, lines.Length);
        
        public static byte[] ToByteArray(this string[] lines, int count) {
            string text = string.Join(Environment.NewLine, lines.Take(count));
            byte[] bytes = Encoding.GetBytes(text);
            return bytes;
        }
    }
}