namespace RabinCryptosystemResearchHelper
{
    public struct Range
    {
        public int Start { get; set; }
        public int End { get; }
        public int Count => End - Start;

        public Range(int start, int end)
        {
            Start = start;
            End = end;
        }
    }
}