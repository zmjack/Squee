using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;
using System.Reflection;

namespace Squee.Antd;

public static class IQueryableExtensions
{
    private static readonly MemoryCache _genericCache = new(new MemoryCacheOptions());
    public static MethodInfo GenericContains => _genericCache.GetOrCreate($"${nameof(Enumerable)}.{nameof(Enumerable.Contains)}", entry =>
    {
        var generic = typeof(Enumerable).GetMethodViaQualifiedName("Boolean Contains[TSource](System.Collections.Generic.IEnumerable`1[TSource], TSource)");
        return generic;
    });

    private static T[] Query<T, TInner, TKey>(IQueryable<T> outer, ICollection<TInner> inner, Expression<Func<T, TKey>> outerKeySelector, Func<TInner, TKey> innerKeySelector)
    {
        var compiledKeySelector = outerKeySelector.Compile();
        var keys = inner.Select(innerKeySelector);
        var parameter = outerKeySelector.Parameters[0];

        var contains = GenericContains.MakeGenericMethod(typeof(TKey));
        var exp = Expression.Lambda<Func<T, bool>>(
            Expression.Call(null, contains, Expression.Constant(keys), outerKeySelector.Body),
            parameter
        );

        return [.. outer.Where(exp)];
    }

    //public static IEnumerable<IJoinResult<TOuter, TInner>> LeftJoin<TOuter, TInner, TKey>(this IQueryable<TOuter> outer, ICollection<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Func<TInner, TKey> innerKeySelector)
    //{
    //    var keySelector = outerKeySelector.Compile();
    //    var records = Query(outer, inner, outerKeySelector, innerKeySelector);
    //    return records.LeftJoin(inner, keySelector, innerKeySelector);
    //}

    public static IEnumerable<IJoinResult<TOuter, TInner>> RightJoin<TOuter, TInner, TKey>(this IQueryable<TOuter> outer, ICollection<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Func<TInner, TKey> innerKeySelector)
    {
        var keySelector = outerKeySelector.Compile();
        var records = Query(outer, inner, outerKeySelector, innerKeySelector);
        return records.RightJoin(inner, keySelector, innerKeySelector);
    }

    public static IEnumerable<IJoinResult<TOuter, TInner>> InnerJoin<TOuter, TInner, TKey>(this IQueryable<TOuter> outer, ICollection<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Func<TInner, TKey> innerKeySelector)
    {
        var keySelector = outerKeySelector.Compile();
        var records = Query(outer, inner, outerKeySelector, innerKeySelector);
        return records.Join(inner, keySelector, innerKeySelector, (outer, inner) => (IJoinResult<TOuter, TInner>)new JoinResult<TOuter, TInner>
        {
            Left = outer,
            Right = inner,
        });
    }

    public static IEnumerable<IJoinResult<TOuter, TInner>> InnerJoin<TOuter, TInner, TKey>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector)
    {
        return outer.Join(inner, outerKeySelector, innerKeySelector, (outer, inner) => (IJoinResult<TOuter, TInner>)new JoinResult<TOuter, TInner>
        {
            Left = outer,
            Right = inner,
        });
    }
}
