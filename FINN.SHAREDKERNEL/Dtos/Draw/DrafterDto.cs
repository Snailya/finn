using System.Text.Json.Serialization;

namespace FINN.SHAREDKERNEL.Dtos.Draw;

public class DrafterDto : BaseDto
{
    /// <summary>
    ///     The id of the job, used to update job status.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// The items that will be drawn as grid.
    /// </summary>
    [JsonPropertyName("grids")]
    public IEnumerable<GridDto> Grids { get; set; } = new List<GridDto>();

    /// <summary>
    /// The items that will be drawn as plate.
    /// </summary>
    [JsonPropertyName("plates")]
    public IEnumerable<PlateDto> Plates { get; set; } = new List<PlateDto>();

    /// <summary>
    /// The items that will be drawn as process item.
    /// </summary>
    [JsonPropertyName("process")]
    public IEnumerable<ProcessDto> Process { get; set; } = new List<ProcessDto>();
}