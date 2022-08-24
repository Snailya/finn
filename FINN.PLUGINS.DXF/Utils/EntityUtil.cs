using FINN.SHAREDKERNEL.Models;
using netDxf.Entities;

namespace FINN.PLUGINS.DXF.Utils;

public static class EntityUtil
{
    public static Line CreateLine(Vector2d startPoint, Vector2d endPoint)
    {
        return CreateLine(netDxf.Tables.Layer.Default, startPoint, endPoint);
    }

    public static Line CreateLine(netDxf.Tables.Layer layer, Vector2d startPoint, Vector2d endPoint)
    {
        return new Line(startPoint.ToVector2(), endPoint.ToVector2()) { Layer = layer };
    }

    public static LwPolyline CreateLwPolyline(bool closed, params Vector2d[] points)
    {
        return CreateLwPolyline(netDxf.Tables.Layer.Default, closed, points);
    }

    public static LwPolyline CreateLwPolyline(netDxf.Tables.Layer layer, bool closed, params Vector2d[] points)
    {
        var polyline = new LwPolyline(points.Select(x => x.ToVector2()), closed) { Layer = layer };
        return polyline;
    }

    public static Circle CreateCircle(Vector2d position, double radius)
    {
        return CreateCircle(netDxf.Tables.Layer.Default, position, radius);
    }

    public static Circle CreateCircle(netDxf.Tables.Layer layer, Vector2d position, double radius)
    {
        return new Circle(position.ToVector3(), radius) { Layer = layer };
    }
}