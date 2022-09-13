namespace FINN.DXF.Geometries;

public class Vector2d
{
    public Vector2d(double x, double y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    ///     The vector represents origin.
    /// </summary>
    public static Vector2d Zero => new(0, 0);

    /// <summary>
    ///     X value from origin
    /// </summary>
    public double X { get; set; }

    /// <summary>
    ///     Y value from origin
    /// </summary>
    public double Y { get; set; }

    public static Vector2d operator /(Vector2d v, double scalar)
    {
        return new Vector2d(v.X / scalar, v.Y / scalar);
    }

    public static Vector2d operator *(Vector2d v, double scalar)
    {
        return new Vector2d(v.X * scalar, v.Y * scalar);
    }

    public static Vector2d operator *(Vector2d v1, Vector2d v2)
    {
        return new Vector2d(v1.X * v2.X, v1.Y * v2.Y);
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
    ///     Transform the point.
    /// </summary>
    /// <param name="transformScale">变换系数</param>
    /// <param name="translation">平移</param>
    public void TransformBy(TransformScale transformScale, Vector2d translation)
    {
        X = (X - transformScale.BasePoint.X) * transformScale.Factor.X + transformScale.BasePoint.X + translation.X;
        Y = (Y - transformScale.BasePoint.Y) * transformScale.Factor.Y + transformScale.BasePoint.Y + translation.Y;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Vector2d v) return false;

        return Math.Abs(v.X - X) < double.Epsilon && Math.Abs(v.Y - Y) < double.Epsilon;
    }

    public override string ToString()
    {
        return $"({X},{Y})";
    }
}