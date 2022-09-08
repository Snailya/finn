using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace FINN.PLUGINS.EFCORE;

public static class Extension
{
    public static void AddDbContext<T>(this IServiceCollection services, string connectionString) where T : DbContext
    {
        services.AddDbContext<T>(options =>
            options.UseSqlite(connectionString));
    }

    public static void AddDbContextFactory<T>(this IServiceCollection services, string connectionString)
        where T : DbContext
    {
        services.AddDbContextFactory<T>(options =>
            options.UseSqlite(connectionString));
    }
}