using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Squee;

public static class DbSetExtensions
{
    [Obsolete("Not recommanded.", true)]
    public static void ForceUpdate<TMain, TMainKey, TSub, TSubKey>(this DbSet<TMain> @this, DbSet<TSub> subSet, Func<TMain, TMainKey> mainKeySelector, Expression<Func<TSub, TSubKey>> subKeySelector, Expression<Func<TSub, TMainKey>> rootSelector, TMain model, Func<TMain, IEnumerable<TSub>> subsSelector)
        where TMain : class
        where TSub : class
    {
        var func_subKeySelector = subKeySelector.Compile();
        var pairs = subSet.AsNoTracking()
            .Where(
                Expression.Lambda<Func<TSub, bool>>(
                    Expression.Equal(rootSelector.Body, Expression.Constant(mainKeySelector(model))),
                    [rootSelector.Parameters[0]]
                )
            ).LeftJoin(subsSelector(model), func_subKeySelector, func_subKeySelector).ToArray();
        var deletingIds = (
            from pair in pairs
            let record = pair.Left
            let part = pair.Right
            where part is null
            select func_subKeySelector(record)
        ).ToArray();

        var contains = IQueryableExtensions.GenericContains.MakeGenericMethod(typeof(TSubKey));
        var deletingRecords = subSet.Where(
            Expression.Lambda<Func<TSub, bool>>(
                Expression.Call(null, contains, Expression.Constant(deletingIds), subKeySelector.Body),
                [subKeySelector.Parameters[0]]
            )
        ).ToArray();

        subSet.RemoveRange(deletingRecords);
        @this.Update(model);
    }
}
