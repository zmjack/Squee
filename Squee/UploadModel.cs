using System.Linq.Expressions;

namespace Squee;

[Obsolete("Temporarily.")]
public interface IUploadModel<TSelf> where TSelf : IUploadModel<TSelf>
{
    Expression<Func<TSelf, object>> Include { get; }

    public static TSelf Instance;
}
