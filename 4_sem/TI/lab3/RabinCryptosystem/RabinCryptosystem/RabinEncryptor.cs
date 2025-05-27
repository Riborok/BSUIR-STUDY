// ReSharper disable PossibleLossOfFraction InconsistentNaming SuggestBaseTypeForParameter ReturnTypeCanBeEnumerable.Local ParameterTypeCanBeEnumerable.Local
using System;
using System.Linq;
using System.Numerics;
using System.Text;

namespace RabinCryptosystem
{
    public static class RabinEncryptor
    {
        private const char Separator = ' '; 
        
        public static string Encrypt(BigInteger n, BigInteger b, byte[] m)
        {
            var c = new StringBuilder();

            foreach (byte mi in m)
                c.Append(Calc_ci(mi, b, n)).Append(Separator);

            c.Length--;
            return c.ToString();
        }
        
        private static string Calc_ci(byte mi, BigInteger b, BigInteger n) => ((mi * (mi + b)) % n).ToString();

        public static byte[] Decrypt(BigInteger p, BigInteger q, BigInteger n, BigInteger b, string c)
        {
            return Decrypt(p, q, n, b, c.ConvertToBigIntegers(Separator));
        }
        
        private static byte[] Decrypt(BigInteger p, BigInteger q, BigInteger n, BigInteger b, BigInteger[] c)
        {
            var m = new byte[c.Length];

            for (var i = 0; i < c.Length; i++)
            {
                var D = Calc_D(b, c[i], n);
                var mp = BigInteger.ModPow(D, (p + 1) / 4, p);
                var mq = BigInteger.ModPow(D, (q + 1) / 4, q);
                var (yp, yq) = MathUtils.ExtendedEuclideanAlgorithm(p, q);
                var d = Calc_d(yp, yq, p, q, mp, mq, n);
                var supposed_m = Calc_supposed_m(d, b, n);
                m[i] = Find_m(supposed_m);
            }

            return m;
        }
        
        private static BigInteger Calc_D(BigInteger b, BigInteger c, BigInteger n) => (b * b + 4 * c) % n;
        
        private static BigInteger[] Calc_d(BigInteger yp, BigInteger yq, BigInteger p, BigInteger q, BigInteger mp, BigInteger mq, BigInteger n)
        {
            var d = new BigInteger[4];
            d[0] = (yp * p * mq + yq * q * mp) % n;
            d[1] = n - d[0];
            d[2] = (yp * p * mq - yq * q * mp) % n;
            d[3] = n - d[2];
            return d;
        }
        
        private static BigInteger[] Calc_supposed_m(BigInteger[] d, BigInteger b, BigInteger n)
        {
            var m = new BigInteger[4];
            for (var i = 0; i < m.Length; i++)
            {
                m[i] = (d[i] - b) % 2 == 0
                    ? (-b + d[i]) / 2 % n
                    : (-b + n + d[i]) / 2 % n;
                if (m[i] < 0)
                    m[i] += n;
            }
            return m;
        }
        
        private static byte Find_m(BigInteger[] m)
        {
            var foundM = m.Where(mi => mi <= byte.MaxValue)
                .Select(mi => (byte)mi)
                .Distinct()
                .ToList();

            if (foundM.Count == 0)
                throw new InvalidOperationException("Can't find any m");
            
            if (foundM.Count > 1)
                throw new InvalidOperationException($"Found {foundM.Count} correct m values: {string.Join(", ", foundM)}");
            
            return foundM[0];
        }
    }
}