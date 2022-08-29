using FINN.PLUGINS.DXF.Utils;
using FINN.SHAREDKERNEL.Models;
using netDxf.Entities;
using netDxf.Tables;

namespace FINN.PLUGINS.DXF.Models;

public class Group : DxfWrapper
{
    private readonly GroupAlignment _alignment;
    private readonly GroupDirection _direction;
    private readonly double _gutter;
    public readonly List<DxfWrapper> Items = new();

    public Group(Vector2d basePoint, GroupDirection direction, GroupAlignment alignment, double gutter) : base(
        Layer.Default,
        basePoint)
    {
        _direction = direction;
        _alignment = alignment;

        _gutter = gutter;

        OnBasePointChanged = value =>
        {
            Items.ForEach(x => x.BasePoint = x.BasePoint + value - BasePoint);
            Box.TransformBy(new Scale(1), value - BasePoint);
            OuterBox.TransformBy(new Scale(1), value - BasePoint);
        };

        OnAddToDocument = dxf =>
        {
            foreach (var item in Items) item.OnAddToDocument?.Invoke(dxf);

            if (string.IsNullOrEmpty(Label) || !IsLabelVisible) return;
            var label = TextUtil.CreateText(Label, Box.MiddleLeft - new Vector2d(gutter, 0), 1000,
                TextAlignment.MiddleRight);
            dxf.AddEntity(label);
        };
    }

    public string Label { get; set; }
    public bool IsLabelVisible { get; set; }

    public override IList<EntityObject> Entities => Items.SelectMany(x => x.Entities).ToList();

    public void Add(DxfWrapper item)
    {
        var point = _alignment switch
        {
            GroupAlignment.Start => _direction switch
            {
                GroupDirection.LeftToRight => (Items.Count == 0
                        ? BasePoint
                        : Items.Last().Box.TopRight + new Vector2d(_gutter, 0)) +
                    item.BasePoint - item.Box.TopLeft,
                GroupDirection.RightToLeft => (Items.Count == 0
                        ? BasePoint
                        : Items.Last().Box.TopLeft + new Vector2d(-_gutter, 0)) +
                    item.BasePoint - item.Box.TopRight,
                GroupDirection.TopToBottom => (Items.Count == 0
                        ? BasePoint
                        : Items.Last().Box.BottomLeft + new Vector2d(0, -_gutter)) +
                    item.BasePoint - item.Box.TopLeft,
                GroupDirection.BottomToTop => (Items.Count == 0
                        ? BasePoint
                        : Items.Last().Box.TopLeft + new Vector2d(0, _gutter)) +
                    item.BasePoint - item.Box.BottomLeft,
                _ => throw new ArgumentOutOfRangeException()
            },
            GroupAlignment.Middle => _direction switch
            {
                GroupDirection.LeftToRight => (Items.Count == 0
                        ? BasePoint
                        : Items.Last().Box.MiddleRight + new Vector2d(_gutter, 0)) +
                    item.BasePoint - item.Box.MiddleLeft,
                GroupDirection.RightToLeft => (Items.Count == 0
                        ? BasePoint
                        : Items.Last().Box.MiddleLeft + new Vector2d(-_gutter, 0)) +
                    item.BasePoint - item.Box.MiddleRight,
                GroupDirection.TopToBottom => (Items.Count == 0
                        ? BasePoint
                        : Items.Last().Box.BottomMiddle + new Vector2d(0, -_gutter)) +
                    item.BasePoint - item.Box.TopMiddle,
                GroupDirection.BottomToTop => (Items.Count == 0
                        ? BasePoint
                        : Items.Last().Box.TopMiddle + new Vector2d(0, _gutter)) +
                    item.BasePoint - item.Box.BottomMiddle,
                _ => throw new ArgumentOutOfRangeException()
            },
            GroupAlignment.End => _direction switch
            {
                GroupDirection.LeftToRight => (Items.Count == 0
                        ? BasePoint
                        : Items.Last().Box.BottomRight + new Vector2d(_gutter, 0)) +
                    item.BasePoint - item.Box.BottomLeft,
                GroupDirection.RightToLeft => (Items.Count == 0
                        ? BasePoint
                        : Items.Last().Box.BottomLeft + new Vector2d(-_gutter, 0)) +
                    item.BasePoint - item.Box.BottomRight,
                GroupDirection.TopToBottom => (Items.Count == 0
                        ? BasePoint
                        : Items.Last().Box.TopRight + new Vector2d(0, -_gutter)) +
                    item.BasePoint - item.Box.BottomRight,
                GroupDirection.BottomToTop => (Items.Count == 0
                        ? BasePoint
                        : Items.Last().Box.BottomRight + new Vector2d(0, _gutter)) +
                    item.BasePoint - item.Box.TopRight,
                _ => throw new ArgumentOutOfRangeException()
            },
            _ => throw new ArgumentOutOfRangeException()
        };

        item.BasePoint = point;
        Items.Add(item);

        // track in group wrapper
        Box.AddBox(item.Box);
        OuterBox.AddBox(item.OuterBox);
    }
}

public sealed class Group<T> : Group where T : DxfWrapper
{
    public Group(Vector2d basePoint, GroupDirection direction, GroupAlignment alignment, double gutter) : base(
        basePoint,
        direction, alignment, gutter)
    {
    }

    public new IEnumerable<T> Items => base.Items.OfType<T>();

    public void Add(T item)
    {
        base.Add(item);
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