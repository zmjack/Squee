using Microsoft.EntityFrameworkCore;

namespace Squee.Antd.Pro;

public interface IIdentifier
{
    Guid Id { get; }
    string? Name { get; }
}

public interface IIdentifier<TSelf> : IIdentifier
    where TSelf : class, IIdentifier<TSelf>
{
    static abstract IQueryable<TSelf> QueryChain(DbSet<TSelf> dbSet);
}

internal class IIdentifierNamer : IIdentifier<IIdentifierNamer>
{
    public Guid Id => throw new NotImplementedException();
    public string? Name => throw new NotImplementedException();
    public static IQueryable<IIdentifierNamer> QueryChain(DbSet<IIdentifierNamer> set)
    {
        throw new NotImplementedException();
    }
}