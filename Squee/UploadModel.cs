namespace Squee;

[Obsolete("Temporarily.")]
public interface IUploadModel<TSelf> where TSelf : IUploadModel<TSelf>
{
    public static TSelf Instance;
}
