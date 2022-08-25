using FINN.SHAREDKERNEL.Dtos.Draw;

namespace FINN.SHAREDKERNEL.Dtos.Drafter;

public class PlatformBlockDto
{
    public PositionDto Placement { get; set; }

    /// <summary>
    ///     The length in x direction from the placement.
    /// </summary>
    public double XLength { get; set; }

    /// <summary>
    ///     The length in y direction from the placement.
    /// </summary>
    public double YLength { get; set; }

    public double Level { get; set; }
}