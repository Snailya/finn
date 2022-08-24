using FINN.PLUGINS.DXF.Models;
using FINN.PLUGINS.DXF.Utils;
using FINN.SHAREDKERNEL.Models;
using netDxf;
using netDxf.Blocks;
using netDxf.Entities;

namespace FINN.PLUGINS.DXF;

public static class Extension
{
    /// <summary>
    ///     Convert a <see cref="Vector2d" /> object to <see cref="Vector2" /> object, which is the model of the application.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public static Vector2 ToVector2(this Vector2d point)
    {
        return new Vector2(point.X, point.Y);
    }

    /// <summary>
    ///     Convert a <see cref="Vector3" /> object to <see cref="Vector2" /> object, which means to a lower dimension xy.
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Vector2 ToVector2(this Vector3 vector)
    {
        return new Vector2(vector.X, vector.Y);
    }

    /// <summary>
    ///     Convert a <see cref="Vector2d" /> object to <see cref="Vector3" /> object, which is the model of the net.dxf.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public static Vector3 ToVector3(this Vector2d point)
    {
        return new Vector3(point.X, point.Y, 0);
    }

    /// <summary>
    ///     Convert a <see cref="Vector3" /> object to <see cref="Vector2d" /> object, which is the model of the application.
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Vector2d ToVector2d(this Vector3 vector)
    {
        return new Vector2d(vector.X, vector.Y);
    }

    /// <summary>
    ///     Convert a <see cref="Vector2" /> object to <see cref="Vector2d" /> object, which is the model of the application.
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Vector2d ToVector2d(this Vector2 vector)
    {
        return new Vector2d(vector.X, vector.Y);
    }

    /// <summary>
    ///     Transform a entity in net.dxf using the scale model defined in the application
    /// </summary>
    /// <param name="entity">a net.dxf entity</param>
    /// <param name="scale"></param>
    /// <param name="translate"></param>
    public static void TransformBy(this EntityObject entity, Scale scale, Vector2d translate)
    {
        switch (entity)
        {
            case Hatch hatch:
                hatch.BoundaryPaths.SelectMany(x => x.Entities).ToList().ForEach(x =>
                    x.TransformBy(Matrix3.Scale(scale.Factor.X, scale.Factor.Y, 1),
                        (scale.BasePoint * (new Vector2d(1, 1) - scale.Factor) + translate).ToVector3()));
                break;
            default:
                entity.TransformBy(Matrix3.Scale(scale.Factor.X, scale.Factor.Y, 1),
                    (scale.BasePoint * (new Vector2d(1, 1) - scale.Factor) + translate).ToVector3());
                break;
        }
    }

    /// <summary>
    ///     Get the bounding box of a <see cref="MText" /> object.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static BoundingBox GetBoundingBox(this MText text)
    {
        return text.AttachmentPoint switch
        {
            MTextAttachmentPoint.TopLeft => new BoundingBox(
                new Vector2d(text.Position.X, text.Position.Y - text.Height),
                new Vector2d(text.Position.X + text.RectangleWidth, text.Position.Y)),
            MTextAttachmentPoint.TopCenter => new BoundingBox(
                new Vector2d(text.Position.X - text.RectangleWidth / 2, text.Position.Y - text.Height),
                new Vector2d(text.Position.X + text.RectangleWidth / 2, text.Position.Y)),
            MTextAttachmentPoint.TopRight => new BoundingBox(
                new Vector2d(text.Position.X - text.RectangleWidth, text.Position.Y - text.Height),
                text.Position.ToVector2d()),
            MTextAttachmentPoint.MiddleLeft => new BoundingBox(
                new Vector2d(text.Position.X, text.Position.Y - text.Height / 2),
                new Vector2d(text.Position.X + text.RectangleWidth, text.Position.Y + text.Height / 2)),
            MTextAttachmentPoint.MiddleCenter => new BoundingBox(
                new Vector2d(text.Position.X - text.RectangleWidth / 2, text.Position.Y - text.Height / 2),
                new Vector2d(text.Position.X + text.RectangleWidth / 2, text.Position.Y + text.Height / 2)),
            MTextAttachmentPoint.MiddleRight => new BoundingBox(
                new Vector2d(text.Position.X - text.RectangleWidth, text.Position.Y - text.Height / 2),
                new Vector2d(text.Position.X, text.Position.Y + text.Height / 2)),
            MTextAttachmentPoint.BottomLeft => new BoundingBox(text.Position.ToVector2d(),
                new Vector2d(text.Position.X + text.RectangleWidth, text.Position.Y + text.Height)),
            MTextAttachmentPoint.BottomCenter => new BoundingBox(
                new Vector2d(text.Position.X - text.RectangleWidth / 2, text.Position.Y),
                new Vector2d(text.Position.X + text.RectangleWidth / 2, text.Position.Y + text.Height)),
            MTextAttachmentPoint.BottomRight => new BoundingBox(
                new Vector2d(text.Position.X - text.RectangleWidth, text.Position.Y),
                new Vector2d(text.Position.X, text.Position.Y + text.Height)),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    ///     Get the bounding box of a <see cref="Text" /> object.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static BoundingBox GetBoundingBox(this Text text)
    {
        switch (text.Alignment)
        {
            case TextAlignment.TopLeft:
                return new BoundingBox(
                    new Vector2d(text.Position.X, text.Position.Y - text.Height),
                    new Vector2d(text.Position.X + text.Width, text.Position.Y));
            case TextAlignment.TopCenter:
                return new BoundingBox(
                    new Vector2d(text.Position.X - text.Width / 2, text.Position.Y - text.Height),
                    new Vector2d(text.Position.X + text.Width / 2, text.Position.Y));
            case TextAlignment.TopRight:
                return new BoundingBox(
                    new Vector2d(text.Position.X - text.Width, text.Position.Y - text.Height),
                    text.Position.ToVector2d());
            case TextAlignment.MiddleLeft:
                return new BoundingBox(
                    new Vector2d(text.Position.X, text.Position.Y - text.Height / 2),
                    new Vector2d(text.Position.X + text.Width, text.Position.Y + text.Height / 2));
            case TextAlignment.Middle:
            case TextAlignment.MiddleCenter:
                return new BoundingBox(
                    new Vector2d(text.Position.X - text.Width / 2, text.Position.Y - text.Height / 2),
                    new Vector2d(text.Position.X + text.Width / 2, text.Position.Y + text.Height / 2));
            case TextAlignment.MiddleRight:
                return new BoundingBox(
                    new Vector2d(text.Position.X - text.Width, text.Position.Y - text.Height / 2),
                    new Vector2d(text.Position.X, text.Position.Y + text.Height / 2));
            case TextAlignment.BottomLeft:
            case TextAlignment.BaselineLeft:
                return new BoundingBox(text.Position.ToVector2d(),
                    new Vector2d(text.Position.X + text.Width, text.Position.Y + text.Height));
            case TextAlignment.BottomCenter:
            case TextAlignment.BaselineCenter:
                return new BoundingBox(
                    new Vector2d(text.Position.X - text.Width / 2, text.Position.Y),
                    new Vector2d(text.Position.X + text.Width / 2, text.Position.Y + text.Height));
            case TextAlignment.BottomRight:
            case TextAlignment.BaselineRight:
                return new BoundingBox(
                    new Vector2d(text.Position.X - text.Width, text.Position.Y),
                    new Vector2d(text.Position.X, text.Position.Y + text.Height));
            case TextAlignment.Aligned:
            case TextAlignment.Fit:
            default:
                throw new ArgumentOutOfRangeException(nameof(text.Alignment));
        }
    }

    /// <summary>
    ///     Add the bounding box of <see cref="EntityObject" /> in net.dxf to <see cref="BoundingBox" />.
    /// </summary>
    /// <param name="box"></param>
    /// <param name="entity"></param>
    public static void AddEntity(this BoundingBox box, EntityObject entity)
    {
        switch (entity)
        {
            case Point point:
                box.AddPoint(point.Position.ToVector2d());
                break;
            case Line line:
                box.AddPoint(line.StartPoint.ToVector2d());
                box.AddPoint(line.EndPoint.ToVector2d());
                break;
            case LwPolyline lwPolyline:
                lwPolyline.Vertexes.ToList().ForEach(x => box.AddPoint(x.Position.ToVector2d()));
                break;
            case Polyline polyline:
                polyline.Vertexes.ToList().ForEach(x => box.AddPoint(x.Position.ToVector2d()));
                break;
            case Circle circle:
                box.AddPoint(new Vector2d(circle.Center.X - circle.Radius, circle.Center.Y - circle.Radius));
                box.AddPoint(new Vector2d(circle.Center.X + circle.Radius, circle.Center.Y + circle.Radius));
                break;
            case Text text:
                box.AddBox(text.GetBoundingBox());
                break;
            case MText mText:
                box.AddBox(mText.GetBoundingBox());
                break;
            case Insert insert:
                box.AddBox(insert.GetBoundingBox());
                break;
            case Hatch hatch:
                hatch.BoundaryPaths.SelectMany(x => x.Entities).ToList().ForEach(box.AddEntity);
                break;
        }
    }

    /// <summary>
    ///     Get the bounding box of the <see cref="Block" /> in net.dxf.
    /// </summary>
    /// <param name="block"></param>
    /// <returns></returns>
    public static BoundingBox GetBoundingBox(this Block block)
    {
        var box = new BoundingBox();
        foreach (var entity in block.Entities) box.AddEntity(entity);

        return box;
    }

    /// <summary>
    ///     Get the bounding box of the <see cref="Insert" /> in net.dxf.
    /// </summary>
    /// <param name="insert"></param>
    /// <returns></returns>
    public static BoundingBox GetBoundingBox(this Insert insert)
    {
        var entities = insert.Explode();
        var box = new BoundingBox();
        entities.ForEach(x => box.AddEntity(x));
        return box;
    }

    /// <summary>
    ///     Get the bounding box of the <see cref="Polyline" /> in net.dxf.
    /// </summary>
    /// <param name="polyline"></param>
    /// <returns></returns>
    public static BoundingBox GetBoundingBox(this Polyline polyline)
    {
        var box = new BoundingBox();
        polyline.Vertexes.ToList().ForEach(x => box.AddPoint(x.Position.ToVector2d()));
        return box;
    }

    /// <summary>
    ///     Add <see cref="DxfWrapper" /> object to the document.
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="wrapper"></param>
    public static void Add(this DxfDocument doc, DxfWrapper wrapper)
    {
        doc.AddEntity(wrapper.Entities);
    }

    /// <summary>
    ///     Explode a insert iteratively.
    /// </summary>
    /// <param name="insert"></param>
    /// <returns></returns>
    public static IEnumerable<EntityObject> ExplodeIteratively(this Insert insert)
    {
        var entities = insert.Explode();

        // filter entities that are not insert
        var noInsert = entities.Where(x => x is not Insert).ToList();

        // explode iteratively for Insert type
        foreach (var item in entities.Except(noInsert).OfType<Insert>().ToList())
            noInsert.AddRange(item.ExplodeIteratively());

        return noInsert;
    }

    /// <summary>
    ///     Explode a block iteratively.
    /// </summary>
    /// <param name="block"></param>
    /// <returns></returns>
    public static IEnumerable<EntityObject> ExplodeIteratively(this Block block)
    {
        var entities = block.Entities;

        // filter entities that are not insert
        var noInsert = entities.Where(x => x is not Insert).ToList();

        // explode iteratively for Insert type
        foreach (var item in entities.Except(noInsert).OfType<Insert>().ToList())
            noInsert.AddRange(item.ExplodeIteratively());

        return noInsert;
    }


    public static void AddType(this EntityObject entity, string typeName)
    {
        var xData = new XData(XDataUtil.GetRegistryByName("FINN.TYPE"));
        xData.XDataRecord.Add(new XDataRecord(XDataCode.String, typeName));
        entity.XData.Add(xData);
    }
}