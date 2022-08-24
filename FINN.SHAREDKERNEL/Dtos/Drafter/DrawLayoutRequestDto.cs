using System.Text.Json.Serialization;

namespace FINN.SHAREDKERNEL.Dtos.Drafter;

public class DrawLayoutRequestDto : BaseDto
{
    /// <summary>
    ///     The id of the job, used to update job status.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    ///     The items that will be drawn as grid.
    /// </summary>
    [JsonPropertyName("grids")]
    public IEnumerable<GridDto> Grids { get; set; } = new List<GridDto>();

    /// <summary>
    ///     The items that will be drawn as plate.
    /// </summary>
    [JsonPropertyName("plates")]
    public IEnumerable<PlatformBlockDto> Platforms { get; set; } = new List<PlatformBlockDto>();

    /// <summary>
    ///     The items that will be drawn as process item.
    /// </summary>
    [JsonPropertyName("process")]
    public IEnumerable<ProcessDto> Process { get; set; } = new List<ProcessDto>();
}