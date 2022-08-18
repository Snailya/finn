namespace FINN.SHAREDKERNEL.Dtos.Draw;

public class GridDto
{
    /// <summary>
    ///     The length of column in x direction.
    /// </summary>
    public double ColumnXLength { get; set; }

    /// <summary>
    ///     The length of column in y direction.
    /// </summary>
    public double ColumnYLength { get; set; }

    /// <summary>
    ///     The coordinates of grid line in x direction.
    /// </summary>
    public IEnumerable<double> XCoordinates { get; set; } = new List<double>();

    /// <summary>
    ///     The coordinates of grid line in y direction.
    /// </summary>
    public IEnumerable<double> YCoordinates { get; set; } = new List<double>();

    /// <summary>
    ///     The level of the current grid.
    /// </summary>
    public double Level { get; set; }
}