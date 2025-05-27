namespace StringFormatter;

using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

public class StringFormatter : IStringFormatter
{
    public static readonly StringFormatter Shared = new StringFormatter();

    private readonly ConcurrentDictionary<string, Func<object, string>> _cache = new();

    public string Format(string template, object target)
    {
        if (template == null) throw new ArgumentNullException(nameof(template));
        if (target == null) throw new ArgumentNullException(nameof(target));

        var sb = new StringBuilder();
        for (int i = 0; i < template.Length;)
        {
            if (template[i] == '{')
            {
                if (i + 1 < template.Length && template[i + 1] == '{')
                {
                    sb.Append('{');
                    i += 2;
                    continue;
                }

                int end = template.IndexOf('}', i);
                if (end == -1)
                    throw new FormatException("Unclosed brace");

                var key = template.Substring(i + 1, end - i - 1).Trim();
                if (string.IsNullOrWhiteSpace(key))
                    throw new FormatException("Empty key");

                var getter = _cache.GetOrAdd(key, k => CompileGetter(k, target.GetType()));
                var value = getter(target);
                sb.Append(value);

                i = end + 1;
            }
            else if (template[i] == '}' && i + 1 < template.Length && template[i + 1] == '}')
            {
                sb.Append('}');
                i += 2;
            }
            else if (template[i] == '}')
            {
                throw new FormatException("Unopened brace");
            }
            else
            {
                sb.Append(template[i]);
                i++;
            }
        }

        return sb.ToString();
    }

    private Func<object, string> CompileGetter(string key, Type targetType)
    {
        var param = Expression.Parameter(typeof(object), "target");
        var casted = Expression.Convert(param, targetType);

        Expression body = casted;
        string[] parts = key.Split(new[] { '.' }, StringSplitOptions.None);

        foreach (var part in parts)
        {
            var propName = part;
            int? index = null;

            var indexStart = part.IndexOf('[');
            if (indexStart != -1 && part.EndsWith("]"))
            {
                propName = part.Substring(0, indexStart);
                var indexStr = part.Substring(indexStart + 1, part.Length - indexStart - 2);
                if (!int.TryParse(indexStr, out var parsed))
                    throw new FormatException($"Invalid index '{indexStr}'");
                index = parsed;
            }

            var member = (MemberInfo?)targetType.GetProperty(propName) ??
                        targetType.GetField(propName);

            if (member == null)
                throw new FormatException($"'{propName}' not found in {targetType.Name}");

            body = member switch
            {
                PropertyInfo prop => Expression.Property(body, prop),
                FieldInfo field => Expression.Field(body, field),
                _ => throw new InvalidOperationException()
            };

            if (index != null)
            {
                if (!typeof(System.Collections.IEnumerable).IsAssignableFrom(body.Type) ||
                    !body.Type.IsArray && body.Type.GetMethod("get_Item") == null)
                    throw new FormatException($"'{propName}' is not indexable");

                if (body.Type.IsArray)
                {
                    body = Expression.ArrayIndex(body, Expression.Constant(index.Value));
                }
                else
                {
                    var indexer = body.Type.GetMethod("get_Item", new[] { typeof(int) });
                    body = Expression.Call(body, indexer!, Expression.Constant(index.Value));
                }
            }
            
            targetType = body.Type;
        }

        var toStringCall = Expression.Call(body, "ToString", null);
        var lambda = Expression.Lambda<Func<object, string>>(toStringCall, param);
        return lambda.Compile();
    }
}
