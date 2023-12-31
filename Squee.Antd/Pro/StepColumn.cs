namespace Squee.Antd.Pro;

public interface IStepColumn
{
    string Title { get; set; }
    string? ValueType { get; set; }
    bool HideInForm { get; set; }
    string DataIndex { get; set; }
    string? InitialValue { get; set; }
    IFormItemProp? FormItemProps { get; set; }
    IDictionary<object, IValueEnumValue>? ValueEnum { get; set; }
    IColumnGroup[]? Columns { get; set; }
}

public class StepColumn : IStepColumn
{
    public string Title { get; set; }

    /// <summary>
    /// <see cref="Antd.ValueType" />
    /// </summary>
    public string? ValueType { get; set; }
    public bool HideInForm { get; set; }

    public string DataIndex { get; set; }

    /// <summary>
    /// 前端设置
    /// </summary>
    public string? InitialValue { get; set; }

    public IFormItemProp? FormItemProps { get; set; }
    public IDictionary<object, IValueEnumValue>? ValueEnum { get; set; }
    public IColumnGroup[]? Columns { get; set; }
}
