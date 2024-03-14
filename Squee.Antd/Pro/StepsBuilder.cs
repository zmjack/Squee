namespace Squee.Antd.Pro;

[Obsolete("不再使用", false)]
public class StepsBuilder<TContext>(TContext context)
{
    public StepsForm Build<T>(string title, T? entity, Func<PropSelector<TContext, T>, IStep[]> selector)
    {
        var propSelector = new PropSelector<TContext, T>(context);
        var form = new StepsForm(title, entity);
        foreach (var step in selector(propSelector))
        {
            form.Add(step);
        };
        return form;
    }
}
