using System.Text.Json.Serialization;

namespace FINN.SHAREDKERNEL.Dtos;

public class InsertBlockResponseDto : BaseDto
{
    [JsonPropertyName("blocks")]
    public IEnumerable<BlockDefinitionDto> Blocks { get; set; } = new List<BlockDefinitionDto>();
}