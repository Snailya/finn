using System.Text;
using System.Text.Json;
using ExcelDataReader;
using FINN.SHAREDKERNEL;
using FINN.SHAREDKERNEL.Dtos;
using FINN.SHAREDKERNEL.Interfaces;
using FINN.SHAREDKERNEL.Models;

namespace FINN.READER;

public class HostedService : BackgroundService
{
    private readonly IBroker _broker;
    private readonly ILogger<HostedService> _logger;
    private readonly IReader _reader;

    public HostedService(ILogger<HostedService> logger, IBroker broker,
        IReader reader)
    {
        _logger = logger;
        _broker = broker;
        _reader = reader;
    }

    private void ReadAndHandOver(ReadOnlyMemory<byte> memory)
    {
        var readerDto = JsonSerializer.Deserialize<ReaderDto>(Encoding.UTF8.GetString(memory.ToArray()));

        // update status
        _broker.Send(RoutingKey.UpdateJobStatus,
            Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new UpdateJobStatusDto(readerDto!.Id, JobStatus.Reading))));

        // do business logic
        try
        {
            using var stream = File.Open(readerDto.Input, FileMode.Open, FileAccess.Read);
            using var excelDataReader = ExcelReaderFactory.CreateReader(stream);
            var spreadSheets = excelDataReader.AsDataSet().Tables;

            // validate spreadsheet
            var draftDto = new DrafterDto { Id = readerDto.Id };
            if (spreadSheets.Contains("轴网")) draftDto.Grids = _reader.ReadAsGrid(spreadSheets["轴网"]!);

            if (spreadSheets.Contains("Process list"))
                draftDto.Process = _reader.ReadAsProcessList(spreadSheets["Process list"]!);

            // hand over
            _broker.Send(RoutingKey.Draw, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(draftDto)));
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);

            _broker.Send(RoutingKey.UpdateJobStatus,
                Encoding.UTF8.GetBytes(
                    JsonSerializer.Serialize(new UpdateJobStatusDto(readerDto.Id, JobStatus.Error))));
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Register handler for {Routing}", RoutingKey.Read);

        _broker.RegisterHandler(RoutingKey.Read, ReadAndHandOver);

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogTrace(
                "{Service} is running at {Time}... Next check is in {Duration}s", nameof(HostedService),
                DateTime.Now.ToLocalTime(), 30);
            await Task.Delay(30000, stoppingToken);
        }

        _logger.LogInformation("Connection closed");
    }
}