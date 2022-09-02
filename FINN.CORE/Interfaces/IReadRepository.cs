using System.Linq.Expressions;
using FINN.CORE.Models;

namespace FINN.CORE.Interfaces;

public interface IReadRepository<T>
{
    /// <summary>
    ///     Get the entity by id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TId"></typeparam>
    /// <returns></returns>
    Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;

    /// <summary>
    ///     List all entities.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<T>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     List entities at specified page.
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<T>> ListAsync(PaginationFilter filter, CancellationToken cancellationToken = default);

    /// <summary>
    ///     List entities that matches predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Find the first or default entity that matches the predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}