namespace Core.ValueGenerator;

public class DateTimeGenerator : IValueGenerator
{
    public bool CanGenerate(Type type)
    {
        return type == typeof(DateTime);
    }

    public object Generate(Type type, GeneratorContext context)
    {
        int year = context.Random.Next(1, DateTime.Now.Year + 1);
        int month = context.Random.Next(1, 12 + 1);
        int day = context.Random.Next(1, 28);
        return new DateTime(year, month, day);
    }
}