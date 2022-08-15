namespace FINN.SHAREDKERNEL.Models;

public class Scale
{
    public static readonly Scale Identity = new(1);

    public Scale(double factor)
    {
        Factor = new Vector2d(factor, factor);
    }

    public Scale(Vector2d basePoint, double x, double y)
    {
        BasePoint = basePoint;
        Factor = new Vector2d(x, y);
    }

    public Scale(Vector2d basePoint, double factor)
    {
        BasePoint = basePoint;
        Factor = new Vector2d(factor, factor);
    }

    public Vector2d Factor { get; set; }
    public Vector2d BasePoint { get; set; } = new(0, 0);
}