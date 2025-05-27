// ReSharper disable InconsistentNaming
using System;
using System.Numerics;
using System.Security.Cryptography;

namespace RSADigitalSignature {
    public class Hasher {
        public static BigInteger HashBasic(byte[] bytes, BigInteger n) => HashBasic(bytes, n, 100);

        public static BigInteger HashBasic(byte[] bytes, BigInteger n, BigInteger h0) {
            var hi = h0;

            foreach (byte b in bytes)
                hi = BigInteger.ModPow(hi + b, 2, n);

            return hi;
        }

        public static BigInteger HashSHA256(byte[] data, BigInteger salt) {
            byte[] dataWithSalt = AddSalt(data, salt);
            var hash = ComputeHash(dataWithSalt, SHA256.Create);
            return BigInteger.Abs(hash) % salt;
        }

        public static BigInteger HashSHA512(byte[] data, BigInteger salt) {
            byte[] dataWithSalt = AddSalt(data, salt);
            var hash = ComputeHash(dataWithSalt, SHA512.Create);
            return BigInteger.Abs(hash) % salt;
        }

        public static BigInteger HashMD5(byte[] data, BigInteger salt) {
            byte[] dataWithSalt = AddSalt(data, salt);
            var hash = ComputeHash(dataWithSalt, MD5.Create);
            return BigInteger.Abs(hash) % salt;
        }
        
        private static byte[] AddSalt(byte[] data, BigInteger salt) {
            byte[] saltBytes = salt.ToByteArray();

            var dataWithSalt = new byte[data.Length + saltBytes.Length];
            Array.Copy(data, 0, dataWithSalt, 0, data.Length);
            Array.Copy(saltBytes, 0, dataWithSalt, data.Length, saltBytes.Length);

            return dataWithSalt;
        }

        private static BigInteger ComputeHash(byte[] dataWithSalt, Func<HashAlgorithm> hashAlgorithmFactory) {
            using var hashAlgorithm = hashAlgorithmFactory();
            byte[] hashBytes = hashAlgorithm.ComputeHash(dataWithSalt);
            return new BigInteger(hashBytes);
        }
    }
}