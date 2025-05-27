using System.Drawing.Imaging;

namespace Library;

using System;
using System.Drawing;
using System.Numerics;

public class TextureMap {
    private readonly int[] _pixels;
    private readonly int _stride;
    
    public TextureMap(string path) {
        using var bitmap = new Bitmap(path);
        Width = bitmap.Width;
        Height = bitmap.Height;
        _stride = Width;
        _pixels = new int[Width * Height];

        var bitmapData = bitmap.LockBits(
            new Rectangle(0, 0, Width, Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb);

        try
        {
            unsafe
            {
                int* ptr = (int*)bitmapData.Scan0;
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        _pixels[y * _stride + x] = ptr[y * (bitmapData.Stride / 4) + x];
                    }
                }
            }
        }
        finally
        {
            bitmap.UnlockBits(bitmapData);
        }
    }

    
    public int Width { get; }
    public int Height { get; }

    private Color GetPixel(int x, int y)
    {
        return System.Drawing.Color.FromArgb(_pixels[y * _stride + x]);
    }

    public Color Color(float u, float v)
    {
        u = MathUtils.Clamp(u, 0f, 1f);
        v = MathUtils.Clamp(v, 0f, 1f);
        float x = u * (Width - 1);
        float y = (1 - v) * (Height - 1);

        int x0 = (int)Math.Floor(x);
        int y0 = (int)Math.Floor(y);
        int x1 = Math.Min(x0 + 1, Width - 1);
        int y1 = Math.Min(y0 + 1, Height - 1);

        float dx = x - x0;
        float dy = y - y0;

        Color c00 = GetPixel(x0, y0);
        Color c10 = GetPixel(x1, y0);
        Color c01 = GetPixel(x0, y1);
        Color c11 = GetPixel(x1, y1);

        float r = Bilinear(c00.R, c10.R, c01.R, c11.R, dx, dy);
        float g = Bilinear(c00.G, c10.G, c01.G, c11.G, dx, dy);
        float b = Bilinear(c00.B, c10.B, c01.B, c11.B, dx, dy);

        return System.Drawing.Color.FromArgb(ClampByte(r), ClampByte(g), ClampByte(b));
    }

    public Vector3 Normal(float u, float v)
    {
        Color color = Color(u, v);
        float nx = color.R / 255f * 2f - 1f;
        float ny = color.G / 255f * 2f - 1f;
        float nz = color.B / 255f * 2f - 1f;
        return Vector3.Normalize(new Vector3(nx, ny, nz));
    }

    public float Specular(float u, float v)
    {
        Color color = Color(u, v);
        return (color.R + color.G + color.B) / (3f * 255f);
    }

    private float Bilinear(float c00, float c10, float c01, float c11, float dx, float dy)
    {
        float top = c00 * (1 - dx) + c10 * dx;
        float bottom = c01 * (1 - dx) + c11 * dx;
        return top * (1 - dy) + bottom * dy;
    }

    private byte ClampByte(float value) => (byte)MathUtils.Clamp((int)Math.Round(value), 0, 255);
}
