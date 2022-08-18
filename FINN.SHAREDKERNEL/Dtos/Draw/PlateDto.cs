namespace FINN.SHAREDKERNEL.Dtos.Draw;

public class PlateDto
{
    /// <summary>
    /// The height of the plate.
    /// </summary>
    public double Level { get; set; }

    /// <summary>
    /// The blocks that compose of the plate.
    /// </summary>
    public IEnumerable<PlateBlockDto> Blocks { get; set; } = new List<PlateBlockDto>();
}