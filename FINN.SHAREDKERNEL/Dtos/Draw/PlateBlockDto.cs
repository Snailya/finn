namespace FINN.SHAREDKERNEL.Dtos.Draw;

public class PlateBlockDto
{
    /// <summary>
    /// The distance from origin to place the block.
    /// </summary>
    public PositionDto Placement { get; set; } = new() { X = 0, Y = 0 };

    /// <summary>
    /// The length in x direction from the placement.
    /// </summary>
    public double XLength { get; set; }

    /// <summary>
    /// The length in y direction from the placement.
    /// </summary>
    public double YLength { get; set; }
}