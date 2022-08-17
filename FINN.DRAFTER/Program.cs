using FINN.CORE;
using FINN.DRAFTER;
using FINN.DRAFTER.Contexts;
using FINN.SHAREDKERNEL.Interfaces;
using Microsoft.EntityFrameworkCore;

var host = Host.CreateDefaultBuilder(args).ConfigureServices((context, collection) =>
{
    collection.AddSingleton<IBroker, RabbitMqBroker>();
    collection.AddDbContextFactory<BlockContext>(options =>
        options.UseSqlite(context.Configuration.GetConnectionString("BlockContextSQLite")));
    collection.AddHostedService<HostedService>();
}).Build();

// Create database
using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<BlockContext>();
    context.Database.EnsureCreated();
}

await host.RunAsync();