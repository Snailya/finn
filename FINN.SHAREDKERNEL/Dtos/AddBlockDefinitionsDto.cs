using System.Text.Json.Serialization;
using FINN.CORE.Models;

namespace FINN.SHAREDKERNEL.Dtos;

public class AddBlockDefinitionsDto : JsonObject
{
    [JsonPropertyName("filename")] public string? Filename { get; set; }
    [JsonPropertyName("blockNames")] public IEnumerable<string>? BlockNames { get; set; }
}