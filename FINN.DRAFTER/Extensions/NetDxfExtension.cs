using FINN.DRAFTER.Models;
using FINN.SHAREDKERNEL.Models;
using netDxf;
using netDxf.Blocks;
using netDxf.Entities;

namespace FINN.DRAFTER.Extensions;

public static class NetDxfExtension
{
    public static Vector2 ToVector2(this Vector2d point)
    {
        return new Vector2(point.X, point.Y);
    }

    public static Vector2 ToVector2(this Vector3 vector)
    {
        return new Vector2(vector.X, vector.Y);
    }

    public static Vector3 ToVector3(this Vector2d point)
    {
        return new Vector3(point.X, point.Y, 0);
    }

    public static Vector2d ToVector2d(this Vector3 vector)
    {
        return new Vector2d(vector.X, vector.Y);
    }

    public static void TransformBy(this EntityObject entity, Scale scale, Vector2d translate)
    {
        entity.TransformBy(Matrix3.Scale(scale.Factor.X, scale.Factor.Y, 1),
            (scale.BasePoint * (new Vector2d(1, 1) - scale.Factor) + translate).ToVector3());
    }

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

    private static BoundingBox GetBoundingBox(this Block block)
    {
        var box = new BoundingBox();
        foreach (var entity in block.Entities) box.AddEntity(entity);

        return box;
    }

    private static BoundingBox GetBoundingBox(this Insert insert)
    {
        var entities = insert.Explode();
        var box = new BoundingBox();
        entities.ForEach(x => box.AddEntity(x));
        return box;
    }

    public static void Add(this DxfDocument doc, DxfWrapper wrapper)
    {
        doc.AddEntity(wrapper.Entities);
    }

    public static IEnumerable<EntityObject> ExplodeIteratively(this Insert insert)
    {
        var entities = insert.Explode();

        // filter entities that are not insert
        var noInsert = entities.Where(x => x is not Insert).ToList();

        // explode iteratively for Insert type
        foreach (var item in entities.Except(noInsert).OfType<Insert>().ToList())
        {
            noInsert.AddRange(item.ExplodeIteratively());
        }

        return noInsert;
    }

    public static IEnumerable<EntityObject> ExplodeIteratively(this Block block)
    {
        var entities = block.Entities;

        // filter entities that are not insert
        var noInsert = entities.Where(x => x is not Insert).ToList();

        // explode iteratively for Insert type
        foreach (var item in entities.Except(noInsert).OfType<Insert>().ToList())
        {
            noInsert.AddRange(item.ExplodeIteratively());
        }

        return noInsert;
    }
}