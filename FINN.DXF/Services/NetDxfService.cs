using FINN.CORE.Interfaces;
using FINN.CORE.Models;
using FINN.DXF.Models;
using FINN.PLUGINS.DXF;
using FINN.PLUGINS.DXF.Models;
using FINN.PLUGINS.DXF.Utils;
using FINN.SHAREDKERNEL.Constants;
using FINN.SHAREDKERNEL.Dtos;
using FINN.SHAREDKERNEL.Interfaces;
using FINN.SHAREDKERNEL.Models;
using netDxf;
using netDxf.Blocks;
using netDxf.Entities;

namespace FINN.DXF.Services;

public class NetDxfService : IDxfService
{
    private const double Gutter = 1600;
    private readonly string _blockFolder;
    private readonly IRepository<BlockDefinition> _repository;

    public NetDxfService(IRepository<BlockDefinition> repository, IConfiguration configuration)
    {
        _repository = repository;
        _blockFolder = configuration["storage"];
    }

    public string DrawLayout(LayoutDto dto)
    {
        const int gutter = 3200;

        var dxf = DocUtil.CreateDoc();

        var canvas = new Group(Vector2d.Zero, GroupDirection.TopToBottom, GroupAlignment.Start, gutter * 4);

        // draw grids
        var grids = new Group(Vector2d.Zero, GroupDirection.TopToBottom, GroupAlignment.Start, Gutter * 8);
        foreach (var gridDto in dto.Grids)
        {
            var grid = Grid.FromDto(gridDto);
            var wrapper = new Group(Vector2d.Zero, GroupDirection.LeftToRight, GroupAlignment.Middle, Gutter)
            {
                Label = grid.Label,
                IsLabelVisible = true
            };
            wrapper.Add(grid);
            grids.Add(wrapper);
        }

        canvas.Add(grids);

        // draw layouts
        var layouts = new Group(Vector2d.Zero, GroupDirection.TopToBottom, GroupAlignment.Start, gutter);
        dto.Process?.ToList().ForEach(item =>
        {
            var layoutItem = new Group(Vector2d.Zero, GroupDirection.LeftToRight, GroupAlignment.Middle, gutter)
            {
                Label = item.Name,
                IsLabelVisible = true
            };

            // add primary
            layoutItem.Add(Booth.FromDto(item, true));

            // divide into booth and blocks
            if (item.SubProcess == null) return;
            var blocks = item.SubProcess.Where(x =>
                x.XLength == 0 && x.YLength == 0 &&
                _repository.SingleOrDefaultAsync(bd => bd.Name == x.Name).Result != null).ToList();
            var booths = item.SubProcess.Except(blocks).ToList();

            // handle booths first
            if (booths.Count > 0)
            {
                var boothGroup = ToBoothGroup(item.SubProcess, Vector2d.Zero);
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
        dxf.Add(canvas);

        // draw plates
        foreach (var grid in grids.Items.OfType<Group>().SelectMany(x => x.Items).OfType<Grid>())
        {
            var platformBlocks =
                dto.Platforms.Where(x =>
                    Math.Abs(x.Level - grid.Level) < double.Epsilon);
            foreach (var platformBlock in platformBlocks)
            {
                var block = new PlatformBlock(new Vector2d(platformBlock.Placement.X, platformBlock.Placement.Y),
                    platformBlock.XLength, platformBlock.YLength,
                    platformBlock.Level);
                block.BasePoint += grid.Origin;
                dxf.Add(block);
            }
        }

        var filename = Path.GetTempFileName().Replace(".tmp", ".dxf");
        dxf.Save(filename);
        return filename;
    }


    public IEnumerable<GeometryDto> ReadLayout(string filename)
    {
        var geos = new List<GeometryDto>();

        var doc = DxfDocument.Load(filename);
        var rects = doc.LwPolylines
            .Where(x => x.Vertexes.Count == 4 && x.IsClosed && x.Owner.Equals(Block.ModelSpace))
            .ToList();

        // find out grids by reg
        var gridReg = XDataUtil.GetRegistryByName("FINN.GRID");
        var grids = doc.Inserts.Where(x =>
            x.XData.ContainsAppId(gridReg.Name)).Select(x =>
        {
            var levelStr = x.XData[gridReg.Name].XDataRecord[0].Value as string ?? string.Empty;
            var level = double.Parse(levelStr);
            return new
            {
                Box = x.GetBoundingBox(),
                Level = level
            };
        }).ToList();

        foreach (var rect in rects)
        {
            // check if on grid
            var boundingBox = rect.GetBoundingBox();
            var onGrid = grids.SingleOrDefault(item => item.Box.Contains(boundingBox));
            if (onGrid == null) continue;

            var minX = rect.Vertexes.MinBy(vertex => vertex.Position.X)!.Position.X;
            var minY = rect.Vertexes.MinBy(vertex => vertex.Position.Y)!.Position.Y;
            var maxX = rect.Vertexes.MaxBy(vertex => vertex.Position.X)!.Position.X;
            var maxY = rect.Vertexes.MaxBy(vertex => vertex.Position.Y)!.Position.Y;

            if (rect.Layer.Name == LayerNames.Platform)
            {
                var geo = new GeometryDto
                {
                    Type = "platform",
                    XLength = maxX - minX,
                    YLength = maxY - minY,
                    ZPosition = onGrid.Level
                };
                geos.Add(geo);
                continue;
            }

            if (LayerNames.Booths.Contains(rect.Layer.Name))
            {
                var geo = new GeometryDto
                {
                    Type = "booth",
                    XLength = maxX - minX,
                    YLength = maxY - minY
                };
                geos.Add(geo);
            }
        }

        return geos;
    }

    public void DeleteBlockDefinitionById(int id)
    {
        var blockDefinition = _repository.GetByIdAsync(id).GetAwaiter().GetResult();
        if (blockDefinition == null) throw new ArgumentNullException();
        _repository.DeleteAsync(blockDefinition).Wait();
    }

    public IEnumerable<BlockDefinitionDto> ListBlockDefinitions(PaginationFilter filter)
    {
        var blockDefinitions = _repository.ListAsync(filter).GetAwaiter().GetResult();
        return blockDefinitions.Select(x => new BlockDefinitionDto
        {
            Id = x.Id, Name = x.Name, Filename = x.DxfFileName
        });
    }

    public IEnumerable<BlockDefinitionDto> AddBlockDefinitions(string filename, IEnumerable<string>? blockNames)
    {
        var doc = DxfDocument.Load(filename);
        var names = blockNames?.ToList();
        names = names != null && names.Any()
            ? names
            : doc.Blocks.Items.Where(x =>
                    !x.Name.StartsWith("*") && !x.Name.StartsWith("_"))
                .Select(x => x.Name).ToList();

        foreach (var name in names.Where(name =>
                     _repository.SingleOrDefaultAsync(x => x.Name == name).Result != null))
            throw new ArgumentException(
                "Block with the same name already exist. Please check the content and consider using update.",
                nameof(name));

        var blocks = names.Select(name => CopyAndSaveBlock(doc.Blocks[name], name)).ToList();
        _repository.AddRangeAsync(blocks).Wait();
        _repository.SaveChangesAsync().Wait();

        return blocks.Select(x => new BlockDefinitionDto
            { Id = x.Id, Filename = x.DxfFileName, Name = x.Name });
    }

    public BlockDefinitionDto? GetBlockDefinition(int id)
    {
        var blockDefinition = _repository.GetByIdAsync(id).GetAwaiter().GetResult();
        return blockDefinition == null
            ? null
            : new BlockDefinitionDto
            {
                Id = blockDefinition.Id, Name = blockDefinition.Name, Filename = blockDefinition.DxfFileName
            };
    }

    private static Group<Booth> ToBoothGroup(IEnumerable<ProcessDto> dtos, Vector2d location)
    {
        var boothGroup =
            new Group<Booth>(location, GroupDirection.LeftToRight, GroupAlignment.Middle, 0);
        dtos.ToList().ForEach(x => boothGroup.Add(Booth.FromDto(x, false)));
        return boothGroup;
    }

    private BlockDefinition CopyAndSaveBlock(Block source, string name)
    {
        // explode

        var entities = source.ExplodeIteratively();
        var exploded = new Block(name, entities.Select(x => x.Clone() as EntityObject));

        // save as individual files
        var file = new DxfDocument();
        file.Blocks.Add(exploded);

        var dxfFileName = Path.Join(_blockFolder,
            Path.GetFileName(Path.GetTempFileName()).Replace(".tmp", ".dxf"));
        file.Save(dxfFileName);

        return new BlockDefinition { Name = name, DxfFileName = dxfFileName };
    }
}