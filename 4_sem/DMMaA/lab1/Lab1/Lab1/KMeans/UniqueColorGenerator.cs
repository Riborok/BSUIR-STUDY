using System;
using System.Drawing;

namespace Lab1.KMeans
{
    public class UniqueColorGenerator
    {
        private const double GoldenRatioConjugate = 0.618033988749895;
        private double _hue = 0.5;

        public Color GenerateUniqueColor()
        {
            double hue = GenerateNextHue();
            return GenerateColorFromHsv(hue, 0.5, 0.95);
        }

        private double GenerateNextHue()
        {
            _hue += GoldenRatioConjugate;
            _hue %= 1;
            return _hue;
        }

        private static Color GenerateColorFromHsv(double hue, double saturation, double value)
        {
            int hi = (int)Math.Floor(hue * 6);
            double f = hue * 6 - hi;
            HsvComponents hsv = new HsvComponents { Value = value, Saturation = saturation };
            RgbComponents components = GetRgbComponents(hi, hsv, f);

            return Color.FromArgb((int)(components.R * 255), (int)(components.G * 255), (int)(components.B * 255));
        }

        private static RgbComponents GetRgbComponents(int hi, in HsvComponents hsv, double f)
        {
            double p = CalculateP(hsv);
            double q = CalculateQ(hsv, f);
            double t = CalculateT(hsv, f);

            double r, g, b;
            switch (hi)
            {
                case 0:
                    r = hsv.Value;
                    g = t;
                    b = p;
                    break;
                case 1:
                    r = q;
                    g = hsv.Value;
                    b = p;
                    break;
                case 2:
                    r = p;
                    g = hsv.Value;
                    b = t;
                    break;
                case 3:
                    r = p;
                    g = q;
                    b = hsv.Value;
                    break;
                case 4:
                    r = t;
                    g = p;
                    b = hsv.Value;
                    break;
                default:
                    r = hsv.Value;
                    g = p;
                    b = q;
                    break;
            }

            return new RgbComponents { R = r, G = g, B = b };
        }

        private static double CalculateP(in HsvComponents hsv)
        {
            return hsv.Value * (1 - hsv.Saturation);
        }

        private static double CalculateQ(in HsvComponents hsv, double f)
        {
            return hsv.Value * (1 - f * hsv.Saturation);
        }

        private static double CalculateT(in HsvComponents hsv, double f)
        {
            return hsv.Value * (1 - (1 - f) * hsv.Saturation);
        }
        
        private struct RgbComponents
        {
            public double R;
            public double G;
            public double B;
        }

        private struct HsvComponents
        {
            public double Value;
            public double Saturation;
        }
    }
}