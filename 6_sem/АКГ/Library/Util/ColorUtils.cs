using System.Drawing;
using System.Numerics;

namespace Library;

public static class ColorUtils {
    public static Vector3 ToVector3(this Color color)
    {
        return new Vector3(color.R, color.G, color.B);
    }

}