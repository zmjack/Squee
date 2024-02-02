using System.Diagnostics.CodeAnalysis;

namespace Squee;

public class AutoComponent
{
    public string Type { get; set; }

    [StringSyntax(StringSyntaxAttribute.Json)]
    public string Json { get; set; }
}
