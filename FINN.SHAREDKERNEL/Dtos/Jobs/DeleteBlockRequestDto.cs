using System.Text.Json.Serialization;
using FINN.SHAREDKERNEL.UseCases;

namespace FINN.SHAREDKERNEL.Dtos.Jobs;

public class DeleteBlockRequestDto : JsonObject
{
    /// <summary>
    ///     The id of the block definition to delete.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }
}