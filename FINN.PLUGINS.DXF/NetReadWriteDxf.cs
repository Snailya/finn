using FINN.CORE.Interfaces;
using FINN.CORE.Models;
using FINN.PLUGINS.DXF.Models;
using FINN.PLUGINS.DXF.Utils;
using FINN.SHAREDKERNEL.Constants;
using FINN.SHAREDKERNEL.Dtos.Cost;
using FINN.SHAREDKERNEL.Dtos.Drafter;
using FINN.SHAREDKERNEL.Dtos.Management;
using FINN.SHAREDKERNEL.Interfaces;
using FINN.SHAREDKERNEL.Models;
using Microsoft.Extensions.Configuration;
using netDxf;
using netDxf.Blocks;
using netDxf.Entities;

namespace FINN.PLUGINS.DXF;

public class NetReadWriteDxf : IReadWriteDxf
{
    private readonly IRepository<BlockDefinition> _repository;
    private readonly IConfiguration _configuration;
    private string _blockFolder;

    public NetReadWriteDxf(IRepository<BlockDefinition> repository, IConfiguration configuration)
    {
        _repository = repository;
        _blockFolder = configuration["storage"];
    }

    private const double Gutter = 1600;

    public EstimateCostRequestDto EstimateFromFile(string filename)
    {
        var doc = DxfDocument.Load(filename);
        var typeReg = XDataUtil.GetRegistryByName("FINN.TYPE");
        var levelReg = XDataUtil.GetRegistryByName("FINN.LEVEL");

        // find out grids from blocks
        var grids = doc.Inserts.Where(x =>
                x.XData.ContainsAppId(typeReg.Name) &&
                ReferenceEquals(x.XData[typeReg.Name].XDataRecord[0].Value, "GRID"))
            .Select(x => new { Level = (double)x.XData[levelReg.Name].XDataRecord[0].Value, Box = x.GetBoundingBox() });

        var rects = doc.Polylines.Where(x => x.Vertexes.Count == 4 && x.IsClosed && x.Owner.Equals(Block.ModelSpace))
            .ToList();
        var potentialPlatformBlocks = rects.Where(x => x.Layer.Name == LayerNames.Platform).ToList();
        var potentialBooth = rects.Where(x => LayerNames.Booths.Contains(x.Layer.Name)).ToList();

        // only treat those lies on grids as valid ones
        var platforms = potentialPlatformBlocks.Select(x =>
        {
            var box = x.GetBoundingBox();
            var targetGrid = grids.SingleOrDefault(i => i.Box.IsBoxInside(box));
            if (targetGrid == null) return new GeometryDto() { ZPosition = 0 };

            var minX = x.Vertexes.MinBy(vertex => vertex.Position.X)!.Position.X;
            var minY = x.Vertexes.MinBy(vertex => vertex.Position.Y)!.Position.Y;
            var maxX = x.Vertexes.MaxBy(vertex => vertex.Position.X)!.Position.X;
            var maxY = x.Vertexes.MaxBy(vertex => vertex.Position.Y)!.Position.Y;

            return new GeometryDto()
            {
                XLength = maxX - minX,
                YLength = maxY - minY,
                ZPosition = targetGrid?.Level ?? 0
            };
        }).Where(x => x.ZPosition != 0);

        var booths = potentialBooth.Where(x =>
        {
            var box = x.GetBoundingBox();
            return grids.Any(y => y.Box.IsBoxInside(box));
        }).Select(x =>
        {
            var minX = x.Vertexes.MinBy(vertex => vertex.Position.X)!.Position.X;
            var minY = x.Vertexes.MinBy(vertex => vertex.Position.Y)!.Position.Y;
            var maxX = x.Vertexes.MaxBy(vertex => vertex.Position.X)!.Position.X;
            var maxY = x.Vertexes.MaxBy(vertex => vertex.Position.Y)!.Position.Y;

            return new GeometryDto
            {
                XLength = maxX - minX,
                YLength = maxY - minY
            };
        });

        return new EstimateCostRequestDto
        {
            Booths = booths,
            PlatformBlocks = platforms
        };
    }


    public string DrawLayout(DrawLayoutRequestDto drawLayoutRequestDto)
    {
        const int gutter = 3200;

        var dxf = DocUtil.CreateDoc();

        var canvas = new Group(Vector2d.Zero, GroupDirection.TopToBottom, GroupAlignment.Start, gutter * 4);

        // draw grids
        var grids = ToGridGroup(drawLayoutRequestDto.Grids, Vector2d.Zero);
        canvas.Add(grids);

        var layouts = new Group(Vector2d.Zero, GroupDirection.TopToBottom, GroupAlignment.Start, gutter);
        drawLayoutRequestDto.Process.ToList().ForEach(dto =>
        {
            var layoutItem = new Group(Vector2d.Zero, GroupDirection.LeftToRight, GroupAlignment.Middle, gutter);
            layoutItem.Add(Booth.FromDto(dto));

            // divide into booth and blocks
            var blocks = dto.SubProcess.Where(x =>
                x.XLength == 0 && x.YLength == 0 &&
                _repository.SingleOrDefaultAsync(bd => bd.Name == x.Name).Result != null).ToList();
            var booths = dto.SubProcess.Except(blocks).ToList();

            // handle booths first
            if (booths.Count > 0)
            {
                var boothGroup = ToBoothGroup(dto.SubProcess, Vector2d.Zero);
                layoutItem.Add(boothGroup);
            }

            // handle blocks
            if (blocks.Count > 0)
            {
                var blockGroup = new Group(Vector2d.Zero, GroupDirection.LeftToRight, GroupAlignment.Middle,
                    gutter);
                blocks.ForEach(x =>
                {
                    var bd = _repository.SingleOrDefaultAsync(bd => bd.Name == x.Name).Result!;
                    var doc = DxfDocument.Load((string)bd.DxfFileName);
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
            var platformBlocks = drawLayoutRequestDto.Platforms.Where(x => Math.Abs(x.Level - grid.Level) < double.Epsilon);
            foreach (var platformBlock in platformBlocks)
            {
                var block = new PlatformBlock(new Vector2d(platformBlock.Placement.X, platformBlock.Placement.Y),
                    platformBlock.XLength, platformBlock.YLength,
                    platformBlock.Level);
                block.BasePoint += grid.Origin;
                dxf.Add(block);
            }
        }

        dxf.Add(canvas);

        var output = Path.GetTempFileName().Replace(".tmp", ".dxf");
        dxf.Save(output);
        return output;
    }

    public IEnumerable<BlockDefinition> InsertBlockDefinitions(InsertBlockRequestDto? dto)
    {
        var doc = DxfDocument.Load(dto.Filename);


        var names = dto.Names.Any()
            ? dto.Names.ToList()
            : doc.Blocks.Items.Where(x =>
                    !x.Name.StartsWith("*") && !x.Name.StartsWith("_"))
                .Select(x => x.Name).ToList();

        foreach (var name in names.Where(name =>
                     _repository.SingleOrDefaultAsync(x => x.Name == name).Result != null))
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

            var fileName = Path.Join(_blockFolder, Path.GetFileName(Path.GetTempFileName()).Replace(".tmp", ".dxf"));
            file.Save(fileName);

            return new BlockDefinition { Name = name, DxfFileName = fileName };
        }).ToList();
        _repository.AddRangeAsync(blocks).Wait();
        _repository.SaveChangesAsync().Wait();
        return blocks;
    }

    private static Group<Booth> ToBoothGroup(IEnumerable<ProcessDto> dtos, Vector2d location)
    {
        var boothGroup =
            new Group<Booth>(location, GroupDirection.LeftToRight, GroupAlignment.Middle, 0);
        dtos.ToList().ForEach(x => boothGroup.Add(Booth.FromDto(x)));
        return boothGroup;
    }

    private static Group<Grid> ToGridGroup(IEnumerable<GridDto> dtos, Vector2d location)
    {
        var gridGroup = new Group<Grid>(location, GroupDirection.BottomToTop, GroupAlignment.Start, Gutter * 8);
        dtos.ToList().ForEach(x => gridGroup.Add(Grid.FromDto(x)));
        return gridGroup;
    }
}