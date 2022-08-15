using FINN.DRAFTER.Model;
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

    public static BoundingBox GetBoundingBox(this MText mText)
    {
        return mText.AttachmentPoint switch
        {
            MTextAttachmentPoint.TopLeft => new BoundingBox(
                new Vector2d(mText.Position.X, mText.Position.Y - mText.Height),
                new Vector2d(mText.Position.X + mText.RectangleWidth, mText.Position.Y)),
            MTextAttachmentPoint.TopCenter => new BoundingBox(
                new Vector2d(mText.Position.X - mText.RectangleWidth / 2, mText.Position.Y - mText.Height),
                new Vector2d(mText.Position.X + mText.RectangleWidth / 2, mText.Position.Y)),
            MTextAttachmentPoint.TopRight => new BoundingBox(
                new Vector2d(mText.Position.X - mText.RectangleWidth, mText.Position.Y - mText.Height),
                mText.Position.ToVector2d()),
            MTextAttachmentPoint.MiddleLeft => new BoundingBox(
                new Vector2d(mText.Position.X, mText.Position.Y - mText.Height / 2),
                new Vector2d(mText.Position.X + mText.RectangleWidth, mText.Position.Y + mText.Height / 2)),
            MTextAttachmentPoint.MiddleCenter => new BoundingBox(
                new Vector2d(mText.Position.X - mText.RectangleWidth / 2, mText.Position.Y - mText.Height / 2),
                new Vector2d(mText.Position.X + mText.RectangleWidth / 2, mText.Position.Y + mText.Height / 2)),
            MTextAttachmentPoint.MiddleRight => new BoundingBox(
                new Vector2d(mText.Position.X - mText.RectangleWidth, mText.Position.Y - mText.Height / 2),
                new Vector2d(mText.Position.X, mText.Position.Y + mText.Height / 2)),
            MTextAttachmentPoint.BottomLeft => new BoundingBox(mText.Position.ToVector2d(),
                new Vector2d(mText.Position.X + mText.RectangleWidth, mText.Position.Y + mText.Height)),
            MTextAttachmentPoint.BottomCenter => new BoundingBox(
                new Vector2d(mText.Position.X - mText.RectangleWidth / 2, mText.Position.Y),
                new Vector2d(mText.Position.X + mText.RectangleWidth / 2, mText.Position.Y + mText.Height)),
            MTextAttachmentPoint.BottomRight => new BoundingBox(
                new Vector2d(mText.Position.X - mText.RectangleWidth, mText.Position.Y),
                new Vector2d(mText.Position.X, mText.Position.Y + mText.Height)),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static void AddEntity(this BoundingBox box, EntityObject entity)
    {
        switch (entity)
        {
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
        foreach (var entity in block.Entities)
        {
            box.AddEntity(entity);
        }

        return box;
    }

    private static BoundingBox GetBoundingBox(this Insert insert)
    {
        var box = insert.Block.GetBoundingBox();
        box.TransformBy(new Scale(insert.Block.Origin.ToVector2d(), insert.Scale.X, insert.Scale.Y),
            insert.Position.ToVector2d());
        return box;
    }

    public static void Add(this DxfDocument doc, DxfWrapper wrapper)
    {
        doc.AddEntity(wrapper.Entities);
    }
}