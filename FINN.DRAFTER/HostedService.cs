using System.Text;
using System.Text.Json;
using FINN.DRAFTER.Model;
using FINN.DRAFTER.Utils;
using FINN.SHAREDKERNEL;
using FINN.SHAREDKERNEL.Dtos;
using FINN.SHAREDKERNEL.Models;
using netDxf;

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
            var dxf = new DxfDocument();

            // draw grids
            var location = Vector2d.Zero;
            var gridGroup = new Group<Grid>(location, GroupDirection.BottomToTop, GroupAlignment.Start, Gutter * 4);
            foreach (var gridDto in drafterDto.Grids)
            {
                var grid = new Grid(gridDto.XCoordinates, gridDto.YCoordinates,
                    gridDto.ColumnXLength,
                    gridDto.ColumnYLength);

                gridGroup.Add(grid);
            }

            dxf.Add(gridGroup);

            // draw layout items
            location = gridGroup.Box.Min;
            foreach (var processDto in drafterDto.Process)
            {
                // step half of process' y length
                var sub = processDto.SubProcess.ToList();
                var yStep = sub.Select(x => x.YLength).Append(processDto.YLength).Append(Gutter).Max() / 2;
                location -= new Vector2d(0, yStep);

                // draw primary
                var primary = processDto.XLength == 0 || processDto.YLength == 0
                    ? new Booth(LayerUtil.GetLayerByName(processDto.Layer), location, processDto.Name)
                    : new Booth(LayerUtil.GetLayerByName(processDto.Layer), location, processDto.XLength,
                        processDto.YLength,
                        processDto.Line1, processDto.Line2);
                dxf.Add(primary);

                // step half of process's x length
                var subLocation = new Vector2d(primary.Box.Max.X + Gutter, location.Y);

                // draw sub process
                if (sub.Count > 0)
                {
                    var boothGroup =
                        new Group<Booth>(subLocation, GroupDirection.LeftToRight, GroupAlignment.Middle, 0);
                    foreach (var subDto in processDto.SubProcess)
                    {
                        var booth = subDto.XLength == 0 || subDto.YLength == 0
                            ? new Booth(LayerUtil.GetLayerByName(subDto.Layer), subDto.Name)
                            : new Booth(LayerUtil.GetLayerByName(subDto.Layer), subDto.XLength, subDto.YLength,
                                subDto.Line1, subDto.Line2);

                        boothGroup.Add(booth);
                    }

                    dxf.Add(boothGroup);
                }

                // step half of process' y length
                location -= new Vector2d(0, yStep + Gutter);
            }

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