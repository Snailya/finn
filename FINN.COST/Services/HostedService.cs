using System.Text.Json;
using FINN.CORE.Interfaces;
using FINN.COST.Services;
using FINN.SHAREDKERNEL.Constants;
using FINN.SHAREDKERNEL.UseCases;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FINN.COST;

public class HostedService : BackgroundService
{
    private readonly IBroker _broker;
    private readonly ILogger<HostedService> _logger;
    private readonly CostService _service;

    public HostedService(ILogger<HostedService> logger, IBroker broker, CostService service)
    {
        _logger = logger;
        _broker = broker;
        _service = service;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Register handler for {Routing}", nameof(RoutingKeys.CostService));

        _broker.RegisterHandler(RoutingKeys.CostService.EstimateCost,
            HandleEstimateCost);

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

    private void HandleEstimateCost(string routingKey, string correlationId, string message)
    {
        var dto = JsonSerializer.Deserialize<IEnumerable<GeometryDto>>(message);
        if (dto == null) throw new ArgumentNullException();
        var cost = _service.EstimateCost(dto);
    }

    #endregion
}