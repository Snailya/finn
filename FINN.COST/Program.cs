using FINN.CORE.Interfaces;
using FINN.COST.Data;
using FINN.COST.Models;
using FINN.COST.Services;
using FINN.PLUGINS.EFCORE;
using FINN.SHAREDKERNEL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args).ConfigureServices((context, collection) =>
{
    collection.AddSingleton<IBroker, RabbitMqBroker>();
    collection.AddDbContextFactory<AppDbContext>(context.Configuration.GetConnectionString("SqliteConnection"));
    collection.AddScoped<IRepositoryFactory<Formula>, RepositoryFactory<Formula, AppDbContext>>();
    collection.AddSingleton<CostService>();
    collection.AddHostedService<HostedCostService>();
}).Build();

// Create database
using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

await host.RunAsync();