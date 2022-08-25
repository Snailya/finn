using System.Text.Json.Serialization;

namespace FINN.SHAREDKERNEL.Dtos.Cost;

public class EstimateCostRequestDto : BaseDto
{
    /// <summary>
    ///     Geometry info of booths.
    /// </summary>
    [JsonPropertyName("booths")]
    public IEnumerable<GeometryDto> Booths { get; set; } = new List<GeometryDto>();

    /// <summary>
    ///     Geomerty info of platforms.
    /// </summary>
    [JsonPropertyName("platforms")]
    public IEnumerable<GeometryDto> PlatformBlocks { get; set; } = new List<GeometryDto>();
}