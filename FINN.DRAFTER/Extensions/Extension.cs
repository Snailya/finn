using FINN.DRAFTER.Model;
using FINN.SHAREDKERNEL.Dtos;
using FINN.SHAREDKERNEL.Models;

namespace FINN.DRAFTER.Extensions;

public static class Extension
{
    private const double Gutter = 1600;

    public static Group<Grid> ToGridGroup(this IEnumerable<GridDto> dtos, Vector2d location)
    {
        var gridGroup = new Group<Grid>(location, GroupDirection.BottomToTop, GroupAlignment.Start, Gutter * 8);
        dtos.ToList().ForEach(x => gridGroup.Add(Grid.FromDto(x)));
        return gridGroup;
    }

    public static Group<Booth> ToBoothGroup(this IEnumerable<ProcessDto> dtos, Vector2d location)
    {
        var boothGroup =
            new Group<Booth>(location, GroupDirection.LeftToRight, GroupAlignment.Middle, 0);
        dtos.ToList().ForEach(x => boothGroup.Add(Booth.FromDto(x)));
        return boothGroup;
    }
}