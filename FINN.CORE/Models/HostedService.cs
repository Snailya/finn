using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FINN.CORE.Models;

public abstract class HostedService : BackgroundService
{
    private readonly ILogger<HostedService> _logger;
    private readonly int _checkInterval;

    protected HostedService(ILogger<HostedService> logger, int checkInterval = 600)
    {
        _logger = logger;
        _checkInterval = checkInterval;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("[{DateTime}] Running status checked. Next check in {Time}s", DateTime.Now,
                _checkInterval);
            await Task.Delay(_checkInterval * 1000, stoppingToken);
        }

        _logger.LogInformation("[{DateTime}] Connection closed", DateTime.Now);
    }
}