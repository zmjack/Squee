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
            foreach (var pair in Filter)
            {
                if (pair.Value is null) continue;

                var prop = props.FirstOrDefault(p => StringEx.CamelCase(p.Name) == pair.Key);
                if (prop is not null)
                {
                    if (prop.PropertyType == typeof(string))
                    {
                        var value = pair.Value;
                        exp &= h.Property(prop.Name).Contains(value);
                    }
                    else if (new[] { typeof(Guid), typeof(Guid?) }.Contains(prop.PropertyType))
                    {
                        var value = Guid.Parse(pair.Value);
                        exp &= h.Property(prop.Name) == value;
                    }
                    else if (new[] { typeof(DateOnly), typeof(DateOnly?) }.Contains(prop.PropertyType))
                    {
                        var dt = DateTime.Parse(pair.Value);
                        var value = new DateOnly(dt.Year, dt.Month, dt.Day);
                        exp &= h.Property(prop.Name) == value;
                    }
                    else if (new[] { typeof(DateTime), typeof(DateTime?) }.Contains(prop.PropertyType))
                    {
                        var value = DateTime.Parse(pair.Value);
                        exp &= h.Property(prop.Name) == value;
                    }
                    else
                    {
                        var value = ConvertEx.ChangeType(pair.Value, prop.PropertyType);
                        exp &= h.Property(prop.Name) == value;
                    }
                }
            }
            return exp;
        });
    }
}
