namespace Core.ValueGenerator;

public class StringGenerator : IValueGenerator
{
    public bool CanGenerate(Type type)
    {
        return type == typeof(string);
    }

    public object Generate(Type type, GeneratorContext context)
    {
        var length = context.Random.Next(1, 1000);
        return new string(Enumerable.Range(0, length)
            .Select(_ => (char)context.Random.Next(char.MinValue, char.MaxValue + 1))
            .ToArray());
    }
}