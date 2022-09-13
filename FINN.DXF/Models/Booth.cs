using FINN.DXF.Geometries;
using FINN.PLUGINS.DXF;
using FINN.PLUGINS.DXF.Utils;
using FINN.SHAREDKERNEL.Dtos;
using netDxf.Entities;
using netDxf.Tables;

namespace FINN.DXF.Models;

public class Booth : DxfWrapper
{
    private readonly bool _colored = true;
    private readonly string _name;
    private readonly double _xLength;
    private readonly double _yLength;

    private bool IsConceptual => _xLength == 0 || _yLength == 0;

    private void PrepareCentralLine()
    {
        var centralLine = EntityUtil.CreateLine(LayerUtil.GetCentralLine(), new Vector2d(Box.Min.X - 800, BasePoint.Y),
            new Vector2d(Box.Max.X + 800, BasePoint.Y));
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
            var label = TextUtil.CreateMText(texts[i], BasePoint + new Vector2d(_xLength / 2,
                (Enumerable.Range(1, texts.Length).Average() - (i + 1)) * interval), 200);
            AddEntity(label, false);
        }
    }

    private void PrepareWallOrPlaceHolder()
    {
        if (IsConceptual)
        {
            var placeHolder = TextUtil.CreateMText(_name, BasePoint, MTextAttachmentPoint.MiddleLeft, 200);
            AddEntity(placeHolder);
            return;
        }

        var wall = EntityUtil.CreateLwPolyline(Layer, true, new Vector2d(0, -_yLength / 2),
            new Vector2d(_xLength, -_yLength / 2), new Vector2d(_xLength, _yLength / 2),
            new Vector2d(0, _yLength / 2));
        wall.TransformBy(new TransformScale(1), BasePoint);

        AddWall(wall);
    }

    private void AddWall(LwPolyline wall)
    {
        if (_colored)
        {
            // create hatch
            var boundary = new HatchBoundaryPath(new List<EntityObject> { wall });
            var hatch = ColorUtil.GetHatchMatchesLayer(Layer, new[] { boundary });
            AddEntity(hatch);
        }

        else
        {
            AddEntity(wall);
        }
    }

    public static Booth FromDto(ProcessDto dto, Vector2d location, bool colored)
    {
        return dto.XLength == 0 || dto.YLength == 0
            ? new Booth(LayerUtil.GetLayerByName(dto.Layer), location, dto.Name)
            : new Booth(LayerUtil.GetLayerByName(dto.Layer), location, dto.XLength, dto.YLength, colored,
                dto.Line1, dto.Line2);
    }

    public static Booth FromDto(ProcessDto dto, bool colored)
    {
        return FromDto(dto, Vector2d.Zero, colored);
    }

    #region Constructors

    public Booth(Layer layer, Vector2d basePoint, double xLength, double yLength, bool colored,
        params string[] texts) : base(layer,
        basePoint)
    {
        if (xLength < 0)
            throw new ArgumentException("value can't be less than 0", nameof(xLength));
        if (yLength < 0)
            throw new ArgumentException("value can't be less than 0", nameof(yLength));

        _xLength = xLength;
        _yLength = yLength;
        _colored = colored;

        PrepareWallOrPlaceHolder();
        PrepareLabel(texts);
        PrepareCentralLine();
    }

    public Booth(Layer layer, double xLength, double yLength, bool colored, params string[] texts) : this(
        layer, Vector2d.Zero, xLength, yLength, colored, texts)
    {
    }

    public Booth(Layer layer, Vector2d basePoint, string name) : base(layer, basePoint)
    {
        _name = name;
        PrepareWallOrPlaceHolder();
    }

    public Booth(Layer layer, string name) : this(layer, Vector2d.Zero, name)
    {
    }

    #endregion
}