using System.Text.Json.Serialization;

namespace FINN.SHAREDKERNEL.Dtos.Read;

public class ReaderDto : BaseDto
{
    public ReaderDto(int id, string input)
    {
        Id = id;
        Input = input;
    }

    /// <summary>
    ///     The id of the job, used to update job status.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; }

    /// <summary>
    ///     The input file path indicates where to load file.
    /// </summary>
    [JsonPropertyName("input")]
    public string Input { get; }
}