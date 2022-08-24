using System.Text.Json.Serialization;

namespace FINN.SHAREDKERNEL.Dtos.Drafter;

public class GridDto
{
    /// <summary>
    ///     The length of column in x direction.
    /// </summary>
    [JsonPropertyName("columnXLength")]
    public double ColumnXLength { get; set; }

    /// <summary>
    ///     The length of column in y direction.
    /// </summary>
    [JsonPropertyName("columnYLength")]
    public double ColumnYLength { get; set; }

    /// <summary>
    ///     The coordinates of grid line in x direction.
    /// </summary>
    [JsonPropertyName("xCoordinates")]
    public IEnumerable<double> XCoordinates { get; set; } = new List<double>();

    /// <summary>
    ///     The coordinates of grid line in y direction.
    /// </summary>
    [JsonPropertyName("yCoordinates")]
    public IEnumerable<double> YCoordinates { get; set; } = new List<double>();

    /// <summary>
    ///     The level of the current grid.
    /// </summary>
    [JsonPropertyName("level")]
    public double Level { get; set; }
}