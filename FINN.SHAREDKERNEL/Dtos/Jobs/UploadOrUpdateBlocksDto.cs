using System.Text.Json.Serialization;
using FINN.SHAREDKERNEL.UseCases;

namespace FINN.SHAREDKERNEL.Dtos.Jobs;

public class UploadOrUpdateBlocksDto : JsonObject
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