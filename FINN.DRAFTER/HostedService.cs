using System.Text.Json;
using FINN.CORE.Interfaces;
using FINN.CORE.Models;
using FINN.SHAREDKERNEL.Constants;
using FINN.SHAREDKERNEL.Dtos;
using FINN.SHAREDKERNEL.Dtos.Drafter;
using FINN.SHAREDKERNEL.Dtos.Management;
using FINN.SHAREDKERNEL.Interfaces;


namespace FINN.DRAFTER;

public class HostedService : BackgroundService
{
    private readonly IBroker _broker;
    private readonly IReadWriteDxf _readWriteDxf;
    private readonly ILogger<HostedService> _logger;

    public HostedService(ILogger<HostedService> logger, IBroker broker, IReadWriteDxf readWriteDxf)
    {
        _logger = logger;
        _broker = broker;
        _readWriteDxf = readWriteDxf;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Register handler for {Routing}", RoutingKeys.Draw);

        _broker.RegisterHandler(RoutingKeys.Draw, (routingKey, correlationId, message) => HandleDraw(message));
        _broker.RegisterHandler(RoutingKeys.InsertBlock, HandleInsertBlock);

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

    private void HandleInsertBlock(string routingKey, string correlationId, string message)
    {
        Response? response = null;

        try
        {
            var dto =
                JsonSerializer.Deserialize<InsertBlockRequestDto>(message);

            var blocks = _readWriteDxf.InsertBlockDefinitions(dto);

            // Prepare response for successful call
            response = new Response<InsertBlockResponseDto>("success", 0,
                new InsertBlockResponseDto
                    { Blocks = blocks.Select(x => new BlockDefinitionDto { Id = x.Id, Name = x.Name }) });
        }
        catch (Exception e)
        {
            response = new Response(e.InnerException?.Message ?? e.Message, ErrorCodes.BlockOfSameNameAlreadyExist);
        }
        finally
        {
            if (response != null) _broker.Reply(routingKey, correlationId, response.ToJson());
        }
    }


    private void HandleDraw(string message)
    {
        var jobStatus = new UpdateJobStatusRequestDto() { Status = JobStatus.Drawing };

        try
        {
            var drawRequestDto = JsonSerializer.Deserialize<DrawLayoutRequestDto>(message)!;
            jobStatus.Id = drawRequestDto.Id;
            _broker.Send(RoutingKeys.UpdateJobStatus, jobStatus.ToJson());

            jobStatus.Output = _readWriteDxf.DrawLayout(drawRequestDto);
            jobStatus.Status = JobStatus.Ready;
            _broker.Send(RoutingKeys.UpdateJobStatus, jobStatus.ToJson());
        }
        catch (Exception e)
        {
            _logger.LogError("{ErrorMessage}", e.InnerException?.Message ?? e.Message);

            jobStatus.Status = JobStatus.Error;
            _broker.Send(RoutingKeys.UpdateJobStatus, jobStatus.ToJson());
        }
    }

    #endregion
}