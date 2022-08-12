using FINN.DRAFTER.Model;
using FINN.SHAREDKERNEL.Models;
using netDxf;
using netDxf.Entities;

namespace FINN.DRAFTER;

public static class Extentsion
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
        entity.TransformBy(Matrix3.Scale(scale.Factor),
            (scale.BasePoint * (1 - scale.Factor) + translate).ToVector3());
    }

    public static BoundingBox GetBoundingBox(this MText mText)
    {
        switch (mText.AttachmentPoint)
        {
            case MTextAttachmentPoint.TopLeft:
                return new BoundingBox(new Vector2d(mText.Position.X, mText.Position.Y - mText.Height),
                    new Vector2d(mText.Position.X + mText.RectangleWidth, mText.Position.Y));
            case MTextAttachmentPoint.TopCenter:
                return new BoundingBox(
                    new Vector2d(mText.Position.X - mText.RectangleWidth / 2, mText.Position.Y - mText.Height),
                    new Vector2d(mText.Position.X + mText.RectangleWidth / 2, mText.Position.Y));
            case MTextAttachmentPoint.TopRight:
                return new BoundingBox(
                    new Vector2d(mText.Position.X - mText.RectangleWidth, mText.Position.Y - mText.Height),
                    mText.Position.ToVector2d());
            case MTextAttachmentPoint.MiddleLeft:
                return new BoundingBox(
                    new Vector2d(mText.Position.X, mText.Position.Y - mText.Height / 2),
                    new Vector2d(mText.Position.X + mText.RectangleWidth, mText.Position.Y + mText.Height / 2));
            case MTextAttachmentPoint.MiddleCenter:
                return new BoundingBox(
                    new Vector2d(mText.Position.X - mText.RectangleWidth / 2, mText.Position.Y - mText.Height / 2),
                    new Vector2d(mText.Position.X + mText.RectangleWidth / 2, mText.Position.Y + mText.Height / 2));
            case MTextAttachmentPoint.MiddleRight:
                return new BoundingBox(
                    new Vector2d(mText.Position.X - mText.RectangleWidth, mText.Position.Y - mText.Height / 2),
                    new Vector2d(mText.Position.X, mText.Position.Y + mText.Height / 2));
            case MTextAttachmentPoint.BottomLeft:
                return new BoundingBox(
                    mText.Position.ToVector2d(),
                    new Vector2d(mText.Position.X + mText.RectangleWidth, mText.Position.Y + mText.Height));
            case MTextAttachmentPoint.BottomCenter:
                return new BoundingBox(
                    new Vector2d(mText.Position.X - mText.RectangleWidth / 2, mText.Position.Y),
                    new Vector2d(mText.Position.X + mText.RectangleWidth / 2, mText.Position.Y + mText.Height));
            case MTextAttachmentPoint.BottomRight:
                return new BoundingBox(
                    new Vector2d(mText.Position.X - mText.RectangleWidth, mText.Position.Y),
                    new Vector2d(mText.Position.X, mText.Position.Y + mText.Height));
            default:
                throw new ArgumentOutOfRangeException();
        }
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
                //todo: currently ignore
                break;
            case Hatch hatch:
                hatch.BoundaryPaths.SelectMany(x => x.Entities).ToList().ForEach(box.AddEntity);
                break;
        }
    }

    public static void Add(this DxfDocument doc, DxfWrapper wrapper)
    {
        doc.AddEntity(wrapper.Entities);
    }
}