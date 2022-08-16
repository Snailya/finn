using FINN.DRAFTER.Extensions;
using FINN.DRAFTER.Utils;
using FINN.SHAREDKERNEL.Dtos;
using FINN.SHAREDKERNEL.Models;
using netDxf.Entities;
using netDxf.Tables;

namespace FINN.DRAFTER.Model;

public class Booth : DxfWrapper
{
    private readonly string _name;
    private readonly double _xLength;
    private readonly double _yLength;

    public Booth(Layer layer, Vector2d location, double xLength, double yLength, params string[] texts) : base(layer,
        location)
    {
        if (xLength < 0)
            throw new ArgumentException("value can't be less than 0", nameof(xLength));
        if (yLength < 0)
            throw new ArgumentException("value can't be less than 0", nameof(yLength));

        _xLength = xLength;
        _yLength = yLength;

        PrepareWallOrPlaceHolder();
        PrepareLabel(texts);
        PrepareCentralLine();
    }

    public Booth(Layer layer, double xLength, double yLength, params string[] texts) : this(
        layer, Vector2d.Zero, xLength, yLength, texts)
    {
    }

    public Booth(Layer layer, Vector2d location, string name) : base(layer, location)
    {
        _name = name;
        PrepareWallOrPlaceHolder();
    }

    public Booth(Layer layer, string name) : this(layer, Vector2d.Zero, name)
    {
    }

    private bool UnSized => _xLength == 0 || _yLength == 0;

    private void PrepareCentralLine()
    {
        var centralLine = EntityUtil.CreateLine(LayerUtil.GetCentralLine(), new Vector2d(Box.Min.X - 800, Location.Y),
            new Vector2d(Box.Max.X + 800, Location.Y));
        AddEntity(centralLine, false);
    }

    private void PrepareLabel(params string[] texts)
    {
        if (texts.Length == 0)
            return;

        var interval = _yLength / (texts.Length + 1);
        for (var i = 0; i < texts.Length; i++)
        {
            if (string.IsNullOrEmpty(texts[i])) continue;
            var label = TextUtil.CreateMText(texts[i], Location + new Vector2d(_xLength / 2,
                (Enumerable.Range(1, texts.Length).Average() - (i + 1)) * interval), 200);
            AddEntity(label, false);
        }
    }

    private void PrepareWallOrPlaceHolder()
    {
        if (UnSized)
        {
            var placeHolder = TextUtil.CreateMText(_name, Location, MTextAttachmentPoint.MiddleLeft, 200);
            AddEntity(placeHolder);
            return;
        }

        var wall = EntityUtil.CreatePolyline(Layer, true, new Vector2d(0, -_yLength / 2),
            new Vector2d(_xLength, -_yLength / 2), new Vector2d(_xLength, _yLength / 2),
            new Vector2d(0, _yLength / 2));
        wall.TransformBy(new Scale(1), Location);

        AddWall(wall);
    }

    private void AddWall(EntityObject wall)
    {
        // create hatch
        var boundary = new HatchBoundaryPath(new List<EntityObject> { (wall.Clone() as EntityObject)! });
        var hatch = ColorUtil.GetHatchMatchesLayer(Layer);
        hatch.BoundaryPaths.Add(boundary);
        AddEntity(hatch);

        AddEntity(wall);
    }

    public static Booth FromDto(ProcessDto dto, Vector2d location)
    {
        return dto.XLength == 0 || dto.YLength == 0
            ? new Booth(LayerUtil.GetLayerByName(dto.Layer), location, dto.Name)
            : new Booth(LayerUtil.GetLayerByName(dto.Layer), location, dto.XLength, dto.YLength,
                dto.Line1, dto.Line2);
    }

    public static Booth FromDto(ProcessDto dto)
    {
        return FromDto(dto, Vector2d.Zero);
    }
}