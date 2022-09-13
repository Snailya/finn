namespace FINN.CORE.Interfaces;

public interface IRepositoryFactory<T> where T : class
{
    /// <summary>
    ///     Create a repository of specified entity.
    /// </summary>
    IRepository<T> CreateRepository();

    /// <summary>
    ///     Create a readonly repository of specified entity.
    /// </summary>
    IReadRepository<T> CreateReadRepository();
}