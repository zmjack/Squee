using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Squee.Antd.Pro;

public class StepDetector<TContext>(TContext context)
{
    public IStepColumn[] DetectStepProps<T>()
    {
        var antd = new AntdHelper<TContext>(context);
        var type = typeof(T);

        var list = new List<IStepColumn>();
        var props = (from prop in type.GetProperties() select prop).ToArray();

        foreach (var prop in props)
        {
            if (prop.Name.EndsWith("Link")) continue;

            var propType = prop.PropertyType;
            var display = prop.GetCustomAttribute<DisplayAttribute>();
            var label = display?.Name ?? prop.Name;

            if (propType.IsType(typeof(ICollection<>)))
            {
                list.Add(new StepColumn
                {
                    Title = label,
                    ValueType = ValueType.FormList,
                    DataIndex = StringEx.CamelCase(prop.Name),
                });
                continue;
            }

            string? valueType = antd.GetValueType(prop);
            if (prop.GetCustomAttribute<JsonIgnoreAttribute>() is not null)
            {
                valueType = null;
            }
            else valueType = antd.GetValueType(prop);

            var valueEnum = antd.GetValueEnum(prop, props);
            var required = Any.Create(() =>
            {
                if (prop.HasAttribute<KeyAttribute>()) return false;

                var attr = prop.GetCustomAttribute<RequiredAttribute>();
                if (attr is not null) return true;
                if (!propType.IsNullable()) return true;
                return false;
            });

            var rules = new List<StepRule>();
            if (required)
            {
                rules.Add(new StepRule
                {
                    Required = true,
                    Message = $"{label} 不能为空",
                });
            }

            list.Add(new StepColumn
            {
                Title = label,
                ValueType = valueType,
                DataIndex = StringEx.CamelCase(prop.Name),
                ValueEnum = valueEnum,
                FormItemProps = new FormItemProps
                {
                    Rules = [.. rules],
                }
            });
        }

        return [.. list];
    }
}
