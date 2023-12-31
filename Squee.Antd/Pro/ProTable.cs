using Squee.Antd.Controllers;

namespace Squee.Antd.Pro;

public interface IProTable
{
    public IProColumn[] Columns { get; set; }
    public IProTableResult Result { get; set; }
}

public class ProTable<TContext, TEntity> : IProTable
{
    public ProTable(TContext context, IProTableResult data)
    {
        var columnsDetector = new ColumnsDetector<TContext>(context);
        Columns = columnsDetector.DetectColumns<TEntity>();
        Result = data;
    }

    public IProColumn[] Columns { get; set; }
    public IProTableResult Result { get; set; }
}
