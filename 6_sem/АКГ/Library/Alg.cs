using System;
using System.Numerics;

namespace Library;

public static class Alg {
    public static void BresenhamLine(Vector2 a, Vector2 b, Action<int, int> setPixel)
    {
        int x1 = (int)Math.Round(a.X, MidpointRounding.AwayFromZero);
        int y1 = (int)Math.Round(a.Y, MidpointRounding.AwayFromZero);
        int x2 = (int)Math.Round(b.X, MidpointRounding.AwayFromZero);
        int y2 = (int)Math.Round(b.Y, MidpointRounding.AwayFromZero);

        int dx = x2 - x1;
        int dy = y2 - y1;
        int w = Math.Abs(dx);
        int h = Math.Abs(dy);
        int l = Math.Max(w, h);
        int m11 = Math.Sign(dx);
        int m12 = 0;
        int m21 = 0;
        int m22 = Math.Sign(dy);

        if (w < h)
        {
            (m11, m12) = (m12, m11);
            (m21, m22) = (m22, m21);
        }

        int y = 0;
        int e = 0;
        int eDec = 2 * l;
        int eInc = 2 * Math.Min(w, h);

        for (int x = 0; x <= l; x++)
        {
            int xt = x1 + m11 * x + m12 * y;
            int yt = y1 + m21 * x + m22 * y;
            setPixel(xt, yt);

            if ((e += eInc) > l)
            {
                e -= eDec;
                y++;
            }
        }
    }
    public struct VertexData(Vector3 pos3D, Vector3 normal, Vector3 world, Vector2 uv, float oneOverW) {
        public Vector2 Position = new(pos3D.X, pos3D.Y);
        public Vector3 Normal = normal;
        public Vector3 World = world;
        public Vector2 UV = uv;
        public float OneOverW = oneOverW;
    }

    public static void ScanlineTriangle(
        (int width, int height) s,
        VertexData v1, VertexData v2, VertexData v3,
        Action<int, int, Vector3, Vector3, Vector2> setPixel)
    {
        if (v1.Position.Y > v3.Position.Y) (v1, v3) = (v3, v1);
        if (v1.Position.Y > v2.Position.Y) (v1, v2) = (v2, v1);
        if (v2.Position.Y > v3.Position.Y) (v2, v3) = (v3, v2);

        var p1 = v1.Position;  var p2 = v2.Position;  var p3 = v3.Position;
        var n1 = v1.Normal;    var n2 = v2.Normal;    var n3 = v3.Normal;
        var w1 = v1.World;     var w2 = v2.World;     var w3 = v3.World;
        var uv1= v1.UV;        var uv2= v2.UV;        var uv3= v3.UV;
        var o1 = v1.OneOverW;     var o2 = v2.OneOverW;     var o3 = v3.OneOverW;

        float dy13 = p3.Y - p1.Y;
        float dy12 = p2.Y - p1.Y;
        float dy23 = p3.Y - p2.Y;

        Vector2 d13 = (p3 - p1) / dy13;
        Vector2 d12 = (p2 - p1) / dy12;
        Vector2 d23 = (p3 - p2) / dy23;

        Vector3 dn13 = (n3 - n1) / dy13;
        Vector3 dn12 = (n2 - n1) / dy12;
        Vector3 dn23 = (n3 - n2) / dy23;

        Vector3 dw13 = (w3 - w1) / dy13;
        Vector3 dw12 = (w2 - w1) / dy12;
        Vector3 dw23 = (w3 - w2) / dy23;

        Vector2 uvd1  = uv1 * o1;
        Vector2 uvd2  = uv2 * o2;
        Vector2 uvd3  = uv3 * o3;

        Vector2 duvd13 = (uvd3 - uvd1) / dy13;
        Vector2 duvd12 = (uvd2 - uvd1) / dy12;
        Vector2 duvd23 = (uvd3 - uvd2) / dy23;

        float doz13 = (o3 - o1) / dy13;
        float doz12 = (o2 - o1) / dy12;
        float doz23 = (o3 - o2) / dy23;

        int yStart = Math.Max(0, (int)Math.Ceiling(p1.Y));
        int yEnd   = Math.Min(s.height, (int)Math.Ceiling(p3.Y));

        for (int y = yStart; y < yEnd; y++)
        {
            bool top = y < p2.Y;
            float ya1 = y - p1.Y;
            float ya2 = y - (top ? p1.Y : p2.Y);

            Vector2 xa   = p1 + d13 * ya1;
            Vector3 na   = n1 + dn13 * ya1;
            Vector3 wa   = w1 + dw13 * ya1;
            Vector2 uvaD = uvd1 + duvd13 * ya1;
            float   oa   = o1 + doz13 * ya1;

            Vector2 xb; Vector3 nb, wb; Vector2 uvbD; float ob;
            if (top)
            {
                xb   = p1 + d12 * ya1;
                nb   = n1 + dn12 * ya1;
                wb   = w1 + dw12 * ya1;
                uvbD = uvd1 + duvd12 * ya1;
                ob   = o1 + doz12 * ya1;
            }
            else
            {
                xb   = p2 + d23 * ya2;
                nb   = n2 + dn23 * ya2;
                wb   = w2 + dw23 * ya2;
                uvbD = uvd2 + duvd23 * ya2;
                ob   = o2 + doz23 * ya2;
            }

            if (xa.X > xb.X)
            {
                (xa, xb)     = (xb, xa);
                (na, nb)     = (nb, na);
                (wa, wb)     = (wb, wa);
                (uvaD, uvbD) = (uvbD, uvaD);
                (oa, ob)     = (ob, oa);
            }

            float dx = xb.X - xa.X;
            Vector3 dn   = (nb - na) / dx;
            Vector3 dw   = (wb - wa) / dx;
            Vector2 duvd = (uvbD - uvaD) / dx;
            float   doz  = (ob - oa) / dx;

            int xStart = Math.Max(0, (int)Math.Ceiling(xa.X));
            int xEnd   = Math.Min(s.width, (int)Math.Ceiling(xb.X));

            for (int x = xStart; x < xEnd; x++)
            {
                float t = x - xa.X;
                var N = na + dn * t;
                var W = wa + dw * t;
                var uvd = uvaD + duvd * t;
                var oinv = oa + doz * t;

                Vector2 uvCorrected = uvd / oinv;

                setPixel(x, y, N, W, uvCorrected);
            }
        }
    }
    
    public static float GetZ(float x, float y, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        float x1 = v1.X, y1 = v1.Y, z1 = v1.Z;
        float x2 = v2.X, y2 = v2.Y, z2 = v2.Z;
        float x3 = v3.X, y3 = v3.Y, z3 = v3.Z;

        float denominator = (y2 - y3) * (x1 - x3) + (x3 - x2) * (y1 - y3);

        float lambda1 = ((y2 - y3) * (x - x3) + (x3 - x2) * (y - y3)) / denominator;
        float lambda2 = ((y3 - y1) * (x - x3) + (x1 - x3) * (y - y3)) / denominator;
        float lambda3 = 1 - lambda1 - lambda2;

        return lambda1 * z1 + lambda2 * z2 + lambda3 * z3;
    }
}