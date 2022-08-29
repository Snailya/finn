﻿using System.Linq.Expressions;
using FINN.CORE.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FINN.PLUGINS.EFCORE;

public class EfReadRepository<T> : IReadRepository<T> where T : class
{
    protected readonly DbContext DbContext;

    public EfReadRepository(DbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
    {
        return await DbContext.Set<T>().FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<List<T>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<T>().ToListAsync(cancellationToken);
    }

    public async Task<List<T>> ListAsync(Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<T>().Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<T>().FirstOrDefaultAsync(predicate, cancellationToken);
    }
}