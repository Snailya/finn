using ExcelDataReader;
using FINN.CORE.Interfaces;
using FINN.CORE.Models;
using FINN.EXCEL.Services;
using FINN.SHAREDKERNEL.Constants;
using FINN.SHAREDKERNEL.Dtos;

namespace FINN.EXCEL;

public class HostedService : BackgroundService
{
    private readonly IBroker _broker;
    private readonly ILogger<HostedService> _logger;
    private readonly ExcelDataTableReader _reader;

    public HostedService(ILogger<HostedService> logger, IBroker broker, ExcelDataTableReader reader)
    {
        _logger = logger;
        _broker = broker;
        _reader = reader;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Register handler for {Routing}", nameof(RoutingKeys.ExcelService));
        _broker.RegisterHandler(RoutingKeys.ExcelService.GetLayout, HandleGetLayout);

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

    private void HandleGetLayout(string routingKey, string correlationId, string filename)
    {
        using var stream = File.Open(filename, FileMode.Open, FileAccess.Read);
        using var excelDataReader = ExcelReaderFactory.CreateReader(stream);
        var spreadSheets = excelDataReader.AsDataSet().Tables;
        stream.Close();

        var layoutDto = new LayoutDto();
        if (spreadSheets.Contains("轴网"))
            (layoutDto.Grids, layoutDto.Platforms) =
                _reader.ReadAsGridSheet(spreadSheets["轴网"]!);

        if (spreadSheets.Contains("Process list"))
            layoutDto.Process = _reader.ReadAsProcessListSheet(spreadSheets["Process list"]!);


        var response = new Response<LayoutDto>("", 0, layoutDto);
        _broker.Reply(routingKey, correlationId, response.ToJson());
    }

    #endregion
}