using FINN.SHAREDKERNEL.Dtos.Draw;

namespace FINN.READER.Models;

public class Plate
{
    /// <summary>
    /// The height of the plate.
    /// </summary>
    public Length Level { get; set; }

    /// <summary>
    /// The blocks that compose of the plate.
    /// </summary>
    public IEnumerable<PlateBlock> Blocks { get; set; } = new List<PlateBlock>();

    public PlateDto ToDto()
    {
        return new PlateDto()
        {
            Level = Level.ToMillimeter(),
            Blocks = Blocks.Select(x => x.ToDto())
        };
    }
}