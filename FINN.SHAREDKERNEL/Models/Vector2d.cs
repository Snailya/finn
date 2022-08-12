namespace FINN.SHAREDKERNEL.Models;

public class Vector2d
{
    public static readonly Vector2d Zero = new(0, 0);

    public Vector2d(double x, double y)
    {
        X = x;
        Y = y;
    }

    public double X { get; set; }
    public double Y { get; set; }

    public static Vector2d operator /(Vector2d v, double scalar)
    {
        return new Vector2d(v.X / scalar, v.Y / scalar);
    }

    public static Vector2d operator *(Vector2d v, double scalar)
    {
        return new Vector2d(v.X * scalar, v.Y * scalar);
    }

    public static Vector2d operator +(Vector2d v1, Vector2d v2)
    {
        return new Vector2d(v1.X + v2.X, v1.Y + v2.Y);
    }

    public static Vector2d operator -(Vector2d v1, Vector2d v2)
    {
        return new Vector2d(v1.X - v2.X, v1.Y - v2.Y);
    }

    /// <summary>
    /// </summary>
    /// <param name="scale">变换系数</param>
    /// <param name="translation">平移</param>
    public void TransformBy(Scale scale, Vector2d translation)
    {
        X = (X - scale.BasePoint.X) * scale.Factor + scale.BasePoint.X + translation.X;
        Y = (Y - scale.BasePoint.Y) * scale.Factor + scale.BasePoint.Y + translation.Y;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Vector2d v) return false;

        return Math.Abs(v.X - X) < double.Epsilon && Math.Abs(v.Y - Y) < double.Epsilon;
    }
}