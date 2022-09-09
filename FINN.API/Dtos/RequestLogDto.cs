using System.Text.Json.Serialization;

namespace FINN.API.Dtos;

public class RequestLogDto
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("status")] public string? Status { get; set; }
    [JsonPropertyName("input")] public string? Input { get; set; }
    [JsonPropertyName("output")] public string? Output { get; set; }
    [JsonPropertyName("type")] public string? Type { get; set; }
    [JsonPropertyName("created")] public string? Created { get; set; }
}