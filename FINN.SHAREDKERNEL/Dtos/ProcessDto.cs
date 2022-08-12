using System.Text.Json.Serialization;

namespace FINN.SHAREDKERNEL.Dtos;

public class ProcessDto
{
    /// <summary>
    ///     Name of the layer to draw.
    /// </summary>
    [JsonPropertyName("layer")]
    public string Layer { get; set; }

    /// <summary>
    ///     Name of the process, used to find the correspond block in AutoCAD
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    ///     The first line to display in the shape, usually of the format name + time.
    /// </summary>
    [JsonPropertyName("line1")]
    public string Line1 { get; set; }

    /// <summary>
    ///     The second line to display in the shape, usually of the format width x length.
    /// </summary>
    [JsonPropertyName("line2")]
    public string Line2 { get; set; }

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

    [JsonPropertyName("subProcess")] public IEnumerable<ProcessDto> SubProcess { get; set; }
}