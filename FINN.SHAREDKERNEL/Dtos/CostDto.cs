using System.Text.Json.Serialization;
using FINN.CORE.Models;

namespace FINN.SHAREDKERNEL.Dtos;

public class CostDto : JsonObject
{
    [JsonPropertyName("category")] public string Category { get; set; } = string.Empty;
    [JsonPropertyName("value")] public double Value { get; set; }
}