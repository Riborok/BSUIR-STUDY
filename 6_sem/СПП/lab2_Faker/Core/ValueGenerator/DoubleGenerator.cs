namespace Core.ValueGenerator;

public class DoubleGenerator : IValueGenerator
{
    public bool CanGenerate(Type type)
    {
        return type == typeof(double);
    }

    public object Generate(Type type, GeneratorContext context)
    {
        return context.Random.NextDouble() * 1000;
    }
}