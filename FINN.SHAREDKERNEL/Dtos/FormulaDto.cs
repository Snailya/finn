using System.Text.Json.Serialization;
using FINN.CORE.Models;

namespace FINN.SHAREDKERNEL.Dtos;

public class FormulaDto: JsonObject
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;
    [JsonPropertyName("expression")] public string Expression { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"Id: {Id}, Type: {Type}, Expression: {Expression}";
    }
}