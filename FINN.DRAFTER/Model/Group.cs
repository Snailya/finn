using FINN.SHAREDKERNEL.Models;
using netDxf.Tables;

namespace FINN.DRAFTER.Model;

public class Group<T> : DxfWrapper where T : DxfWrapper
{
    private readonly GroupAlignment _alignment;
    private readonly double _gutter;
    private readonly GroupDirection _direction;
    private readonly List<T> _items = new();

    public Group(Vector2d location, GroupDirection direction, GroupAlignment alignment, double gutter) : base(
        Layer.Default,
        location)
    {
        _direction = direction;
        _alignment = alignment;
        _gutter = gutter;
    }

    public virtual void Add(T item)
    {
        item.Location = _alignment switch
        {
            GroupAlignment.Start => _direction switch
            {
                GroupDirection.LeftToRight => (_items.Count == 0 ? Location : _items.Last().Box.TopRight) +
                    item.Location - item.Box.TopLeft + new Vector2d(_gutter, 0),
                GroupDirection.RightToLeft => (_items.Count == 0 ? Location : _items.Last().Box.TopLeft) +
                    item.Location - item.Box.TopRight + new Vector2d(-_gutter, 0),
                GroupDirection.TopToBottom => (_items.Count == 0 ? Location : _items.Last().Box.BottomLeft) +
                    item.Location - item.Box.TopLeft + new Vector2d(0, -_gutter),
                GroupDirection.BottomToTop => (_items.Count == 0 ? Location : _items.Last().Box.TopLeft) +
                    item.Location - item.Box.BottomLeft + new Vector2d(0, _gutter),
                _ => throw new ArgumentOutOfRangeException()
            },
            GroupAlignment.Middle => _direction switch
            {
                GroupDirection.LeftToRight => (_items.Count == 0 ? Location : _items.Last().Box.MiddleRight) +
                    item.Location - item.Box.MiddleLeft + new Vector2d(_gutter, 0),
                GroupDirection.RightToLeft => (_items.Count == 0 ? Location : _items.Last().Box.MiddleLeft) +
                    item.Location - item.Box.MiddleRight + new Vector2d(-_gutter, 0),
                GroupDirection.TopToBottom => (_items.Count == 0 ? Location : _items.Last().Box.BottomMiddle) +
                    item.Location - item.Box.TopMiddle,
                GroupDirection.BottomToTop => (_items.Count == 0 ? Location : _items.Last().Box.TopMiddle) +
                    item.Location - item.Box.BottomMiddle + new Vector2d(0, _gutter),
                _ => throw new ArgumentOutOfRangeException()
            },
            GroupAlignment.End => _direction switch
            {
                GroupDirection.LeftToRight => (_items.Count == 0 ? Location : _items.Last().Box.BottomRight) +
                    item.Location - item.Box.BottomLeft + new Vector2d(_gutter, 0),
                GroupDirection.RightToLeft => (_items.Count == 0 ? Location : _items.Last().Box.BottomLeft) +
                    item.Location - item.Box.BottomRight + new Vector2d(-_gutter, 0),
                GroupDirection.TopToBottom => (_items.Count == 0 ? Location : _items.Last().Box.TopRight) +
                    item.Location - item.Box.BottomRight + new Vector2d(0, -_gutter),
                GroupDirection.BottomToTop => (_items.Count == 0 ? Location : _items.Last().Box.BottomRight) +
                    item.Location - item.Box.TopRight + new Vector2d(0, _gutter),
                _ => throw new ArgumentOutOfRangeException()
            },
            _ => throw new ArgumentOutOfRangeException()
        };

        _items.Add(item);

        // track in group wrapper
        item.Entities.ToList().ForEach(x => AddEntity(x));
    }
}

public enum GroupDirection
{
    LeftToRight,
    RightToLeft,
    TopToBottom,
    BottomToTop
}

public enum GroupAlignment
{
    Start,
    Middle,
    End
}