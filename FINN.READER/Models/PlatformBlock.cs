using FINN.SHAREDKERNEL.Dtos.Drafter;
using FINN.SHAREDKERNEL.Dtos.Draw;

namespace FINN.READER.Models;

public class PlatformBlock
{
    public Length XPosition { get; set; } = Length.Zero;
    public Length YPosition { get; set; } = Length.Zero;
    public Length Length { get; set; } = Length.Zero;
    public Length Width { get; set; } = Length.Zero;
    public Length Level { get; set; } = Length.Zero;

    public PlatformBlockDto ToDto()
    {
        return new PlatformBlockDto()
        {
            Placement = new PositionDto() { X = XPosition.ToMillimeter(), Y = YPosition.ToMillimeter() },
            Level = Level.ToMillimeter(),
            XLength = Length.ToMillimeter(),
            YLength = Width.ToMillimeter(),
        };
    }
}