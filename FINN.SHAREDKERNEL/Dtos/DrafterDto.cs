using System.Text.Json.Serialization;

namespace FINN.SHAREDKERNEL.Dtos;

public class DrafterDto
{
    /// <summary>
    ///     The id of the job, used to update job status.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// </summary>
    [JsonPropertyName("grids")]
    public IEnumerable<GridDto> Grids { get; set; }

    /// <summary>
    /// </summary>
    [JsonPropertyName("process")]
    public IEnumerable<ProcessDto> Process { get; set; }
}