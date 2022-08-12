namespace FINN.SHAREDKERNEL.Dtos;

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
    public double[] XCoordinates { get; set; }

    /// <summary>
    ///     The coordinates of grid line in y direction.
    /// </summary>
    public double[] YCoordinates { get; set; }

    /// <summary>
    ///     The label of the current grid.
    /// </summary>
    public string Label { get; set; }
}