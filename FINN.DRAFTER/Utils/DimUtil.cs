using FINN.SHAREDKERNEL.Models;
using netDxf.Entities;
using netDxf.Tables;

namespace FINN.DRAFTER.Utils;

public static class DimUtil
{
    private static readonly Dictionary<string, DimensionStyle> DimStyles = new();

    public static AlignedDimension CreateAlignedDim(Vector2d point1, Vector2d point2, double scaleFactor)
    {
        return new AlignedDimension(point1.ToVector2(), point2.ToVector2(), 800, GetDimStyle(scaleFactor))
            { Layer = LayerUtil.GetDim() };
    }

    public static DimensionStyle GetDimStyle(double scaleFactor)
    {
        var name = $"1-{scaleFactor}";
        if (DimStyles.TryGetValue(name, out var dimStyle)) return dimStyle;

        dimStyle = new DimensionStyle(name)
        {
            TextInsideAlign = false,
            TextHeight = 3,
            DimScaleOverall = scaleFactor,
            TextVerticalPlacement = DimensionStyleTextVerticalPlacement.Above,
            LengthPrecision = 0
        };
        DimStyles.Add(name, dimStyle);
        return dimStyle;
    }
}