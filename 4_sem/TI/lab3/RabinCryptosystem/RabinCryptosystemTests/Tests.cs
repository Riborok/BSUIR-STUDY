using System;
using System.Numerics;
using NUnit.Framework;
using RabinCryptosystem;

namespace RabinCryptosystemTests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Test1()
        {
            byte value = 17;
            
            BigInteger p = 5003;
            BigInteger q = 5227;
            BigInteger n = p * q;
            BigInteger b = 1234;
            
            var c = RabinEncryptor.Encrypt(n, b, new []{value});
            var m = RabinEncryptor.Decrypt(p, q, n, b, c);
            Assert.AreEqual(m[0], value);
        }
    }
}