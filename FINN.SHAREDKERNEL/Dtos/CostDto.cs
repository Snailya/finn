using System.Text.Json.Serialization;
using FINN.CORE.Models;

namespace FINN.SHAREDKERNEL.Dtos;

public class CostDto : JsonObject
{
    [JsonPropertyName("platform")] public decimal Platform { get; set; }
    [JsonPropertyName("booth")] public decimal Booth { get; set; }
}