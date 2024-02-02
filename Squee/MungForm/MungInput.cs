using Mung;

namespace Squee.MungForm;

public abstract class MungFormInput
{
    public string Type { get; set; }
}

public class MungInputText : MungFormInput
{
    public string Value { get; set; }
}

public class MungSelect(string key) : MungFormInput
{
    public bool Nullable { get; set; }
    public Dictionary<string, string> Options { get; set; }
    public string Value { get; set; }
}

public class MungArea : MungFormInput
{
    public string Value { get; set; }
}

public class MungList : MungFormInput
{

}
