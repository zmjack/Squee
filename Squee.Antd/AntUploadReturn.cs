namespace Squee.Antd;

public class AntUploadReturn : AntUploadReturn<object> { }

public class AntUploadReturn<TData>
{
    public string Name { get; set; }
    public AntUploadStatus Status { get; set; }
    public string Url { get; set; }
    public string ThumbUrl { get; set; }
    public TData Data { get; set; }
}
