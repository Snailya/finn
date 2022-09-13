using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FINN.REPOSITORY.EFCORE;

public static class Extension
{
    /// <summary>
    /// Registers the given context as a service in the IServiceCollection using sqlite database
    /// </summary>
    public static void AddDbContext<T>(this IServiceCollection services, string connectionString) where T : DbContext
    {
        services.AddDbContext<T>(options =>
            options.UseSqlite(connectionString));
    }

    /// <summary>
    /// Registers an IDbContextFactory in the IServiceCollection to create instances of given DbContext type using sqlite database
    /// </summary>
    public static void AddDbContextFactory<T>(this IServiceCollection services, string connectionString)
        where T : DbContext
    {
        services.AddDbContextFactory<T>(options =>
            options.UseSqlite(connectionString));
    }
}