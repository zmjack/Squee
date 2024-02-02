using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Squee.Antd.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Squee.Antd.Pro;

public class AntdHelper<TContext>(TContext context)
{
    private static readonly MethodInfo _method_ToArray = typeof(Enumerable).GetMethod(nameof(Enumerable.ToArray))!;
    private static readonly MethodInfo _method_AsNoTracking = typeof(EntityFrameworkQueryableExtensions).GetMethod(nameof(EntityFrameworkQueryableExtensions.AsNoTracking))!;
    private static readonly MemoryCache _cache_IIdentifier_QueryChain = new(new MemoryCacheOptions());

    public string GetValueType(PropertyInfo prop)
    {
        var propType = prop.PropertyType;

        if (propType == typeof(string))
        {
            var stringLength = prop.GetCustomAttribute<StringLengthAttribute>();
            if (stringLength is not null)
            {
                return stringLength.MaximumLength switch
                {
                    < 256 => ValueType.Text,
                    _ => ValueType.TextArea,
                };
            }
            else return ValueType.TextArea;
        }
        else if (propType == (typeof(bool)))
        {
            return ValueType.Radio;
        }
        else if (propType == typeof(DateOnly) || propType == typeof(DateOnly?))
        {
            return ValueType.Date;
        }
        else if (propType == typeof(DateTime) || propType == typeof(DateTime?))
        {
            return ValueType.DateTime;
        }
        else if (propType.IsEnum)
        {
            return ValueType.Select;
        }
        else
        {
            var foreignKey = prop.GetCustomAttribute<ForeignKeyAttribute>();
            if (foreignKey is not null)
            {
                return ValueType.Select;
            }
            else return ValueType.Text;
        }
    }

    public IDictionary<object, IValueEnumValue>? GetValueEnum(PropertyInfo prop, PropertyInfo[] props)
    {
        var propType = prop.PropertyType;
        var entityMapping = new EntityMapping<TContext>(context);

        if (propType == typeof(bool))
        {
            return new Dictionary<object, IValueEnumValue>
            {
                [true] = new ValueEnumValue("是"),
                [false] = new ValueEnumValue("否"),
            };
        }
        else if (propType.IsEnum)
        {
            if (propType.GetCustomAttribute<FlagsAttribute>() is not null)
            {
                throw new NotImplementedException();
            }
            else
            {
                var valueEnumValues = new Dictionary<object, IValueEnumValue>();
                var enumOptions = EnumEx.GetOptions(propType);
                foreach (var option in enumOptions)
                {
                    valueEnumValues.Add(option.Value, new ValueEnumValue(option.Enum.GetDisplayName()));
                }
                return valueEnumValues;
            }
        }
        else
        {
            var foreignKey = prop.GetCustomAttribute<ForeignKeyAttribute>();
            if (foreignKey is not null)
            {
                var targetProp = props.First(x => x.Name == foreignKey.Name);
                var targetType = targetProp.PropertyType;
                var dbSet = entityMapping.Entity2DbSet(targetType).GetValue(context);

                object queryable = dbSet!;
                if (targetType.IsImplement(typeof(IIdentifier<>)))
                {
                    var queyrChain = _cache_IIdentifier_QueryChain.GetOrCreate(targetType.FullName, entry =>
                    {
                        entry.SlidingExpiration = TimeSpan.FromMinutes(1);
                        return targetType.GetMethod(nameof(IIdentifierNamer.QueryChain));
                    });
                    queryable = queyrChain!.Invoke(null, [dbSet])!;
                }
                //TODO: 不跟踪无法进行外部缓存
                //var asNoTracking = _method_AsNoTracking.MakeGenericMethod(targetType);
                //queryable = asNoTracking.Invoke(null, [queryable])!;

                var identifiers = _method_ToArray.MakeGenericMethod(targetType).Invoke(null, [queryable]) as IIdentifier[];

                var valueEnumValues = new Dictionary<object, IValueEnumValue>();
                var defaultKey = targetType.CreateDefault();
                if (defaultKey is not null)
                {
                    valueEnumValues[defaultKey] = new ValueEnumValue("");
                }

                if (identifiers is not null)
                {
                    foreach (var identifier in identifiers)
                    {
                        valueEnumValues.Add(identifier.Id, new ValueEnumValue(identifier.Name ?? ""));
                    }
                    return valueEnumValues;
                }
            }
        }

        return null;
    }

}
