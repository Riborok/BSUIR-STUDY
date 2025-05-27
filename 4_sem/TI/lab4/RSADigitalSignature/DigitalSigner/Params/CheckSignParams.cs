using System.Numerics;

namespace DigitalSigner.Params {
    public class CheckSignParams {
        public BigInteger e { get; }
        public BigInteger r { get; }

        public CheckSignParams(BigInteger e, BigInteger r)
        {
            this.e = e;
            this.r = r;
        }
    }
}