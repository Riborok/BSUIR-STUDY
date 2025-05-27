using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RabinCryptosystemResearchHelper;

namespace RabinCryptosystemResearchConsole
{
    internal static class Program
    {
        private const string ResultFileName = "result.txt";
        
        public static void Main()
        {
            UnambiguityTest? unambiguityTest = null;
            var task = Task.Run(() => unambiguityTest = new UnambiguityTest());
            Console.WriteLine($"A file named '{PrimeStorage.PrimesTextFileName}' has been created where the indices and values of prime numbers with remainder 3 when divided by 4 are listed.");
            var pRange = ReadRangeFromConsole("Enter the range for p (e.g., 1, 10) where p is the index of a prime number with remainder 3 when divided by 4: ");
            var qRange = ReadRangeFromConsole("Enter the range for q (e.g., 1, 10) where q is the index of a prime number with remainder 3 when divided by 4: ");
            var bRange = ReadRangeFromConsole("Enter the range for b (e.g., 1, 10) where b is a prime number: ");
            task.Wait();
            var result = unambiguityTest!.Run(pRange, qRange, bRange);
            SaveResultToFile(result);
        }

        private static Range ReadRangeFromConsole(string message)
        {
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine();
                if (input != null && IsValidRangeInput(input, out int start, out int end))
                    return new Range(start, end);
                Console.WriteLine("Invalid input. Please enter the range in the format 'start, end'.");
            }
        }
        
        private static bool IsValidRangeInput(string input, out int start, out int end)
        {
            start = end = 0;
            string[] parts = input.Split(',');
            return parts.Length == 2 && 
                   int.TryParse(parts[0].Trim(), out start) && 
                   int.TryParse(parts[1].Trim(), out end);
        }
        
        private static void SaveResultToFile(List<TestResult> resultList)
        {
            using var writer = new StreamWriter(ResultFileName);
            foreach (var result in resultList)
                writer.WriteLine($"p: {result.p}, q: {result.q}, b: {result.b}");
        }
    }
}