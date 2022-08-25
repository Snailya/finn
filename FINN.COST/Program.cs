using FINN.CORE.Interfaces;
using FINN.COST;
using FINN.SHAREDKERNEL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args).ConfigureServices((context, collection) =>
{
    collection.AddSingleton<IBroker, RabbitMqBroker>();
    collection.AddHostedService<HostedService>();
}).Build();

await host.RunAsync();