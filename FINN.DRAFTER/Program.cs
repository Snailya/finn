using FINN.CORE;
using FINN.DRAFTER;
using FINN.SHAREDKERNEL;

var host = Host.CreateDefaultBuilder(args).ConfigureServices((context, collection) =>
{
    collection.AddSingleton<IBroker, RabbitMqBroker>();
    collection.AddHostedService<HostedService>();
}).Build();

await host.RunAsync();