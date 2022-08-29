using FINN.CORE.Interfaces;
using FINN.COST;
using FINN.COST.Data;
using FINN.COST.Models;
using FINN.COST.Services;
using FINN.PLUGINS.EFCORE;
using FINN.SHAREDKERNEL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args).ConfigureServices((context, collection) =>
{
    collection.AddSingleton<IBroker, RabbitMqBroker>();
    collection.AddDbContext<AppDbContext>(context.Configuration.GetConnectionString("SqliteConnection"));
    collection.AddScoped<DbContext, AppDbContext>();
    collection.AddScoped<IRepository<Cost>, EfRepository<Cost>>();
    collection.AddSingleton<CostService>();
    collection.AddHostedService<HostedService>();
}).Build();

await host.RunAsync();