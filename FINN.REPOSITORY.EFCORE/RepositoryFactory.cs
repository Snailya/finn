using FINN.CORE.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FINN.REPOSITORY.EFCORE;

public class RepositoryFactory<TEntity, TContext> : IRepositoryFactory<TEntity>
    where TEntity : class
    where TContext : DbContext
{
    private readonly IDbContextFactory<TContext> _contextFactory;

    public RepositoryFactory(IDbContextFactory<TContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    /// <inheritdoc />
    public IRepository<TEntity> CreateRepository()
    {
        var context = _contextFactory.CreateDbContext();
        return new EfRepository<TEntity>(context);
    }

    /// <inheritdoc />
    public IReadRepository<TEntity> CreateReadRepository()
    {
        var context = _contextFactory.CreateDbContext();
        return new EfReadRepository<TEntity>(context);
    }
}