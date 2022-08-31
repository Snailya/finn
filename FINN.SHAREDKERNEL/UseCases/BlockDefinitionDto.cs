using System.Text.Json.Serialization;
using FINN.CORE.Models;

namespace FINN.SHAREDKERNEL.UseCases;

public class BlockDefinitionDto : JsonObject
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("filename")] public string? Filename { get; set; }
}