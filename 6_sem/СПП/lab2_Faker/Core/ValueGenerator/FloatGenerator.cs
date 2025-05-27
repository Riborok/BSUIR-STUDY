namespace Core.ValueGenerator;

public class FloatGenerator : IValueGenerator
{
    public bool CanGenerate(Type type)
    {
        return type == typeof(float);
    }

    public object Generate(Type type, GeneratorContext context)
    {
        return (float)(context.Random.NextDouble() * 1000);
    }
}