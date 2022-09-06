using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FINN.CORE.Models;

public abstract class BaseEntity
{
    /// <summary>
    ///     The id in database.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    ///     When the entity is created.
    /// </summary>
    public DateTime? Created { get; set; } = DateTime.UtcNow;

    /// <summary>
    ///     Last modified time of the entity.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? Modified { get; set; }
}