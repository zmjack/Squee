using System.Collections;

namespace Squee.Antd.Pro;

public interface IStep
{
    string Title { get; set; }
    IStepColumn[] Columns { get; }
}

public class Step(string title) : IStep, IEnumerable<IStepColumn>
{
    public string Title { get; set; } = title;

    private readonly List<IStepColumn> _columns = [];
    public IStepColumn[] Columns => [.. _columns];

    public void Add(IStepColumn column)
    {
        _columns.Add(column);
    }

    public IEnumerator<IStepColumn> GetEnumerator()
    {
        return _columns.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}
