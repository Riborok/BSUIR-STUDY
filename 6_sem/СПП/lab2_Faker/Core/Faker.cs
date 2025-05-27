using System.Reflection;
using Core.ValueGenerator;

namespace Core;

public class Faker
{
    private readonly FakerConfig config;
    private readonly List<IValueGenerator> generators;
    private readonly Random random = new Random();
    private readonly GeneratorContext context;
    
    public Faker(FakerConfig? config = null) {
        context = new GeneratorContext(random, this);
        this.config = config ?? new FakerConfig();
        generators = [
            new IntGenerator(),
            new DoubleGenerator(),
            new StringGenerator(),
            new DateTimeGenerator(),
            new BoolGenerator(),
            new FloatGenerator(),
            new LongGenerator(),
            new CollectionGenerator()
        ];
    }

    public object Create(Type type)
    {
        return Create(type, new HashSet<Type>());
    }
    
    public T Create<T>()
    {
        return (T)Create(typeof(T), new HashSet<Type>());
    }

    private object? Create(Type type, HashSet<Type> typeStack)
    {
        if (!typeStack.Add(type))
            return GetDefaultValue(type);

        foreach (var generator in generators)
        {
            if (generator.CanGenerate(type))
            {
                var generated = generator.Generate(type, context);
                typeStack.Remove(type);
                return generated;
            }
        }

        object result = CreateByReflection(type, typeStack);
        typeStack.Remove(type);
        return result;
    }

    private object CreateByReflection(Type type, HashSet<Type> typeStack)
    {
        object? result = null;
        var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                               .OrderByDescending(c => c.GetParameters().Length);
        foreach (var ctor in constructors)
        {
            try
            {
                var parameters = ctor.GetParameters();
                object[] args = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    var param = parameters[i];
                    if (config.TryGetGenerator(type, param.Name, out IValueGenerator customGen))
                    {
                        args[i] = customGen.Generate(param.ParameterType, context);
                    }
                    else
                    {
                        args[i] = Create(param.ParameterType, typeStack);
                    }
                }
                result = ctor.Invoke(args);
                break;
            }
            catch (Exception ex)
            {
            }
        }
        
        if (result == null)
        {
            throw new Exception();
        }

        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                 .Where(p => p.CanWrite))
        {
            object currentValue = prop.GetValue(result);
            if (IsDefault(currentValue, prop.PropertyType))
            {
                if (config.TryGetGenerator(type, prop.Name, out IValueGenerator customGen))
                {
                    prop.SetValue(result, customGen.Generate(prop.PropertyType, context));
                }
                else
                {
                    prop.SetValue(result, Create(prop.PropertyType, typeStack));
                }
            }
        }

        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            object currentValue = field.GetValue(result);
            if (IsDefault(currentValue, field.FieldType))
            {
                if (config.TryGetGenerator(type, field.Name, out IValueGenerator customGen))
                {
                    field.SetValue(result, customGen.Generate(field.FieldType, context));
                }
                else
                {
                    field.SetValue(result, Create(field.FieldType, typeStack));
                }
            }
        }

        return result;
    }

    private bool IsDefault(object? value, Type type)
    {
        if (value == null)
            return true;
        if (type.IsValueType)
        {
            object? defaultValue = Activator.CreateInstance(type);
            return value.Equals(defaultValue);
        }
        return false;
    }

    private static object? GetDefaultValue(Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }
}