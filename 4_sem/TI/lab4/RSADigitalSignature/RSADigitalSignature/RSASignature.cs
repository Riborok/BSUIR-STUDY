// ReSharper disable InconsistentNaming
using System.Numerics;

namespace RSADigitalSignature {
    public static class RSASignature {
        public static BigInteger GetSignature(BigInteger m, BigInteger d, BigInteger r) {
            return BigInteger.ModPow(m, d, r);
        }

        public static bool CheckSignature(BigInteger s, BigInteger e, BigInteger r, BigInteger signature) {
            return GetSignature(s, e, r) == signature;
        }
    }
}