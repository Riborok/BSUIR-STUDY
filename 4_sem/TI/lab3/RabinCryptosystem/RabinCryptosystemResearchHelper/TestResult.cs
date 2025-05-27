// ReSharper disable InconsistentNaming
namespace RabinCryptosystemResearchHelper
{
    public struct TestResult
    {
        public int p { get; }
        public int q { get; }
        public long b { get; }
        public int pIndex { get; }
        public int qIndex { get; }

        public TestResult(int p, int q, long b, int pIndex, int qIndex)
        {
            this.p = p;
            this.q = q;
            this.b = b;
            this.pIndex = pIndex;
            this.qIndex = qIndex;
        }
    }
}