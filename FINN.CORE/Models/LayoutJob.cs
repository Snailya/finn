using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FINN.CORE.Models;

public class LayoutJob
{
    public LayoutJob(string input)
    {
        Input = input;
    }


    public int Id { get; set; }



    /// <summary>
    ///     Gets the path of input file.
    /// </summary>
    public string Input { get; set; }

    /// <summary>
    ///     Gets the path of output file.
    /// </summary>
    public string Output { get; set; } = string.Empty;

    /// <summary>
    ///     The status of the job.
    /// </summary>
    public JobStatus Status { get; set; } = JobStatus.Pending;
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum JobStatus
{
    Error = 0,
    Pending = 10,
    Reading = 11,
    Drawing = 12,
    Ready = 13
}