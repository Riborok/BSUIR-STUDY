using System.Collections;

namespace Core.ValueGenerator;

public class CollectionGenerator : IValueGenerator
{
    public bool CanGenerate(Type type)
    {
        if (type == typeof(string))
            return false;
        return typeof(IEnumerable).IsAssignableFrom(type) && type.IsGenericType;
    }

    public object Generate(Type type, GeneratorContext context)
    {
        Type elementType = type.GetGenericArguments()[0];
        int count = context.Random.Next(1, 1000);
        Type listType = typeof(List<>).MakeGenericType(elementType);
        var list = (IList)Activator.CreateInstance(listType)!;
        for (int i = 0; i < count; i++)
        {
            list.Add(context.Faker.Create(elementType));
        }

        if (type.IsArray)
        {
            Array array = Array.CreateInstance(elementType, list.Count);
            list.CopyTo(array, 0);
            return array;
        }
        
        return list;
    }
}
