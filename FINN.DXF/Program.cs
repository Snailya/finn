using FINN.CORE.Interfaces;
using FINN.DXF.Data;
using FINN.DXF.Models;
using FINN.DXF.Services;
using FINN.PLUGINS.EFCORE;
using FINN.SHAREDKERNEL;
using FINN.SHAREDKERNEL.Interfaces;
using Microsoft.EntityFrameworkCore;

var host = Host.CreateDefaultBuilder(args).ConfigureServices((context, collection) =>
{
    collection.AddSingleton<IBroker, RabbitMqBroker>();
    collection.AddDbContext<AppDbContext>(context.Configuration.GetConnectionString("SqliteConnection"));
    collection.AddScoped<DbContext, AppDbContext>();
    collection.AddScoped<IRepository<BlockDefinition>, EfRepository<BlockDefinition>>();
    collection.AddSingleton<IDxfService, NetDxfService>();
    collection.AddHostedService<HostedDxfService>();
}).Build();

// Create database
using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

await host.RunAsync();