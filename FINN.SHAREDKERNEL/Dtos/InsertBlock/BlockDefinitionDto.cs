using System.Text.Json.Serialization;

namespace FINN.SHAREDKERNEL.Dtos.InsertBlock;

public class BlockDefinitionDto : BaseDto
{
    /// <summary>
    /// The id of the block in database.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// The name of the block, used to insert block.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}