using System.Text.Json.Serialization;

namespace FINN.SHAREDKERNEL.Dtos.Draw;

public class PositionDto
{
    [JsonPropertyName("x")] public double X { get; set; }
    [JsonPropertyName("y")] public double Y { get; set; }
}