using System.Text;
using System.Text.Json;
using FINN.DRAFTER.Extensions;
using FINN.DRAFTER.Model;
using FINN.DRAFTER.Utils;
using FINN.SHAREDKERNEL;
using FINN.SHAREDKERNEL.Dtos;
using FINN.SHAREDKERNEL.Models;

namespace FINN.DRAFTER;

public class HostedService : BackgroundService
{
    private const double Gutter = 1600;
    private readonly IBroker _broker;
    private readonly ILogger<HostedService> _logger;

    public HostedService(ILogger<HostedService> logger, IBroker broker)
    {
        _logger = logger;
        _broker = broker;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Register handler for {Routing}", RoutingKey.Draw);

        _broker.RegisterHandler("drafter", DrawAndSave);

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

    private void DrawAndSave(ReadOnlyMemory<byte> memory)
    {
        var drafterDto = JsonSerializer.Deserialize<DrafterDto>(Encoding.UTF8.GetString(memory.ToArray()));

        // update status
        _broker.Send(RoutingKey.UpdateJobStatus,
            Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(new UpdateJobStatusDto(drafterDto!.Id, JobStatus.Drawing))));
        try
        {
            // do business
            var dxf = DocUtil.CreateDoc();

            var canvas = new Group(Vector2d.Zero, GroupDirection.TopToBottom, GroupAlignment.Start, Gutter);

            var grids = drafterDto.Grids.ToGridGroup(Vector2d.Zero);
            canvas.Add(grids);

            var layouts = new Group(Vector2d.Zero, GroupDirection.TopToBottom, GroupAlignment.Start, Gutter);
            drafterDto.Process.ToList().ForEach(x =>
            {
                var layoutItem = new Group(Vector2d.Zero, GroupDirection.LeftToRight, GroupAlignment.Middle, Gutter);
                layoutItem.Add(Booth.FromDto(x));
                var sub = x.SubProcess.ToList();
                if (sub.Count <= 0) return;
                var boothGroup = x.SubProcess.ToBoothGroup(Vector2d.Zero);
                layoutItem.Add(boothGroup);

                layouts.Add(layoutItem);
            });
            canvas.Add(layouts);

            dxf.Add(canvas);

            var output = Path.GetTempFileName().Replace(".tmp", ".dxf");
            dxf.Save(output);

            // update status
            _broker.Send(RoutingKey.UpdateJobStatus,
                Encoding.UTF8.GetBytes(
                    JsonSerializer.Serialize(new UpdateJobStatusDto(drafterDto.Id, output))));
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);

            _broker.Send(RoutingKey.UpdateJobStatus,
                Encoding.UTF8.GetBytes(
                    JsonSerializer.Serialize(new UpdateJobStatusDto(drafterDto.Id, JobStatus.Error))));
        }
    }
}