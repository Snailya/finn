using System.Text.Json.Serialization;

namespace FINN.SHAREDKERNEL.Dtos.Reader;

public class ReadRequestDto : BaseDto
{
    public ReadRequestDto(int jobId, string filename)
    {
        JobId = jobId;
        Filename = filename;
    }

    /// <summary>
    ///     The id of the job, used to update job status.
    /// </summary>
    [JsonPropertyName("id")]
    public int JobId { get; }

    /// <summary>
    ///     The input file path indicates where to load file.
    /// </summary>
    [JsonPropertyName("filename")]
    public string Filename { get; }
}