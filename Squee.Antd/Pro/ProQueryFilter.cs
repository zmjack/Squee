namespace Squee.Antd;

public class ProQueryFilter<T> : IQueryFilter<T>
{
    public required Dictionary<string, string> Filter { get; set; }

    public IQueryable<T> Apply(IQueryable<T> source)
    {
        var props = typeof(T).GetProperties().ToArray();
        return source.Filter(h =>
        {
            var exp = h.Empty;
            foreach (var rule in Filter)
            {
                if (rule.Value is null) continue;

                var prop = props.FirstOrDefault(p => StringEx.CamelCase(p.Name) == rule.Key);
                if (prop is not null)
                {
                    if (prop.PropertyType == typeof(Guid))
                    {
                        var value = Guid.Parse(rule.Value);
                        exp &= h.Property(prop.Name) == value;
                    }
                    else if (prop.PropertyType == typeof(string))
                    {
                        var value = rule.Value;
                        exp &= h.Property(prop.Name).Contains(value);
                    }
                    else if (prop.PropertyType == typeof(DateOnly))
                    {
                        var value = DateOnly.Parse(rule.Value);
                        exp &= h.Property(prop.Name) == value;
                    }
                }
            }
            return exp;
        });
    }
}
