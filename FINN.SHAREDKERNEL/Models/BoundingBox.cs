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

    public Vector2d? Min { get; private set; }
    public Vector2d? Max { get; private set; }

    public Vector2d? TopLeft => Min != null && Max != null ? new Vector2d(Min.X, Max.Y) : null;
    public Vector2d? TopMiddle => Min != null && Max != null ? new Vector2d((Min.X + Max.X) / 2, Max.Y) : null;
    public Vector2d? TopRight => Max;
    public Vector2d? MiddleLeft => Min != null && Max != null ? new Vector2d(Min.X, (Min.Y + Max.Y) / 2) : null;

    public Vector2d? Center =>
        Min != null && Max != null ? new Vector2d((Min.X + Max.X) / 2, (Min.Y + Max.Y) / 2) : null;

    public Vector2d? MiddleRight => Min != null && Max != null ? new Vector2d(Max.X, (Min.Y + Max.Y) / 2) : null;
    public Vector2d? BottomLeft => Min;
    public Vector2d? BottomMiddle => Min != null && Max != null ? new Vector2d((Min.X + Max.X) / 2, Min.Y) : null;
    public Vector2d? BottomRight => Min != null && Max != null ? new Vector2d(Max.X, Min.Y) : null;

    public void AddPoint(Vector2d point)
    {
        if (Min == null || Max == null)
        {
            Min = point;
            Max = point;
            return;
        }

        var minX = new[] { Min.X, point.X }.Min();
        var minY = new[] { Min.Y, point.Y }.Min();

        var maxX = new[] { Max.X, point.X }.Max();
        var maxY = new[] { Max.Y, point.Y }.Max();

        Min = new Vector2d(minX, minY);
        Max = new Vector2d(maxX, maxY);
    }

    /// <summary>
    ///     Transform the bounding box with scale and translation.
    /// </summary>
    /// <param name="scale">变换系数</param>
    /// <param name="translation">平移</param>
    public void TransformBy(Scale scale, Vector2d translation)
    {
        Min?.TransformBy(scale, translation);
        Max?.TransformBy(scale, translation);
    }

    /// <summary>
    ///     Extends the bounding box with a new bounding box.
    /// </summary>
    /// <param name="box"></param>
    /// <exception cref="Exception"></exception>
    public void AddBox(BoundingBox box)
    {
        if (box.Min == null || box.Max == null)
            throw new ArgumentException("Adding a box that is not initialized is not allowed.");

        AddPoint(box.Min);
        AddPoint(box.Max);
    }

    /// <summary>
    ///     Check if the input box in inside the bounding box.
    /// </summary>
    /// <param name="box"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public bool Contains(BoundingBox box)
    {
        if (Min == null || Max == null || box.Min == null || box.Max == null)
            throw new ArgumentException("Compare a box that is not initialized is not allowed.");

        return box.Min.X >= Min.X && box.Max.X <= Max.X && box.Min.Y >= Min.Y && box.Max.Y <= Max.Y;
    }
}