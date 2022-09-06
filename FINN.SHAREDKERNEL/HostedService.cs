using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FINN.SHAREDKERNEL;

public abstract class HostedService : BackgroundService
{
    protected readonly ILogger<HostedService> Logger;
    private const int CheckInterval = 600;

    protected HostedService(ILogger<HostedService> logger)
    {
        Logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Logger.LogInformation("[{DateTime}] Running status checked. Next check in {Time}s", DateTime.Now,
                CheckInterval);
            await Task.Delay(CheckInterval * 1000, stoppingToken);
        }

        Logger.LogInformation("[{DateTime}] Connection closed", DateTime.Now);
    }
}