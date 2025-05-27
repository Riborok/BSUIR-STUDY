using System;
using System.Collections.Generic;
using RabinCryptosystem;

namespace RabinCryptosystemResearchHelper
{
    public class UnambiguityTest
    {
        private readonly byte[] _data = new byte[byte.MaxValue + 1];
        public List<int> Primes { get; } 

        public UnambiguityTest()
        {
            Primes = PrimeStorage.LoadOrCalcPrimes();
            for (var i = 0; i < _data.Length; i++)
                _data[i] = (byte)i;
        }
        
        public List<TestResult> Run(Range pRange, Range qRange, Range bRange)
        {
            var list = new List<TestResult>();
            for (int i = pRange.Start; i < Math.Min(Primes.Count, pRange.End); i++)
            {
                int p = Primes[i];
                for (int j = qRange.Start; j < Math.Min(Primes.Count, qRange.End); j++)
                {
                    int q = Primes[j];
                    long n = p * q;
                    if (n <= byte.MaxValue + 1)
                        continue;

                    for (long b = bRange.Start; b < Math.Min(n, bRange.End); b++)
                        AddUnambiguityEncryptedResult(list, p, q, n, b, i, j);
                }
            }
            return list;
        }
        
        private void AddUnambiguityEncryptedResult(List<TestResult> list, int p, int q, long n, long b, int pIndex, int qIndex)
        {
            string m = RabinEncryptor.Encrypt(n, b, _data);
            try
            {
                byte[] c = RabinEncryptor.Decrypt(p, q, n, b, m);
                list.Add(new TestResult(p, q, b, pIndex, qIndex));
            }
            catch (ArgumentException e)
            {
            }
        }
    }
}