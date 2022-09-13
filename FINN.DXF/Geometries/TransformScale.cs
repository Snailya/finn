namespace FINN.DXF.Geometries;

/// <summary>
///     The uniform scale and base point to perform transformation, used by TransformedBy().
/// </summary>
public class TransformScale
{
    public TransformScale(double factor)
    {
        Factor = new Vector2d(factor, factor);
    }

    public TransformScale(Vector2d basePoint, double x, double y)
    {
        BasePoint = basePoint;
        Factor = new Vector2d(x, y);
    }

    public TransformScale(Vector2d basePoint, double factor)
    {
        BasePoint = basePoint;
        Factor = new Vector2d(factor, factor);
    }

    /// <summary>
    ///     The identity scale.
    /// </summary>
    public static TransformScale Identity => new(1);

    /// <summary>
    ///     The uniform scale factor.
    /// </summary>
    public Vector2d Factor { get; set; }

    /// <summary>
    ///     The base point to preform transformation.
    /// </summary>
    public Vector2d BasePoint { get; set; } = new(0, 0);
}