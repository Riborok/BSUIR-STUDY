using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace RabinCryptosystemResearchHelper
{
    public static class PrimeStorage
    {
        public const string PrimesFileName = "primes.bin";
        public const string PrimesTextFileName = "primes.txt";

        internal static List<int> LoadOrCalcPrimes()
        {
            if (File.Exists(PrimesFileName))
            {
                using (var fileStream = new FileStream(PrimesFileName, FileMode.Open))
                {
                    var binaryFormatter = new BinaryFormatter();
                    return (List<int>)binaryFormatter.Deserialize(fileStream);
                }
            }
            
            var result =  SieveOfEratosthenes.GetVariantPQ(int.MaxValue);
            SavePrimesToFile(result);
            return result;
        }

        private static void SavePrimesToFile(List<int> primes)
        {
            using (var fileStream = new FileStream(PrimesFileName, FileMode.Create))
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStream, primes);
            }
            
            using (var textFileStream = new StreamWriter(PrimesTextFileName))
                for (var i = 0; i < primes.Count; i++)
                    textFileStream.WriteLine($"{i}: {primes[i]}");
        }
    }
}