namespace Squee.Antd.Pro;

public interface IColumnGroup
{
    string ValueType { get; }
    IStepColumn[] Columns { get; set; }
}

public class ColumnGroup : IColumnGroup
{
    public string ValueType { get; } = Antd.ValueType.Group;
    public IStepColumn[] Columns { get; set; }
}

