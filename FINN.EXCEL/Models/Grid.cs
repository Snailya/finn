using FINN.SHAREDKERNEL.Dtos;

namespace FINN.EXCEL.Models;

internal class Grid
{
    /// <summary>
    ///     The length of column in x direction.
    /// </summary>
    public Length ColumnLength { get; set; } = Length.Zero;

    /// <summary>
    ///     The length of column in y direction.
    /// </summary>
    public Length ColumnWidth { get; set; } = Length.Zero;

    /// <summary>
    ///     The coordinates of grid line in x direction.
    /// </summary>
    public List<Length> XCoordinates { get; set; } = new();

    /// <summary>
    ///     The coordinates of grid line in y direction.
    /// </summary>
    public List<Length> YCoordinates { get; set; } = new();

    /// <summary>
    ///     The label of the current grid.
    /// </summary>
    public Length Level { get; set; } = Length.Zero;

    public GridDto ToDto()
    {
        return new GridDto
        {
            Level = Level.ToMillimeter(),
            ColumnXLength = ColumnLength.ToMillimeter(),
            ColumnYLength = ColumnWidth.ToMillimeter(),
            XCoordinates = XCoordinates.Select(x => x.ToMillimeter()).ToArray(),
            YCoordinates = YCoordinates.Select(x => x.ToMillimeter()).ToArray()
        };
    }
}