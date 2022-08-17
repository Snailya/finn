using System.Text;
using FINN.CORE;
using FINN.READER;
using FINN.SHAREDKERNEL.Interfaces;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
var host = Host.CreateDefaultBuilder(args).ConfigureServices((context, collection) =>
{
    collection.AddSingleton<IBroker, RabbitMqBroker>();
    collection.AddSingleton<IReader, ExcelReader>();
    collection.AddHostedService<HostedService>();
}).Build();

await host.RunAsync();