using System.Linq.Expressions;

namespace FINN.CORE.Interfaces;

public interface IReadRepository<T>
{
    Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;
    Task<List<T>> ListAsync(CancellationToken cancellationToken = default);
    Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}