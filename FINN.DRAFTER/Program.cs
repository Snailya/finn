using FINN.CORE.Interfaces;
using FINN.CORE.Models;
using FINN.DRAFTER;
using FINN.PLUGINS.DXF;
using FINN.PLUGINS.EFCORE;
using FINN.PLUGINS.EFCORE.Data;
using FINN.SHAREDKERNEL;
using FINN.SHAREDKERNEL.Interfaces;

var host = Host.CreateDefaultBuilder(args).ConfigureServices((context, collection) =>
{
    collection.AddSingleton<IBroker, RabbitMqBroker>();
    collection.AddDbContext(context.Configuration.GetConnectionString("SqliteConnection"));
    collection.AddScoped<IRepository<BlockDefinition>, EfRepository<BlockDefinition>>();
    collection.AddSingleton<IReadWriteDxf, NetReadWriteDxf>();
    collection.AddHostedService<HostedService>();
}).Build();

// Create database
using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

await host.RunAsync();