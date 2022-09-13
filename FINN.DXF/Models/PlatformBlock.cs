using FINN.DXF.Geometries;
using FINN.PLUGINS.DXF.Utils;
using netDxf;
using netDxf.Entities;
using netDxf.Tables;

namespace FINN.DXF.Models;

public class PlatformBlock : DxfWrapper
{
    #region Constructor

    private PlatformBlock(Layer layer, Vector2d basePoint, double xLength, double yLength, double level) : base(layer,
        basePoint)
    {
        var frame = EntityUtil.CreateLwPolyline(Layer, true, new Vector2d(basePoint.X, basePoint.Y),
            new Vector2d(basePoint.X + xLength, basePoint.Y),
            new Vector2d(basePoint.X + xLength, basePoint.Y + yLength),
            new Vector2d(basePoint.X, basePoint.Y + yLength));

        // create hatch
        var boundary = new HatchBoundaryPath(new List<EntityObject> { frame });
        var hatch = ColorUtil.GetHatchMatchesLayer(Layer, new[] { boundary });
        AddEntity(hatch);

        frame.XData.Add(new XData(new ApplicationRegistry("finn"))
            { XDataRecord = { new XDataRecord(XDataCode.String, "plate") } });
    }

    public PlatformBlock(Vector2d location, double xLength, double yLength, double level) : this(LayerUtil.GetPlate(),
        location,
        xLength,
        yLength, level)
    {
    }

    #endregion
}