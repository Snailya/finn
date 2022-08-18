using System.Text.Json.Serialization;

namespace FINN.SHAREDKERNEL.Dtos.InsertBlock;

public class InsertBlockResponseDto : BaseDto
{
    /// <summary>
    /// The blocks that has been persisted in database.
    /// </summary>
    [JsonPropertyName("blocks")]
    public IEnumerable<BlockDefinitionDto> Blocks { get; set; } = new List<BlockDefinitionDto>();
}