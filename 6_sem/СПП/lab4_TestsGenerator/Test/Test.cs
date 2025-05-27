namespace Source
{
    public class TestClass
    {
        public class TextProcessor
        {
            private ITextFormatter formatter;

            public TextProcessor(ITextFormatter formatter)
            {
                this.formatter = formatter;
            }

            public string ReplaceWords(string text, string oldWord, string newWord)
            {
                return text.Replace(oldWord, newWord);
            }

            public int CountWords(string text)
            {
                if (string.IsNullOrEmpty(text))
                    return 0;
                return text.Split(' ').Length;
            }

            public void Ex() {
                
            }
        }

        public class MathOperations
        {
            private ICalculationConfig config;

            public MathOperations(ICalculationConfig config)
            {
                this.config = config;
            }

            public int Subtract(int a, int b)
            {
                return a - b;
            }

            public double Multiply(double x, double y, double factor)
            {
                return x * y * factor;
            }
        }

        public class ArrayHandler
        {
            public int[] GetEvenNumbers(int[] numbers)
            {
                return numbers.Where(x => x % 2 == 0).ToArray();
            }

            public double CalculateAverage(int[] numbers)
            {
                if (numbers == null || numbers.Length == 0)
                    return 0;
                return numbers.Average();
            }

            public string[] RemoveEmptyStrings(string[] items)
            {
                return items.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            }
        }

        public class DataValidator
        {
            private IValidationRules rules;

            public DataValidator(IValidationRules rules)
            {
                this.rules = rules;
            }

            public bool ValidateEmail(string email)
            {
                if (string.IsNullOrEmpty(email))
                    return false;
                return email.Contains("@");
            }

            public bool ValidateAge(int age, int minAge, int maxAge)
            {
                return age >= minAge && age <= maxAge;
            }
        }
    }

    public interface ITextFormatter
    {
        string Format(string text);
    }

    public interface ICalculationConfig
    {
        double DefaultFactor { get; }
    }

    public interface IValidationRules
    {
        int MinAge { get; }
        int MaxAge { get; }
    }
}