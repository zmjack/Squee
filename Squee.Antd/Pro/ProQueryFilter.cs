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
                    if (prop.PropertyType == typeof(string))
                    {
                        var value = rule.Value;
                        exp &= h.Property(prop.Name).Contains(value);
                    }
                    else if (new[] { typeof(Guid), typeof(Guid?) }.Contains(prop.PropertyType))
                    {
                        var value = Guid.Parse(rule.Value);
                        exp &= h.Property(prop.Name) == value;
                    }
                    else if (new[] { typeof(DateOnly), typeof(DateOnly?) }.Contains(prop.PropertyType))
                    {
                        var value = DateOnly.Parse(rule.Value);
                        exp &= h.Property(prop.Name) == value;
                    }
                    else if (new[] { typeof(DateTime), typeof(DateTime?) }.Contains(prop.PropertyType))
                    {
                        var value = DateTime.Parse(rule.Value);
                        exp &= h.Property(prop.Name) == value;
                    }
                }
            }
            return exp;
        });
    }
}
