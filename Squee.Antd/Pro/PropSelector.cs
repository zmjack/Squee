using System.Linq.Expressions;

namespace Squee.Antd.Pro;

public sealed class PropSelector<TContext, TEntity>(TContext context)
{
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
                else break;
            }
            else break;
        }
        return [.. units];
    }

    private static string GetDataIndex(string[] path)
    {
        return (from x in path select StringEx.CamelCase(x)).Join(".");
    }

    public IStepColumn Show<TRet>(Expression<Func<TEntity, TRet>> selector)
    {
        var path = GetPath(selector);
        var dataIndex = GetDataIndex(path);

        var stepDetector = new StepDetector<TContext>(context);
        var columns = stepDetector.DetectStepProps<TEntity>();
        var column = columns!.First(x => x.DataIndex == dataIndex);
        return column;
    }

    public IStepColumn Hide(Expression<Func<TEntity, object>> selector)
    {
        var column = Show(selector);
        column.HideInForm = true;
        return column;
    }

    public IStepColumn Show<TRet>(Expression<Func<TEntity, IEnumerable<TRet>>> selector, Func<PropSelector<TContext, TRet>, IStepColumn[]> tableSelector)
    {
        if (selector.Body is MemberExpression member)
        {
            var subSelector = new PropSelector<TContext, TRet>(context);

            var path = GetPath(selector);
            var dataIndex = GetDataIndex(path);

            var stepDetector = new StepDetector<TContext>(context);
            var columns = stepDetector.DetectStepProps<TEntity>();
            var column = columns!.First(x => x.DataIndex == dataIndex);
            column.Columns = [new ColumnGroup
            {
                Columns = tableSelector(subSelector),
            }];
            return column;
        }
        else throw new NotSupportedException("Only MemberExpression is supported.");
    }
}
