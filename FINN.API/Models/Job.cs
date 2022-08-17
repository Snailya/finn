using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FINN.SHAREDKERNEL.Models;

namespace FINN.API.Models;

public class Job
{
    public Job(string input)
    {
        Input = input;
    }

    /// <summary>
    ///     The id in database.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    ///     When the entity is created.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime? Created { get; set; }

    /// <summary>
    ///     Last modified time of the entity.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? Modified { get; set; }

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