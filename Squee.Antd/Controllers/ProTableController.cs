using Microsoft.AspNetCore.Mvc;

namespace Squee.Antd.Controllers;

public abstract class ProTableController<TSource, TResult> : Controller
{
    protected abstract IEnumerable<TSource> Filter(Dictionary<string, string> filter);
    protected abstract TResult[] Select(IEnumerable<TSource> source);

    protected IProTableResult Query(Dictionary<string, string> request)
    {
        var current = request.TryGetValue("current", out var s_current) ? int.TryParse(s_current, out var _current) ? _current : 0 : 0;
        var pageSize = request.TryGetValue("pageSize", out var s_pageSize) ? int.TryParse(s_pageSize, out var _pageSize) ? _pageSize : 0 : 0;

        var dict = new Dictionary<string, string>();
        foreach (var pair in request)
        {
            if (pair.Key == "current" || pair.Key == "pageSize") continue;
            dict.Add(pair.Key, pair.Value);
        }

        var source = Filter(dict);
        if (current > 0 && pageSize > 0)
        {
            //source = source.Page(current, pageSize);
        }

        var result = Select(source);
        var proTableResult = new TableResult<TResult>
        {
            Success = true,
            Data = result,
        };

        return proTableResult;
    }

}
