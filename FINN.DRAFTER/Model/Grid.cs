using FINN.DRAFTER.Utils;
using FINN.SHAREDKERNEL.Models;
using netDxf;
using netDxf.Blocks;
using netDxf.Entities;
using netDxf.Tables;

namespace FINN.DRAFTER.Model;

public class Grid : DxfWrapper
{
    public Grid(Vector2d location, double[] xCoordinates, double[] yCoordinates, double columnXLength,
        double columnYLength) : base(Layer.Default, location)
    {
        var (xLines, yLines) = PrepareGridLine(xCoordinates, yCoordinates);
        PrepareGridIndex(xLines);
        PrepareGridIndex(yLines);
        PrepareContinuesDimensions(xLines);
        PrepareContinuesDimensions(yLines);
        PrepareColumns(xCoordinates, yCoordinates, columnXLength, columnYLength);
    }

    public Grid(double[] xCoordinates, double[] yCoordinates, double columnXLength,
        double columnYLength) : this(Vector2d.Zero, xCoordinates, yCoordinates,
        columnXLength, columnYLength)
    {
    }

    private void PrepareGridIndex(IEnumerable<Line> lines)
    {
        var block = new Block("_Axios", new[] { EntityUtil.CreateCircle(LayerUtil.GetAxis(), Vector2d.Zero, 1) },
            new[]
            {
                new AttributeDefinition("A", 1,
                    new TextStyle("COMPLEX", "complex.shx"))
                {
                    Alignment = TextAlignment.MiddleCenter, Layer = LayerUtil.GetAxisText(), Value = "X",
                    IsVisible = true, Position = Vector3.Zero
                }
            });
        foreach (var line in lines)
        {
            var ins1 = new Insert(block, line.StartPoint, 1600);
            var ins2 = new Insert(block, line.EndPoint, 1600);

            AddEntity(ins1);
            AddEntity(ins2);
        }
    }

    private (IEnumerable<Line>, IEnumerable<Line>) PrepareGridLine(double[] xCoordinates, double[] yCoordinates)
    {
        var xLines = new List<Line>();
        var yLines = new List<Line>();

        var xLength = xCoordinates.Max();
        var yLength = yCoordinates.Max();

        foreach (var coordinate in xCoordinates)
        {
            var line = EntityUtil.CreateLine(new Vector2d(coordinate, 0), new Vector2d(coordinate, yLength));
            line.TransformBy(new Scale(((line.StartPoint + line.EndPoint) / 2).ToVector2d(), 1.2), Location);
            AddEntity(line);

            xLines.Add(line);
        }

        foreach (var coordinate in yCoordinates)
        {
            var line = EntityUtil.CreateLine(new Vector2d(0, coordinate), new Vector2d(xLength, coordinate));
            line.TransformBy(new Scale(((line.StartPoint + line.EndPoint) / 2).ToVector2d(), 1.2), Location);
            AddEntity(line);

            yLines.Add(line);
        }

        return (xLines, yLines);
    }

    private void PrepareColumns(IEnumerable<double> xCoordinates, IEnumerable<double> yCoordinates,
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

    private void PrepareContinuesDimensions(IEnumerable<Line> lines)
    {
        var enumerable = lines.ToList();
        // start point
        enumerable.Skip(1)
            .Zip(enumerable.Take(enumerable.Count - 1), (l1, l2) => (p1: l1.StartPoint, p2: l2.StartPoint)).ToList()
            .ForEach(
                i =>
                {
                    var dim = DimUtil.CreateAlignedDim(i.p1.ToVector2d(), i.p2.ToVector2d(), 200);
                    AddEntity(dim, false);
                });
        // end point
        enumerable.Skip(1)
            .Zip(enumerable.Take(enumerable.Count - 1), (l1, l2) => (p1: l1.EndPoint, p2: l2.EndPoint)).ToList()
            .ForEach(
                i =>
                {
                    var dim = DimUtil.CreateAlignedDim(i.p1.ToVector2d(), i.p2.ToVector2d(), 200);
                    AddEntity(dim, false);
                });
    }
}