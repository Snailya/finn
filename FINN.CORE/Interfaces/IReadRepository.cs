using System.Linq.Expressions;
using FINN.CORE.Models;

namespace FINN.CORE.Interfaces;

public interface IReadRepository<T> : IDisposable
{
    /// <summary>
    ///     Get the entity by id.
    /// </summary>
    Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;

    /// <summary>
    ///     List all entities.
    /// </summary>
    Task<List<T>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     List entities at specified page.
    /// </summary>
    Task<List<T>> ListAsync(PaginationFilter filter, CancellationToken cancellationToken = default);

    /// <summary>
    ///     List entities that matches predicate.
    /// </summary>
    Task<List<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Find the first or default entity that matches the predicate.
    /// </summary>
    Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}