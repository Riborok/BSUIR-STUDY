using System.Numerics;
using RSADigitalSignature;

namespace DigitalSigner.Params {
    public class SignParams {
        public BigInteger p { get; }
        public BigInteger q { get; }
        public BigInteger e { get; }
        public BigInteger r { get; private set; }
        public BigInteger phi { get; private set; }
        public BigInteger d { get; private set; }
        
        public SignParams(BigInteger p, BigInteger q, BigInteger e)
        {
            this.p = p;
            this.q = q;
            this.e = e;
            CalculateR();
            CalculatePhi();
            CalculateD();
        }
        
        private void CalculateR()
        {
            r = p * q;
        }

        private void CalculatePhi()
        {
            phi = (p - 1) * (q - 1);
        }

        private void CalculateD()
        {
            var (x, y) = MathUtils.ExtendedEuclideanAlgorithm(phi, e);
            if (y < 0)
                y += phi;
            d = y;
        }
    }
}