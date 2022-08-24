namespace FINN.SHAREDKERNEL.Models;

public class BoundingBox
{
    public BoundingBox()
    {
    }

    public BoundingBox(Vector2d min, Vector2d max)
    {
        Min = min;
        Max = max;
    }

    public Vector2d Min { get; private set; } = new(0, 0);
    public Vector2d Max { get; private set; } = new(0, 0);

    public Vector2d TopLeft => new(Min.X, Max.Y);
    public Vector2d TopMiddle => new((Min.X + Max.X) / 2, Max.Y);
    public Vector2d TopRight => Max;
    public Vector2d MiddleLeft => new(Min.X, (Min.Y + Max.Y) / 2);
    public Vector2d Center => new((Min.X + Max.X) / 2, (Min.Y + Max.Y) / 2);
    public Vector2d MiddleRight => new(Max.X, (Min.Y + Max.Y) / 2);
    public Vector2d BottomLeft => Min;
    public Vector2d BottomMiddle => new((Min.X + Max.X) / 2, Min.Y);
    public Vector2d BottomRight => new(Max.X, Min.Y);

    public void AddPoint(Vector2d point)
    {
        var minX = new[] { Min.X, point.X }.Min();
        var minY = new[] { Min.Y, point.Y }.Min();

        var maxX = new[] { Max.X, point.X }.Max();
        var maxY = new[] { Max.Y, point.Y }.Max();

        Min = new Vector2d(minX, minY);
        Max = new Vector2d(maxX, maxY);
    }

    /// <summary>
    /// </summary>
    /// <param name="scale">变换系数</param>
    /// <param name="translation">平移</param>
    public void TransformBy(Scale scale, Vector2d translation)
    {
        Min.TransformBy(scale, translation);
        Max.TransformBy(scale, translation);
    }

    public void AddBox(BoundingBox box)
    {
        AddPoint(box.Min);
        AddPoint(box.Max);
    }

    public bool IsBoxInside(BoundingBox box)
    {
        return box.Min.X >= Min.X && box.Max.X <= Max.X && box.Min.Y >= Min.Y && box.Max.Y <= Max.Y;
    }
}