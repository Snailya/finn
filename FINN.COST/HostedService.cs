using FINN.CORE.Interfaces;
using FINN.SHAREDKERNEL.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FINN.COST;

public class HostedService : BackgroundService
{
    private readonly IBroker _broker;
    private readonly ILogger<HostedService> _logger;

    public HostedService(ILogger<HostedService> logger, IConfiguration configuration, IBroker broker)
    {
        _logger = logger;
        _broker = broker;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Register handler for {Routing}", RoutingKeys.Estimate);

        _broker.RegisterHandler(RoutingKeys.Estimate, (routingKey, correlationId, message) => HandleEstimate(message));

        // monitor service status
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogTrace(
                "{Service} is running at {Time}... Next check is in {Duration}s", nameof(HostedService),
                DateTime.Now.ToLocalTime(), 30);
            await Task.Delay(30000, stoppingToken);
        }

        _logger.LogInformation("Connection closed");
    }

    #region Handlers

    private void HandleEstimate(string message)
    {
    }

    #endregion
}