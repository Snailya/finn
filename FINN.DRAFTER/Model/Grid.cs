using FINN.DRAFTER.Extensions;
using FINN.DRAFTER.Utils;
using FINN.SHAREDKERNEL.Dtos;
using FINN.SHAREDKERNEL.Models;
using netDxf;
using netDxf.Blocks;
using netDxf.Entities;
using netDxf.Tables;

namespace FINN.DRAFTER.Model;

public class Grid : DxfWrapper
{
    private readonly Block _axios = new("_Axios",
        new[] { EntityUtil.CreateCircle(LayerUtil.GetAxis(), Vector2d.Zero, 1) },
        new[]
        {
            new AttributeDefinition("A", 1,
                new TextStyle("COMPLEX", "complex.shx"))
            {
                Alignment = TextAlignment.MiddleCenter, Layer = LayerUtil.GetAxisText(), Value = "X",
                IsVisible = true, Position = Vector3.Zero
            }
        });

    public Grid(Vector2d location, double[] xCoordinates, double[] yCoordinates, double columnXLength,
        double columnYLength) : base(Layer.Default, location)
    {
        var xLength = xCoordinates.Max();
        var yLength = yCoordinates.Max();

        PopulateLinesWidthLabelAndDims(xCoordinates, yLength, PopulateDirection.Horizontal);
        PopulateLinesWidthLabelAndDims(yCoordinates, xLength, PopulateDirection.Vertical);
        PopulateColumns(xCoordinates, yCoordinates, columnXLength, columnYLength);
    }

    public Grid(double[] xCoordinates, double[] yCoordinates, double columnXLength,
        double columnYLength) : this(Vector2d.Zero, xCoordinates, yCoordinates,
        columnXLength, columnYLength)
    {
    }

    private void PopulateLinesWidthLabelAndDims(double[] coordinates, double length, PopulateDirection direction)
    {
        var lines = coordinates.Select(x =>
        {
            var line = direction == PopulateDirection.Horizontal
                ? EntityUtil.CreateLine(new Vector2d(x, 0), new Vector2d(x, length))
                : EntityUtil.CreateLine(new Vector2d(length, x), new Vector2d(0, x));
            line.TransformBy(new Scale(((line.StartPoint + line.EndPoint) / 2).ToVector2d(), 1.2), Location);
            AddEntity(line);

            var label1 =
                new Insert(_axios, line.StartPoint - Vector3.Normalize(line.Direction) * 1600)
                    { Scale = new Vector3(1600, 1600, 1600) };
            var label2 = new Insert(_axios, line.EndPoint + Vector3.Normalize(line.Direction) * 1600)
                { Scale = new Vector3(1600, 1600, 1600) };
            AddEntity(label1);
            AddEntity(label2);

            return (Line: line, Labels: new[] { label1, label2 });
        }).ToList();

        var dims = lines.Take(lines.Count - 1)
            .Zip(lines.Skip(1),
                (l1, l2) =>
                {
                    return new[]
                    {
                        new Line(l1.Line.StartPoint, l2.Line.StartPoint), new Line(l2.Line.EndPoint, l1.Line.EndPoint)
                    };
                }).SelectMany(x => x).Select(x =>
            {
                var dim = DimUtil.CreateAlignedDim(x, 200);
                AddEntity(dim);
                return dim;
            }).ToList();
    }

    private void PopulateColumns(IEnumerable<double> xCoordinates, IEnumerable<double> yCoordinates,
        double columnXLength,
        double columnYLength)
    {
        var intersections = xCoordinates.SelectMany(x => yCoordinates, (x, y) => (x, y));

        var prototype = EntityUtil.CreatePolyline(true, new Vector2d(-columnXLength / 2, -columnYLength / 2),
            new Vector2d(-columnXLength / 2, columnYLength / 2), new Vector2d(columnXLength / 2, columnYLength / 2),
            new Vector2d(columnXLength / 2, -columnYLength / 2));

        foreach (var (x, y) in intersections)
        {
            var column = (Polyline)prototype.Clone();
            column.TransformBy(new Scale(1), Location + new Vector2d(x, y));
            AddEntity(column);
        }
    }

    public static Grid FromDto(GridDto dto)
    {
        return new Grid(dto.XCoordinates, dto.YCoordinates,
            dto.ColumnXLength,
            dto.ColumnYLength);
    }

    private enum PopulateDirection
    {
        Horizontal,
        Vertical
    }
}