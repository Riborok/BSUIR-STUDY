using System;
using System.Drawing;
using System.Numerics;

namespace Library;

public static class CubemapSampler
{
    public static Color Sample(Cubemap cube, Vector3 dir)
    {
        dir = Vector3.Normalize(dir);
        float absX = Math.Abs(dir.X);
        float absY = Math.Abs(dir.Y);
        float absZ = Math.Abs(dir.Z);

        CubeFace face;
        float u, v, max;

        if (absX >= absY && absX >= absZ)
        {
            max = absX;
            if (dir.X > 0) { face = CubeFace.PositiveX; u = -dir.Z; v = dir.Y; }
            else            { face = CubeFace.NegativeX; u = dir.Z;  v = dir.Y; }
        }
        else if (absY >= absX && absY >= absZ)
        {
            max = absY;
            if (dir.Y > 0) { face = CubeFace.PositiveY; u = dir.X;  v = dir.Z; }
            else            { face = CubeFace.NegativeY; u = dir.X;  v = -dir.Z; }
        }
        else
        {
            max = absZ;
            if (dir.Z > 0) { face = CubeFace.PositiveZ; u = dir.X;  v = dir.Y; }
            else            { face = CubeFace.NegativeZ; u = -dir.X; v = dir.Y; }
        }
        
        u = 0.5f * (u / max + 1f);
        v = 0.5f * (v / max + 1f);
        int px = (int)(u * (cube.Size - 1));
        int py = (int)((1 - v) * (cube.Size - 1));

        return cube.GetPixel(face, px, py);
    }
}