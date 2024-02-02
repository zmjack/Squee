using Mung;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using static Squee.GuidePanelModel;

namespace Squee;

public static class UIComponent
{
    public const string Hide = "hide";
    public const string Text = "text";
    public const string List = "list";
}

public class MungFormColumn
{
    /// <summary>
    /// <see cref="UIComponent" />
    /// </summary>
    public required string Type { get; set; }
    public required string DataIndex { get; set; }
    public required string Label { get; set; }
    public Dictionary<Guid, string>? Options { get; set; }
}

public class MungFormItem<T> : MungFormColumn
{
    public required T Value { get; set; }

    public Hori ToHori()
    {
        return new Hori(Type)
        {
            new MungValue(Value) { Source = this },
        };
    }
}

public class MungFormList<T> : MungFormColumn
{
    public required MungFormColumn[] Columns { get; set; }
    public required IEnumerable<T> Rows { get; set; }

    public Hori ToHori()
    {
        return new Hori(Type)
        {
            new MungValue("[array]") { Source = this },
        };
    }
}

public class MungFormHelper<TEntity>(TEntity source)
{
    private static string GetDisplayName<TRet>(Expression<Func<TEntity, TRet>> selector)
    {
        var expr = selector.Body;

        if (expr is MemberExpression member)
        {
            var display = member.Member.GetCustomAttribute<DisplayAttribute>();
            return display?.Name ?? member.Member.Name;
        }
        else if (expr is UnaryExpression unary)
        {
            if (unary.NodeType == ExpressionType.Convert)
            {
                var _member = (unary.Operand as MemberExpression)!;
                var display = _member.Member.GetCustomAttribute<DisplayAttribute>();
                return display?.Name ?? _member.Member.Name;
            }
            else throw new NotSupportedException();
        }
        else throw new NotSupportedException();
    }

    private static string[] GetPath<TRet>(Expression<Func<TEntity, TRet>> selector)
    {
        var units = new LinkedList<string>();
        for (var expr = selector.Body; expr is not null;)
        {
            if (expr is MemberExpression member)
            {
                units.AddFirst(member.Member.Name);
                expr = member.Expression;
            }
            else if (expr is UnaryExpression unary)
            {
                if (unary.NodeType == ExpressionType.Convert)
                {
                    var _member = (unary.Operand as MemberExpression)!;
                    units.AddFirst(_member.Member.Name);
                    expr = _member.Expression;
                }
                else throw new NotSupportedException();
            }
            else if (expr is ParameterExpression param) break;
            else throw new NotSupportedException();
        }
        return [.. units];
    }

    private static string GetDataIndex(string[] path)
    {
        return (from x in path select StringEx.CamelCase(x)).Join(".");
    }

    public TEntity Source { get; set; } = source;

    public MungFormColumn Column<T>(Expression<Func<TEntity, T>> selector)
    {
        var path = GetPath(selector);
        var dataIndex = GetDataIndex(path);

        return new MungFormColumn
        {
            Type = UIComponent.Text,
            DataIndex = dataIndex,
            Label = GetDisplayName(selector),
        };
    }

    public MungFormItem<T> Item<T>(Expression<Func<TEntity, T>> selector)
    {
        var path = GetPath(selector);
        var dataIndex = GetDataIndex(path);

        return new MungFormItem<T>
        {
            Type = UIComponent.Text,
            DataIndex = dataIndex,
            Label = GetDisplayName(selector),
            Value = Source is not null ? selector.Compile()(Source) : default,
        };
    }

    public MungFormList<T> List<T>(Expression<Func<TEntity, IEnumerable<T>>> selector, Func<MungFormHelper<T>, MungFormColumn[]> columnSelector)
    {
        var path = GetPath(selector);
        var dataIndex = GetDataIndex(path);

        var helper = new MungFormHelper<T>(default);
        return new MungFormList<T>
        {
            Type = UIComponent.List,
            DataIndex = dataIndex,
            Label = GetDisplayName(selector),
            Columns = columnSelector(helper),
            Rows = selector.Compile()(Source),
        };
    }
}
