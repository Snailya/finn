using FINN.SHAREDKERNEL.Models;
using netDxf.Entities;
using netDxf.Tables;

namespace FINN.PLUGINS.DXF.Utils;

public static class EntityUtil
{
    public static Line CreateLine(Vector2d startPoint, Vector2d endPoint)
    {
        return CreateLine(Layer.Default, startPoint, endPoint);
    }

    public static Line CreateLine(Layer layer, Vector2d startPoint, Vector2d endPoint)
    {
        return new Line(startPoint.ToVector2(), endPoint.ToVector2()) { Layer = layer };
    }

    public static LwPolyline CreateLwPolyline(bool closed, params Vector2d[] points)
    {
        return CreateLwPolyline(Layer.Default, closed, points);
    }

    public static LwPolyline CreateLwPolyline(Layer layer, bool closed, params Vector2d[] points)
    {
        var polyline = new LwPolyline(points.Select(x => x.ToVector2()), closed) { Layer = layer };
        return polyline;
    }

    public static Circle CreateCircle(Vector2d position, double radius)
    {
        return CreateCircle(Layer.Default, position, radius);
    }

    public static Circle CreateCircle(Layer layer, Vector2d position, double radius)
    {
        return new Circle(position.ToVector3(), radius) { Layer = layer };
    }
}