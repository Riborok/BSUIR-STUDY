using System.Numerics;
using System.Security.Cryptography;
// ReSharper disable InconsistentNaming

namespace RSADigitalSignature
{
    public static class MillerRabin
    {
        private const int k = 6;
        
        public static bool MillerRabinTest(BigInteger n)
        {
            if (n == 2 || n == 3)
                return true;

            if (n < 2 || n % 2 == 0)
                return false;

            ComputeTAndS(n, out var t, out int s);

            for (var i = 0; i < k; i++)
            {
                var a = SelectRandomA(n);
                var x = BigInteger.ModPow(a, t, n);

                if (x == 1 || x == n - 1)
                    continue;

                if (!CheckMillerRabinInnerLoop(x, n, s))
                    return false;
            }

            return true;
        }

        private static void ComputeTAndS(BigInteger n, out BigInteger t, out int s)
        {
            t = n - 1;
            s = 0;

            while (t % 2 == 0)
            {
                t /= 2;
                s += 1;
            }
        }

        private static BigInteger SelectRandomA(BigInteger n)
        {
            var rng = new RNGCryptoServiceProvider();
            var _a = new byte[n.ToByteArray().LongLength];
            BigInteger a;

            do
            {
                rng.GetBytes(_a);
                a = new BigInteger(_a);
            }
            while (a < 2 || a >= n - 2);

            return a;
        }

        private static bool CheckMillerRabinInnerLoop(BigInteger x, BigInteger n, int s)
        {
            for (var r = 1; r < s; r++)
            {
                x = BigInteger.ModPow(x, 2, n);

                if (x == 1)
                    return false;

                if (x == n - 1)
                    break;
            }

            return x == n - 1;
        }
    }
}