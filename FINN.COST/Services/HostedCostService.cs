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
        _broker.RegisterHandler(RoutingKeys.CostService.ListFormulas, HandleListFormulas);
        _broker.RegisterHandler(RoutingKeys.CostService.AddFormula, HandleAddFormula);
        _broker.RegisterHandler(RoutingKeys.CostService.UpdateFormula, HandleUpdateFormula);
        _broker.RegisterHandler(RoutingKeys.CostService.DeleteFormula, HandleDeleteFormula);
        _broker.RegisterHandler(RoutingKeys.CostService.EstimateCost, HandleEstimateCost);

        await base.ExecuteAsync(stoppingToken);
    }


    #region Handlers

    private void HandleListFormulas(string routingKey, string correlationId, string message)
    {
        var formulas = _service.ListFormulas();
        var response =
            new Response<IEnumerable<FormulaDto>>("", 0, formulas);
        _broker.Reply(routingKey, correlationId, response.ToJson());
    }

    private void HandleUpdateFormula(string routingKey, string correlationId, string message)
    {
        var dto = JsonSerializer.Deserialize<FormulaDto>(message);
        if (dto == null) throw new ArgumentNullException();
        var formulas = _service.UpdateFormula(dto);
        var response = new Response<FormulaDto>("", 0, formulas);
        _broker.Reply(routingKey, correlationId, response.ToJson());
    }

    private void HandleAddFormula(string routingKey, string correlationId, string message)
    {
        var dto = JsonSerializer.Deserialize<FormulaDto>(message);
        if (dto == null) throw new ArgumentNullException();
        var formulas = _service.AddFormula(dto);
        var response = new Response<FormulaDto>("", 0, formulas);
        _broker.Reply(routingKey, correlationId, response.ToJson());
    }

    private void HandleDeleteFormula(string routingKey, string correlationId, string idStr)
    {
        var id = int.Parse(idStr);
        _service.DeleteFormulaById(id);
        var response = new Response("", 0);
        _broker.Reply(routingKey, correlationId, response.ToJson());
    }

    private void HandleEstimateCost(string routingKey, string correlationId, string message)
    {
        var dto = JsonSerializer.Deserialize<IEnumerable<GeometryDto>>(message);
        if (dto == null) throw new ArgumentNullException();
        var cost = _service.EstimateCost(dto);
        var response = new Response<IEnumerable<CostDto>>("", 0, cost);
        _broker.Reply(routingKey, correlationId, response.ToJson());
    }

    #endregion
}