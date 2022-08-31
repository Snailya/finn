using System.Text.Json.Serialization;
using FINN.CORE.Models;

namespace FINN.SHAREDKERNEL.UseCases;

public class LayoutDto : JsonObject
{
    [JsonPropertyName("grids")] public IEnumerable<GridDto>? Grids { get; set; }
    [JsonPropertyName("platforms")] public IEnumerable<PlatformDto>? Platforms { get; set; }
    [JsonPropertyName("process")] public IEnumerable<ProcessDto>? Process { get; set; }
}

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

public class ProcessDto
{
    /// <summary>
    ///     Name of the layer to draw.
    /// </summary>
    [JsonPropertyName("layer")]
    public string? Layer { get; set; }

    /// <summary>
    ///     Name of the process, used to find the correspond block in AutoCAD
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    ///     The first line to display in the shape, usually of the format name + time.
    /// </summary>
    [JsonPropertyName("line1")]
    public string? Line1 { get; set; }

    /// <summary>
    ///     The second line to display in the shape, usually of the format width x length.
    /// </summary>
    [JsonPropertyName("line2")]
    public string? Line2 { get; set; }

    /// <summary>
    ///     Lenght(mm) (length in x-direction) of room.
    /// </summary>
    [JsonPropertyName("xLength")]
    public double XLength { get; set; }

    /// <summary>
    ///     Lenght(mm) (length in y-direction) of room.
    /// </summary>
    [JsonPropertyName("yLength")]
    public double YLength { get; set; }

    /// <summary>
    ///     The process items that compose of the process.
    /// </summary>
    [JsonPropertyName("subProcess")]
    public IEnumerable<ProcessDto>? SubProcess { get; set; }
}

public class PlatformDto
{
    public PositionDto Placement { get; set; }

    /// <summary>
    ///     The length in x direction from the placement.
    /// </summary>
    public double XLength { get; set; }

    /// <summary>
    ///     The length in y direction from the placement.
    /// </summary>
    public double YLength { get; set; }

    public double Level { get; set; }
}

public class PositionDto
{
    [JsonPropertyName("x")] public double X { get; set; }
    [JsonPropertyName("y")] public double Y { get; set; }
}