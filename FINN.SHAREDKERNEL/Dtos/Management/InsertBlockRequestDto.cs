using System.Text.Json.Serialization;

namespace FINN.SHAREDKERNEL.Dtos.Management;

public class InsertBlockRequestDto : BaseDto
{
    /// <summary>
    ///     The names of the blocks in file to persist in database.
    /// </summary>
    [JsonPropertyName("names")]
    public IEnumerable<string> Names { get; set; } = new List<string>();

    /// <summary>
    ///     The full file path of the dxf document that to search for blocks.
    /// </summary>
    [JsonPropertyName("filename")]
    public string Filename { get; set; } = string.Empty;
}