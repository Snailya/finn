using System.Text.Json.Serialization;

namespace FINN.SHAREDKERNEL.Dtos;

public class BlockDefinitionDto : BaseDto
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }
}