using FINN.SHAREDKERNEL.Dtos;
using FINN.SHAREDKERNEL.Dtos.Draw;

namespace FINN.READER.Models;

public class PlateBlock
{
    /// <summary>
    /// Distance to origin point of gird in x direction.
    /// </summary>
    public Length XDistance { get; set; } = Length.Zero;

    /// <summary>
    /// Distance to origin point of gird in y direction.
    /// </summary>
    public Length YDistance { get; set; } = Length.Zero;

    /// <summary>
    /// The length of the block in x direction.
    /// </summary>
    public Length XLength { get; set; } = Length.Zero;

    /// <summary>
    /// The length of the block in y direction.
    /// </summary>
    public Length YLength { get; set; } = Length.Zero;

    public PlateBlockDto ToDto()
    {
        return new PlateBlockDto()
        {
            Placement = new PositionDto()
            {
                X = XDistance.ToMillimeter(),
                Y = YDistance.ToMillimeter(),
            },
            XLength = XLength.ToMillimeter(),
            YLength = YLength.ToMillimeter()
        };
    }
}