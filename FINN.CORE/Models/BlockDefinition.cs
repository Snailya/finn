using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FINN.CORE.Models;

public class BlockDefinition
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// The name of the block.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The full path of the dxf file that stores the block.
    /// </summary>
    public string DxfFileName { get; set; } = string.Empty;
}