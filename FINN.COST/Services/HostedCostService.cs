using System.Text.Json;
using FINN.CORE.Interfaces;
using FINN.CORE.Models;
using FINN.SHAREDKERNEL;
using FINN.SHAREDKERNEL.Constants;
using FINN.SHAREDKERNEL.Dtos;
using Microsoft.Extensions.Logging;

namespace FINN.COST.Services;

public class HostedCostService : HostedService
{
    private readonly IBroker _broker;
    private readonly CostService _service;

    public HostedCostService(ILogger<HostedCostService> logger, IBroker broker, CostService service) : base(logger)
    {
        _broker = broker;
        _service = service;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _broker.RegisterHandler(RoutingKeys.CostService.EstimateCost,
            HandleEstimateCost);

        await base.ExecuteAsync(stoppingToken);
    }

    #region Handlers

    private void HandleEstimateCost(string routingKey, string correlationId, string message)
    {
        var dto = JsonSerializer.Deserialize<IEnumerable<GeometryDto>>(message);
        if (dto == null) throw new ArgumentNullException();
        var cost = _service.EstimateCost(dto);
        var response = new Response<CostDto>("", 0, cost);
        _broker.Reply(routingKey, correlationId, response.ToJson());
    }

    #endregion
}