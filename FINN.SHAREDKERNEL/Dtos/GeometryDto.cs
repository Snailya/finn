using System.Text.Json.Serialization;
using FINN.CORE.Models;

namespace FINN.SHAREDKERNEL.Dtos;

public class GeometryDto : JsonObject
{
    [JsonPropertyName("type")] public string Type { get; set; }

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
    [JsonPropertyName("zLength")]
    public double ZLength { get; set; }
}