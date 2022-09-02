using System.Text.Json;
using FINN.CORE.Interfaces;
using FINN.CORE.Models;
using FINN.SHAREDKERNEL.Constants;
using FINN.SHAREDKERNEL.Dtos;
using FINN.SHAREDKERNEL.Interfaces;

namespace FINN.DXF;

public class HostedService : BackgroundService
{
    private readonly IBroker _broker;
    private readonly ILogger<HostedService> _logger;
    private readonly IDxfService _service;

    public HostedService(ILogger<HostedService> logger, IBroker broker, IDxfService service)
    {
        _logger = logger;
        _broker = broker;
        _service = service;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Register handler for {Routing}", nameof(RoutingKeys.DxfService));

        _broker.RegisterHandler(RoutingKeys.DxfService.ListBlockDefinitions, HandleListBlockDefinitions);
        _broker.RegisterHandler(RoutingKeys.DxfService.GetBlockDefinition, HandleGetBlockDefinition);
        _broker.RegisterHandler(RoutingKeys.DxfService.AddBlockDefinitions, HandleAddBlockDefinitions);
        _broker.RegisterHandler(RoutingKeys.DxfService.DeleteBlockDefinition, HandleDeleteBlockDefinition);
        _broker.RegisterHandler(RoutingKeys.DxfService.DrawLayout, HandleDrawLayout);
        _broker.RegisterHandler(RoutingKeys.DxfService.ReadLayout, HandleReadLayout);

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

    private void HandleListBlockDefinitions(string routingKey, string correlationId, string message)
    {
        var blockDefinitions = _service.ListBlockDefinitions();
        var response = new Response<IEnumerable<BlockDefinitionDto>>("", 0, blockDefinitions);
        _broker.Reply(routingKey, correlationId, response.ToJson());
    }

    #region Handlers

    private void HandleDrawLayout(string routingKey, string correlationId, string message)
    {
        var dto = JsonSerializer.Deserialize<LayoutDto>(message);
        if (dto == null) throw new ArgumentNullException();
        var filename = _service.DrawLayout(dto);
        var response = new Response<string>("", 0, filename);
        _broker.Reply(routingKey, correlationId, response.ToJson());
    }

    private void HandleReadLayout(string routingKey, string correlationId, string filename)
    {
        _logger.LogInformation("Read layout from {Filename}", filename);

        var geometries = _service.ReadLayout(filename);
        var response = new Response<IEnumerable<GeometryDto>>("", 0, geometries);
        _broker.Reply(routingKey, correlationId, response.ToJson());
    }

    private void HandleDeleteBlockDefinition(string routingKey, string correlationId, string idStr)
    {
        var id = int.Parse(idStr);
        _service.DeleteBlockDefinitionById(id);
        var response = new Response("", 0);
        _broker.Reply(routingKey, correlationId, response.ToJson());
    }

    private void HandleAddBlockDefinitions(string routingKey, string correlationId, string message)
    {
        var dto = JsonSerializer.Deserialize<AddBlockDefinitionsDto>(message);
        if (dto == null || string.IsNullOrEmpty(dto.Filename)) throw new ArgumentException();
        var blockDefinitions = _service.AddBlockDefinitions(dto.Filename, dto.BlockNames);
        var response = new Response<IEnumerable<BlockDefinitionDto>>("", 0, blockDefinitions);
        _broker.Reply(routingKey, correlationId, response.ToJson());
    }

    private void HandleGetBlockDefinition(string routingKey, string correlationId, string idStr)
    {
        var id = int.Parse(idStr);
        var blockDefinition = _service.GetBlockDefinition(id);
        var response = new Response<BlockDefinitionDto>("", 0, blockDefinition);
        _broker.Reply(routingKey, correlationId, response.ToJson());
    }

    #endregion
}