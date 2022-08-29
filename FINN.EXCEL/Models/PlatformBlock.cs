using FINN.SHAREDKERNEL.UseCases;

namespace FINN.EXCEL.Models;

public class PlatformBlock
{
    public Length XPosition { get; set; } = Length.Zero;
    public Length YPosition { get; set; } = Length.Zero;
    public Length Length { get; set; } = Length.Zero;
    public Length Width { get; set; } = Length.Zero;
    public Length Level { get; set; } = Length.Zero;

    public PlatformDto ToDto()
    {
        return new PlatformDto
        {
            Placement = new PositionDto { X = XPosition.ToMillimeter(), Y = YPosition.ToMillimeter() },
            Level = Level.ToMillimeter(),
            XLength = Length.ToMillimeter(),
            YLength = Width.ToMillimeter()
        };
    }
}