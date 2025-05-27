using System;
using System.Drawing;

namespace Library;

public class Cubemap 
{
    // Faces[0] = +X, [1] = -X, [2] = +Y, [3] = -Y, [4] = +Z, [5] = -Z
    public Color[][,] Faces = new Color[6][,];

    public int Size { get; }

    public Cubemap(int faceSize)
    {
        Size = faceSize;
        for (int i = 0; i < 6; i++)
            Faces[i] = new Color[Size, Size];
    }
    
    public Color GetPixel(CubeFace face, int x, int y) {
        x = MathUtils.Clamp(x, 0, Size - 1);
        y = MathUtils.Clamp(y, 0, Size - 1);
        return Faces[(int)face][x, y];
    }
}