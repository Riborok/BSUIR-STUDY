using System;
using System.Threading.Tasks;

namespace Library;
using System.Drawing;

public class CubemapLoader {
    public static Cubemap LoadCubemap(string[] facePaths)
    {
        if (facePaths == null || facePaths.Length != 6)
            throw new ArgumentException("Необходимо передать массив из 6 путей к файлам кубической карты.");

        // Загружаем изображения
        Bitmap[] bitmaps = new Bitmap[6];
        for (int i = 0; i < 6; i++)
        {
            bitmaps[i] = new Bitmap(facePaths[i]);
        }

        // Проверяем, что все изображения одного размера
        int size = bitmaps[0].Width;
        foreach (var bmp in bitmaps)
        {
            if (bmp.Width != size || bmp.Height != size)
                throw new InvalidOperationException("Все грани кубической карты должны быть квадратными и иметь одинаковый размер.");
        }

        // Создаем Cubemap и заполняем пиксели
        Cubemap cube = new Cubemap(size);
        Parallel.For(0, 6, face =>
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    Color srcColor = bitmaps[face].GetPixel(x, y);
                    cube.Faces[face][x, y] = Color.FromArgb(srcColor.A, srcColor.R, srcColor.G, srcColor.B);
                }
            }
        });

        // Освобождаем ресурсы Bitmap
        foreach (var bmp in bitmaps)
        {
            bmp.Dispose();
        }

        return cube;
    }
}