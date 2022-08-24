using FINN.PLUGINS.EFCORE.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FINN.PLUGINS.EFCORE;

public static class Extension
{
    public static void AddDbContext(this IServiceCollection services, string connectionString) =>
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString)); // will be created in web project root
}