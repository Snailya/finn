using System.Text.Json.Serialization;

namespace FINN.SHAREDKERNEL.Dtos.Cost;

public class GeometryDto
{
    /// <summary>
    ///     The distance from origin to place the block in x.
    /// </summary>
    [JsonPropertyName("xPosition")]
    public double XPosition { get; set; }

    /// <summary>
    ///     The distance from origin to place the block in y.
    /// </summary>
    [JsonPropertyName("yPosition")]
    public double YPosition { get; set; }

    /// <summary>
    ///     The distance from origin to place the block in z.
    /// </summary>
    [JsonPropertyName("zPosition")]
    public double ZPosition { get; set; }

    /// <summary>
    ///     The length in x direction from the placement.
    /// </summary>
    [JsonPropertyName("xLength")]
    public double XLength { get; set; }

    /// <summary>
    ///     The length in y direction from the placement.
    /// </summary>
    [JsonPropertyName("yLength")]
    public double YLength { get; set; }

    /// <summary>
    ///     The length in z direction from the placement.
    /// </summary>
    [JsonPropertyName("yLength")]
    public double ZLength { get; set; }
}