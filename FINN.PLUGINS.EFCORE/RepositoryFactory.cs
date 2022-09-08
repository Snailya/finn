using Microsoft.EntityFrameworkCore;

namespace FINN.PLUGINS.EFCORE;

public class RepositoryFactory<TEntity, TContext> : IRepositoryFactory<TEntity>
    where TEntity : class
    where TContext : DbContext
{
    private readonly IDbContextFactory<TContext> _contextFactory;

    public RepositoryFactory(IDbContextFactory<TContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public EfRepository<TEntity> CreateRepository()
    {
        var context = _contextFactory.CreateDbContext();
        return new EfRepository<TEntity>(context);
    }

    public EfReadRepository<TEntity> CreateReadRepository()
    {
        var context = _contextFactory.CreateDbContext();
        return new EfReadRepository<TEntity>(context);
    }
}