using System.Numerics;

namespace RabinCryptosystem
{
    internal static class Converter
    {
        internal static BigInteger[] ConvertToBigIntegers(this string numbersString, char separator)
        {
            string[] numberStrings = numbersString.Split(separator);
            var numbers = new BigInteger[numberStrings.Length];

            for (int i = 0; i < numberStrings.Length; i++)
                numbers[i] = BigInteger.Parse(numberStrings[i]);
            
            return numbers;
        }
    }
}