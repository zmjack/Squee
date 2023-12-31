using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace Squee.Antd;

public class EntityMapping<TContext>(TContext context)
{
    private static readonly MemoryCache _dbsetCache = new(new MemoryCacheOptions());

    public PropertyInfo Entity2DbSet(Type type)
    {
        return _dbsetCache.GetOrCreate(type.FullName!, entry =>
        {
            var props = context.GetType().GetProperties().Where(x => x.PropertyType.Name.StartsWith("DbSet"));

            foreach (var prop in props)
            {
                var entityType = prop.PropertyType.GetGenericArguments()[0];
                if (entityType == type) return prop;
            }

            return null;
        })!;
    }
}
