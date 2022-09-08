namespace FINN.PLUGINS.EFCORE;

public interface IRepositoryFactory<T> where T : class
{
    EfRepository<T> CreateRepository();
    EfReadRepository<T> CreateReadRepository();
}