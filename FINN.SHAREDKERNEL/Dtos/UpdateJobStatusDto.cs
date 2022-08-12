using System.Text.Json.Serialization;
using FINN.SHAREDKERNEL.Models;

namespace FINN.SHAREDKERNEL.Dtos;

public class UpdateJobStatusDto
{
    public UpdateJobStatusDto()
    {
    }

    public UpdateJobStatusDto(int id, JobStatus jobStatus)
    {
        Id = id;
        Status = jobStatus;
    }

    public UpdateJobStatusDto(int id, string output)
    {
        Id = id;
        Output = output;
        Status = JobStatus.Ready;
    }

    /// <summary>
    ///     The id of the job.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    ///     Gets the path of output file.
    /// </summary>
    [JsonPropertyName("output")]
    public string Output { get; set; } = string.Empty;

    /// <summary>
    ///     The status of the job.
    /// </summary>
    [JsonPropertyName("status")]
    public JobStatus Status { get; set; }
}