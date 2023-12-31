namespace Squee.Antd.Pro;

public interface IProColumn
{
    string Title { get; set; }
    string[] DataIndex { get; set; }
    string ValueType { get; set; }
    IDictionary<object, IValueEnumValue>? ValueEnum { get; set; }
}

public class ProColumn : IProColumn
{
    public required string Title { get; set; }
    public required string[] DataIndex { get; set; }
    public required string ValueType { get; set; }
    public IDictionary<object, IValueEnumValue>? ValueEnum { get; set; }
}
