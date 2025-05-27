using System.Numerics;

namespace RSADigitalSignature
{
    public static class MathUtils
    {
        public static BigInteger FastModularExponentiation(BigInteger a, BigInteger z, BigInteger n)
        {
            BigInteger a1 = a;
            BigInteger z1 = z;
            BigInteger x = 1;

            while (z1 != 0)
            {
                while (z1 % 2 == 0)
                {
                    z1 /= 2;
                    a1 = (a1 * a1) % n;
                }

                z1--;
                x = (x * a1) % n;
            }

            return x;
        }
        
        public static (BigInteger x, BigInteger y) ExtendedEuclideanAlgorithm(BigInteger a, BigInteger b)
        {
            BigInteger d0 = a, d1 = b;
            BigInteger x0 = 1, x1 = 0;
            BigInteger y0 = 0, y1 = 1;

            while (d1 > 1)
            {
                var q = d0 / d1;
                var d2 = d0 % d1;
                var x2 = x0 - q * x1;
                var y2 = y0 - q * y1;
                d0 = d1;
                d1 = d2;
                x0 = x1;
                x1 = x2;
                y0 = y1;
                y1 = y2;
            }

            return (x1, y1);
        }
        
        public static bool AreCoprime(BigInteger a, BigInteger b)
        {
            return BigInteger.GreatestCommonDivisor(a, b) == 1;
        }
    }
}