using ExcelDataReader;
using FINN.CORE.Interfaces;
using FINN.CORE.Models;
using FINN.EXCEL.Services;
using FINN.SHAREDKERNEL;
using FINN.SHAREDKERNEL.Constants;
using FINN.SHAREDKERNEL.Dtos;

namespace FINN.EXCEL;

public class HostedExcelService : HostedService
{
    private readonly IBroker _broker;
    private readonly ExcelDataTableReader _reader;

    public HostedExcelService(ILogger<HostedExcelService> logger, IBroker broker, ExcelDataTableReader reader) :
        base(logger)
    {
        _broker = broker;
        _reader = reader;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _broker.RegisterHandler(RoutingKeys.ExcelService.GetLayout, HandleGetLayout);

        await base.ExecuteAsync(stoppingToken);
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