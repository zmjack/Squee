namespace Squee.Antd.Pro;

public interface IValueEnumValue
{
    string Text { get; set; }
}

public class ValueEnumValue(string text) : IValueEnumValue
{
    public string Text { get; set; } = text;
}
