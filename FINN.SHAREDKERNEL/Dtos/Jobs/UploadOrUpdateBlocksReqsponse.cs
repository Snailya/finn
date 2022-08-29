using System.Text.Json.Serialization;
using FINN.SHAREDKERNEL.UseCases;

namespace FINN.SHAREDKERNEL.Dtos.Jobs;

public class UploadOrUpdateBlocksReqsponse : JsonObject
{
    /// <summary>
    ///     The blocks that has been persisted in database.
    /// </summary>
    [JsonPropertyName("blocks")]
    public IEnumerable<BlockDefinitionDto> Blocks { get; set; } = new List<BlockDefinitionDto>();
}