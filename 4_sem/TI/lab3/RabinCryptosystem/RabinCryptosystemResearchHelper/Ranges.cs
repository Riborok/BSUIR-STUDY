// ReSharper disable InconsistentNaming

using System;

namespace RabinCryptosystemResearchHelper
{
    public readonly struct Ranges
    {
        public Range pRange { get; }
        public Range qRange { get; }
        public Range bRange { get; }

        public Ranges(Range pRange, Range qRange, Range bRange)
        {
            pRange.Start = Math.Max(0, pRange.Start);
            this.pRange = pRange;
            
            qRange.Start = Math.Max(0, qRange.Start);
            this.qRange = qRange;
            
            bRange.Start = Math.Max(1, bRange.Start);
            this.bRange = bRange;
        }
        
        public void Deconstruct(out Range pRange, out Range qRange, out Range bRange)
        {
            pRange = this.pRange;
            qRange = this.qRange;
            bRange = this.bRange;
        }
    }
}