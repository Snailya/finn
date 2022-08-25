using System.Text;
using FINN.CORE.Interfaces;
using FINN.READER;
using FINN.SHAREDKERNEL;
using FINN.SHAREDKERNEL.Interfaces;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
var host = Host.CreateDefaultBuilder(args).ConfigureServices((context, collection) =>
{
    collection.AddSingleton<IBroker, RabbitMqBroker>();
    collection.AddSingleton<IDataTableReader, ExcelDataTableReader>();
    collection.AddHostedService<HostedService>();
}).Build();

await host.RunAsync();