namespace FINN.SHAREDKERNEL.Models;

public class Scale
{
    public static Scale Identity = new(1);

    public Scale(double factor)
    {
        Factor = factor;
    }

    public Scale(Vector2d basePoint, double factor)
    {
        BasePoint = basePoint;
        Factor = factor;
    }

    public double Factor { get; set; }
    public Vector2d BasePoint { get; set; } = new(0, 0);
}