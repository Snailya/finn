using System.Text.Json;
using FINN.DRAFTER.Contexts;
using FINN.DRAFTER.Extensions;
using FINN.DRAFTER.Models;
using FINN.DRAFTER.Utils;
using FINN.SHAREDKERNEL;
using FINN.SHAREDKERNEL.Dtos;
using FINN.SHAREDKERNEL.Dtos.Draw;
using FINN.SHAREDKERNEL.Dtos.InsertBlock;
using FINN.SHAREDKERNEL.Dtos.UpdateJobStatus;
using FINN.SHAREDKERNEL.Interfaces;
using FINN.SHAREDKERNEL.Models;
using Microsoft.EntityFrameworkCore;
using netDxf;
using netDxf.Blocks;
using netDxf.Entities;

namespace FINN.DRAFTER;

public class HostedService : BackgroundService
{
    private readonly IBroker _broker;
    private readonly IDbContextFactory<BlockContext> _factory;
    private readonly ILogger<HostedService> _logger;
    private readonly string _blockFolder;

    public HostedService(ILogger<HostedService> logger, IConfiguration configuration, IBroker broker,
        IDbContextFactory<BlockContext> factory)
    {
        _logger = logger;
        _broker = broker;
        _factory = factory;
        _blockFolder = configuration["Storage:Blocks"];
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Register handler for {Routing}", RoutingKey.Draw);

        _broker.RegisterHandler(RoutingKey.Draw, (routingKey, correlationId, message) => HandleDraw(message));
        _broker.RegisterHandler(RoutingKey.InsertBlock, HandleInsertBlock);

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

            var doc = DxfDocument.Load(dto.Filename);

            var context = _factory.CreateDbContext();

            var names = dto.Names.Any()
                ? dto.Names.ToList()
                : doc.Blocks.Items.Where(x =>
                        !x.Name.StartsWith("*") && !x.Name.StartsWith("_"))
                    .Select(x => x.Name).ToList();

            foreach (var name in names.Where(name =>
                         context.BlockDefinitions.SingleOrDefault(x => x.Name == name) != null))
                throw new ArgumentException(
                    "Block with the same name already exist. Please check the content and consider using update.",
                    nameof(name));

            var blocks = names.Select(name =>
            {
                // explode
                var origin = doc.Blocks[name];
                var entities = origin.ExplodeIteratively();
                var exploded = new Block(name, entities);

                // save as individual files
                var file = new DxfDocument();
                file.Blocks.Add(exploded);

                var fileName = Path.Join(_blockFolder,
                    Path.GetFileName(Path.GetTempFileName()).Replace(".tmp", ".dxf"));
                file.Save(fileName);

                return new BlockDefinition { Name = name, DxfFileName = fileName };
            }).ToList();
            context.BlockDefinitions.AddRange(blocks);
            context.SaveChanges();

            // Prepare response for successful call
            response = new Response<InsertBlockResponseDto>("success", 0,
                new InsertBlockResponseDto
                    { Blocks = blocks.Select(x => new BlockDefinitionDto() { Id = x.Id, Name = x.Name }) });
        }
        catch (Exception ex)
        {
            response = new Response(ex.Message, ErrorCode.BlockOfSameNameAlreadyExist);
        }
        finally
        {
            if (response != null) _broker.Reply(routingKey, correlationId, response.ToJson());
        }
    }

    private void HandleDraw(string message)
    {
        var drafterDto = JsonSerializer.Deserialize<DrafterDto>(message);

        // update status
        _broker.Send(RoutingKey.UpdateJobStatus,
            new UpdateJobStatusDto(drafterDto!.Id, JobStatus.Drawing).ToJson());
        try
        {
            const int gutter = 3200;

            var dxf = DocUtil.CreateDoc();

            var canvas = new Group(Vector2d.Zero, GroupDirection.TopToBottom, GroupAlignment.Start, gutter * 4);

            // draw grids
            var grids = drafterDto.Grids.ToGridGroup(Vector2d.Zero);
            canvas.Add(grids);

            var layouts = new Group(Vector2d.Zero, GroupDirection.TopToBottom, GroupAlignment.Start, gutter);
            drafterDto.Process.ToList().ForEach(dto =>
            {
                var layoutItem = new Group(Vector2d.Zero, GroupDirection.LeftToRight, GroupAlignment.Middle, gutter);
                layoutItem.Add(Booth.FromDto(dto));

                // divide into booth and blocks
                var context = _factory.CreateDbContext();
                var blocks = dto.SubProcess.Where(x =>
                    x.XLength == 0 && x.YLength == 0 &&
                    context.BlockDefinitions.SingleOrDefault(bd => bd.Name == x.Name) != null).ToList();
                var booths = dto.SubProcess.Except(blocks).ToList();

                // handle booths first
                if (booths.Count > 0)
                {
                    var boothGroup = dto.SubProcess.ToBoothGroup(Vector2d.Zero);
                    layoutItem.Add(boothGroup);
                }

                // handle blocks
                if (blocks.Count > 0)
                {
                    var blockGroup = new Group(Vector2d.Zero, GroupDirection.LeftToRight, GroupAlignment.Middle,
                        gutter);
                    blocks.ForEach(x =>
                    {
                        var bd = context.BlockDefinitions.Single(bd => bd.Name == x.Name);
                        var doc = DxfDocument.Load(bd.DxfFileName);
                        var block = doc.Blocks.Items.Single(b => b.Name == bd.Name);
                        var insert = new Insert(block);
                        var blockWrapper = new SimpleWrapper(insert);
                        blockGroup.Add(blockWrapper);
                    });
                    layoutItem.Add(blockGroup);
                }

                layouts.Add(layoutItem);
            });
            canvas.Add(layouts);

            // draw plates
            foreach (var grid in grids.Items)
            {
                var plate = drafterDto.Plates.SingleOrDefault(x => Math.Abs(x.Level - grid.Level) < double.Epsilon);
                if (plate != null && plate.Blocks.Any())
                {
                    foreach (var blockDto in plate.Blocks)
                    {
                        var block = new PlatformBlock(new Vector2d(blockDto.Placement.X, blockDto.Placement.Y),
                            blockDto.XLength, blockDto.YLength,
                            plate.Level);
                        block.BasePoint += grid.Origin;
                        dxf.Add(block);
                    }
                }
            }

            dxf.Add(canvas);

            var output = Path.GetTempFileName().Replace(".tmp", ".dxf");
            dxf.Save(output);

            // update status
            _broker.Send(RoutingKey.UpdateJobStatus,
                new UpdateJobStatusDto(drafterDto.Id, output).ToJson());
        }
        catch (Exception e)
        {
            _logger.LogError(e.InnerException.StackTrace);

            _broker.Send(RoutingKey.UpdateJobStatus,
                new UpdateJobStatusDto(drafterDto.Id, JobStatus.Error).ToJson());
        }
    }

    #endregion
}