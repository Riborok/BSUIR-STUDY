namespace Core.ValueGenerator;

public class LongGenerator : IValueGenerator
{
    public bool CanGenerate(Type type)
    {
        return type == typeof(long);
    }

    public object Generate(Type type, GeneratorContext context)
    {
        return (long)context.Random.Next(1, 1000);
    }
}
