namespace Core.ValueGenerator;

public class IntGenerator : IValueGenerator
{
    public bool CanGenerate(Type type)
    {
        return type == typeof(int);
    }

    public object Generate(Type type, GeneratorContext context)
    {
        return context.Random.Next(1, 1000);
    }
}