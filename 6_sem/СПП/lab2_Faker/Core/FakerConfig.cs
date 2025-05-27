using System.Linq.Expressions;
using Core.ValueGenerator;

namespace Core;

public class FakerConfig
{
    private readonly Dictionary<(Type, string), IValueGenerator> _customGenerators = new();

    public void Add<TObject, TProp, TGenerator>(Expression<Func<TObject, TProp>> propertySelector)
        where TGenerator : IValueGenerator, new()
    {
        if (propertySelector.Body is MemberExpression memberExpression)
        {
            var propertyName = memberExpression.Member.Name;
            _customGenerators[(typeof(TObject), propertyName)] = new TGenerator();
        }
    }

    public bool TryGetGenerator(Type objectType, string propertyName, out IValueGenerator? generator)
    {
        return _customGenerators.TryGetValue((objectType, propertyName), out generator);
    }
}