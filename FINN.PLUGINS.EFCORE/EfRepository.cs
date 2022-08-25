using System.Linq.Expressions;
using FINN.CORE.Interfaces;
using FINN.PLUGINS.EFCORE.Data;
using Microsoft.EntityFrameworkCore;

namespace FINN.PLUGINS.EFCORE;

public class EfRepository<T> : IRepository<T> where T : class
{
    private readonly DbContext _dbContext;

    public EfRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().Add(entity);
        await SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities,
        CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().AddRange(entities);
        await SaveChangesAsync(cancellationToken);
        return entities;
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().Update(entity);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().UpdateRange(entities);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().Remove(entity);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().RemoveRange(entities);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
    {
        return await _dbContext.Set<T>().FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<List<T>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<T>().ToListAsync(cancellationToken);
    }

    public async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<T>().FirstOrDefaultAsync(predicate, cancellationToken);
    }
}