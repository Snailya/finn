using FINN.DRAFTER.Extensions;
using FINN.SHAREDKERNEL.Models;
using netDxf.Entities;
using netDxf.Tables;

namespace FINN.DRAFTER.Utils;

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

    public static Polyline CreatePolyline(bool closed, params Vector2d[] points)
    {
        return CreatePolyline(Layer.Default, closed, points);
    }

    public static Polyline CreatePolyline(Layer layer, bool closed, params Vector2d[] points)
    {
        var polyline = new Polyline { Layer = layer };
        polyline.Vertexes.AddRange(points.Select(x => new PolylineVertex(x.ToVector3())));
        polyline.IsClosed = closed;
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