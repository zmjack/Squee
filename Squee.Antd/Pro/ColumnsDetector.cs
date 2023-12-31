using LinqSharp.EFCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Squee.Antd.Pro;

public class ColumnsDetector<TContext>(TContext context)
{
    public IProColumn[] DetectColumns<T>()
    {
        var antd = new AntdHelper<TContext>(context);
        var type = typeof(T);
        var list = new List<IProColumn>();
        var props = type.GetProperties().ToArray();

        foreach (var prop in props)
        {
            if (prop.Name == "Id") continue;
            if (prop.Name.EndsWith("Link")) continue;
            if (prop.PropertyType.IsType(typeof(ICollection<>))) continue;

            var jsonIgnore = prop.GetCustomAttribute<JsonIgnoreAttribute>();
            if (jsonIgnore is not null) continue;

            var autoCreationTime = prop.GetCustomAttribute<AutoCreationTimeAttribute>();
            if (autoCreationTime is not null) continue;

            var autoLastWriteTime = prop.GetCustomAttribute<AutoLastWriteTimeAttribute>();
            if (autoLastWriteTime is not null) continue;

            var display = prop.GetCustomAttribute<DisplayAttribute>();
            var title = display?.Name ?? prop.Name;

            var valueType = antd.GetValueType(prop);
            var valueEnum = antd.GetValueEnum(prop, props);

            list.Add(new ProColumn
            {
                Title = title,
                DataIndex = [StringEx.CamelCase(prop.Name)],
                ValueType = valueType,
                ValueEnum = valueEnum,
            });
        }

        return list.ToArray();
    }
}
