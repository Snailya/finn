using System.Text.Json;
using ExcelDataReader;
using FINN.CORE.Interfaces;
using FINN.CORE.Models;
using FINN.SHAREDKERNEL.Constants;
using FINN.SHAREDKERNEL.Dtos.Drafter;
using FINN.SHAREDKERNEL.Dtos.Management;
using FINN.SHAREDKERNEL.Dtos.Reader;
using FINN.SHAREDKERNEL.Interfaces;

namespace FINN.READER;

public class HostedService : BackgroundService
{
    private readonly IBroker _broker;
    private readonly IDataTableReader _dataTableReader;
    private readonly ILogger<HostedService> _logger;

    public HostedService(ILogger<HostedService> logger, IBroker broker,
        IDataTableReader dataTableReader)
    {
        _logger = logger;
        _broker = broker;
        _dataTableReader = dataTableReader;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Register handler for {Routing}", RoutingKeys.ReadXlsx);

        _broker.RegisterHandler(RoutingKeys.ReadXlsx, (routingKey, correlationId, message) => HandleReadXlsx(message));
        _broker.RegisterHandler(RoutingKeys.ReadDxf, (routingKey, correlationId, message) => HandleReadDxf(message));

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

    private void HandleReadXlsx(string message)
    {
        var request = JsonSerializer.Deserialize<ReadRequestDto>(message);

        UpdateJobStatus(request.JobId, JobStatus.Reading);

        try
        {
            using var stream = File.Open(request.Filename, FileMode.Open, FileAccess.Read);
            using var excelDataReader = ExcelReaderFactory.CreateReader(stream);
            var spreadSheets = excelDataReader.AsDataSet().Tables;

            // validate spreadsheet
            var drawRequestDto = new DrawLayoutRequestDto { Id = request.JobId };
            if (spreadSheets.Contains("轴网"))
                (drawRequestDto.Grids, drawRequestDto.Platforms) =
                    _dataTableReader.ReadAsGridSheet(spreadSheets["轴网"]!);

            if (spreadSheets.Contains("Process list"))
                drawRequestDto.Process = _dataTableReader.ReadAsProcessListSheet(spreadSheets["Process list"]!);

            // hand over
            _broker.Send(RoutingKeys.Draw, drawRequestDto.ToJson());
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            UpdateJobStatus(request.JobId, JobStatus.Error);
        }
    }

    private void HandleReadDxf(string message)
    {
        var request = JsonSerializer.Deserialize<ReadRequestDto>(message);

        UpdateJobStatus(request.JobId, JobStatus.Reading);

        try
        {
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            UpdateJobStatus(request.JobId, JobStatus.Error);
        }
    }

    private void UpdateJobStatus(int id, JobStatus status)
    {
        // update status
        _broker.Send(RoutingKeys.UpdateJobStatus, new UpdateJobStatusRequestDto(id, status).ToJson());
    }

    #endregion
}