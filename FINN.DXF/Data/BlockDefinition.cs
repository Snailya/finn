using FINN.CORE.Models;

namespace FINN.DXF.Models;

public class BlockDefinition : BaseEntity
{
    /// <summary>
    ///     The name of the block.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     The full path of the dxf file that stores the block.
    /// </summary>
    public string DxfFileName { get; set; } = string.Empty;
}