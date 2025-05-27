using System;
using System.Collections;
using System.Collections.Generic;

namespace RabinCryptosystemResearchHelper
{
    internal static class SieveOfEratosthenes
    {
        internal static List<int> GetVariantPQ(int max)
        {
            var primes = new List<int>() { 2 };

            var isPrime = new BitArray(max / 2 + 1, true);
            var sqrtMax = (int)Math.Sqrt(max);
            for (var p = 3; p <= sqrtMax; p += 2)
                if (isPrime[p / 2]) 
                    for (long i = p * p; i <= max; i += p * 2)
                        isPrime[(int)(i / 2)] = false;

            for (var i = 3; i <= max - 1; i += 2)
                if (isPrime[i / 2] && i % 4 == 3)
                    primes.Add(i);
            
            return primes;
        }
    }
}