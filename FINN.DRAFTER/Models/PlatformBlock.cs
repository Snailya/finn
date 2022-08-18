using FINN.DRAFTER.Extensions;
using FINN.DRAFTER.Utils;
using FINN.SHAREDKERNEL.Models;
using netDxf;
using netDxf.Entities;
using netDxf.Tables;

namespace FINN.DRAFTER.Models;

public class PlatformBlock : DxfWrapper
{
    #region Constructor

    private PlatformBlock(Layer layer, Vector2d basePoint, double xLength, double yLength, double level) : base(layer, basePoint)
    {
        var frame = EntityUtil.CreatePolyline(Layer, true, new Vector2d(basePoint.X, basePoint.Y),
            new Vector2d(basePoint.X + xLength, basePoint.Y), new Vector2d(basePoint.X + xLength, basePoint.Y + yLength),
            new Vector2d(basePoint.X, basePoint.Y + yLength));

        // create hatch
        var boundary = new HatchBoundaryPath(new List<EntityObject> { (frame.Clone() as EntityObject)! });
        var hatch = ColorUtil.GetHatchMatchesLayer(Layer);
        hatch.BoundaryPaths.Add(boundary);
        AddEntity(hatch);

        // make sure the frame is above the hatch
        AddEntity(frame);

        frame.XData.Add(new XData(new ApplicationRegistry("finn"))
            { XDataRecord = { new XDataRecord(XDataCode.String, "plate") } });
    }

    public PlatformBlock(Vector2d location, double xLength, double yLength, double level) : this(LayerUtil.GetPlate(), location,
        xLength,
        yLength, level)
    {
    }

    #endregion
}