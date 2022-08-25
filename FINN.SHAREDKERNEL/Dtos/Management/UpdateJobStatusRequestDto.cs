using System.Text.Json.Serialization;
using FINN.CORE.Models;

namespace FINN.SHAREDKERNEL.Dtos.Management;

public class UpdateJobStatusRequestDto : BaseDto
{
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

    #region Constructors

    public UpdateJobStatusRequestDto()
    {
    }

    public UpdateJobStatusRequestDto(int id, JobStatus jobStatus)
    {
        Id = id;
        Status = jobStatus;
    }

    public UpdateJobStatusRequestDto(int id, string output)
    {
        Id = id;
        Output = output;
        Status = JobStatus.Ready;
    }

    #endregion
}