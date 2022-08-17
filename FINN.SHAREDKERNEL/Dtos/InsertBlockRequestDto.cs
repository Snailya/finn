using System.Text.Json.Serialization;

namespace FINN.SHAREDKERNEL.Dtos;

public class InsertBlockRequestDto : BaseDto
{
    [JsonPropertyName("names")] public IEnumerable<string> Names { get; set; } = new List<string>();
    [JsonPropertyName("filename")] public string Filename { get; set; } = string.Empty;
}