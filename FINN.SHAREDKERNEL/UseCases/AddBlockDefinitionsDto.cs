using System.Text.Json.Serialization;

namespace FINN.SHAREDKERNEL.UseCases;

public class AddBlockDefinitionsDto : JsonObject
{
    [JsonPropertyName("filename")] public string? Filename { get; set; }

    [JsonPropertyName("blockNames")] public IEnumerable<string>? BlockNames { get; set; }
}