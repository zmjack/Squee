namespace Squee.Antd.Pro;

public interface IFormItemProp
{
    StepRule[]? Rules { get; set; }
}

public class FormItemProps : IFormItemProp
{
    public StepRule[]? Rules { get; set; }
}
