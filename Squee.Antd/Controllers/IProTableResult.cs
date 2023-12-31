namespace Squee.Antd.Controllers;

public interface IProTableResult
{
    bool Success { get; set; }
    object? Data { get; set; }
    int? Total { get; set; }
}

public class TableResult<T> : IProTableResult
{
    public bool Success { get; set; }
    public int? Total { get; set; }
    object? IProTableResult.Data { get; set; }

    public T[]? Data
    {
        get => (this as IProTableResult).Data as T[];
        set => (this as IProTableResult).Data = value;
    }
}
