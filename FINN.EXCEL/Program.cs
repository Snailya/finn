using System.Text;
using FINN.BROKER.RABBITMQ;
using FINN.CORE.Interfaces;
using FINN.EXCEL.Services;
using FINN.SHAREDKERNEL.Interfaces;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
var host = Host.CreateDefaultBuilder(args).ConfigureServices((context, collection) =>
{
    collection.AddSingleton<IBroker, RabbitMqBroker>();
    collection.AddSingleton<IExcelService, ExcelService>();
    collection.AddHostedService<HostedExcelService>();
}).Build();

await host.RunAsync();