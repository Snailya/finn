using FINN.PLUGINS.DXF.Utils;
using FINN.SHAREDKERNEL.Models;
using FINN.SHAREDKERNEL.UseCases;
using netDxf;
using netDxf.Blocks;
using netDxf.Entities;
using netDxf.Tables;

namespace FINN.PLUGINS.DXF.Models;

public sealed class Grid : DxfWrapper
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

    /// <summary>
    ///     The origin point of the grid, used to location plates.
    /// </summary>
    public Vector2d Origin => BasePoint;

    /// <summary>
    ///     The height of the grid that represents.
    /// </summary>
    public double Level { get; }

    /// <summary>
    ///     The label for the item.
    /// </summary>
    public string Label => $"+{Level / 1000}m层";

    private void PopulateLabel()
    {
        var levelLabel = TextUtil.CreateText(Label, Box.TopLeft + new Vector2d(0, 1600), 500);
        AddEntity(levelLabel);
    }

    private void PopulateLinesWidthLabelAndDims(IEnumerable<double> coordinates, double length,
        PopulateDirection direction)
    {
        var lines = coordinates.Select((value, index) =>
        {
            var line = direction == PopulateDirection.Horizontal
                ? EntityUtil.CreateLine(new Vector2d(value, 0) - new Vector2d(0, 6400),
                    new Vector2d(value, length) + new Vector2d(0, 6400))
                : EntityUtil.CreateLine(new Vector2d(length, value) + new Vector2d(6400, 0),
                    new Vector2d(0, value) - new Vector2d(6400, 0));
            line.TransformBy(Scale.Identity, BasePoint);
            AddEntity(line);

            // label content
            var labelContent = direction switch
            {
                PopulateDirection.Horizontal => (index + 1).ToString(),
                PopulateDirection.Vertical => ((char)(index + 65)).ToString()
            };

            var label1 =
                new Insert(_axios, line.StartPoint - Vector3.Normalize(line.Direction) * 1600)
                    { Scale = new Vector3(1600, 1600, 1600) };
            label1.Attributes.AttributeWithTag("A").Value = labelContent;
            label1.Sync();

            var label2 = label1.Clone() as Insert;
            label2.Position = line.EndPoint + Vector3.Normalize(line.Direction) * 1600;
            label2.Sync();
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

        var prototype = EntityUtil.CreateLwPolyline(true, new Vector2d(-columnXLength / 2, -columnYLength / 2),
            new Vector2d(-columnXLength / 2, columnYLength / 2), new Vector2d(columnXLength / 2, columnYLength / 2),
            new Vector2d(columnXLength / 2, -columnYLength / 2));

        foreach (var (x, y) in intersections)
        {
            var column = (LwPolyline)prototype.Clone();
            column.TransformBy(new Scale(1), BasePoint + new Vector2d(x, y));
            AddEntity(column);
        }
    }

    public static Grid FromDto(GridDto dto)
    {
        return new Grid(dto.XCoordinates, dto.YCoordinates,
            dto.ColumnXLength,
            dto.ColumnYLength, dto.Level);
    }

    private enum PopulateDirection
    {
        Horizontal,
        Vertical
    }

    #region Constructors

    private Grid(Vector2d basePoint, IEnumerable<double> xCoordinates, IEnumerable<double> yCoordinates,
        double columnXLength,
        double columnYLength, double level) : base(Layer.Default, basePoint)
    {
        Level = level;

        var xLength = xCoordinates.Max();
        var yLength = yCoordinates.Max();

        PopulateLinesWidthLabelAndDims(xCoordinates, yLength, PopulateDirection.Horizontal);
        PopulateLinesWidthLabelAndDims(yCoordinates, xLength, PopulateDirection.Vertical);
        PopulateColumns(xCoordinates, yCoordinates, columnXLength, columnYLength);
        PopulateLabel();

        // convert to block
        var dims = Entities.Where(x => x is Dimension).ToList();
        var entitiesWithoutDimension = Entities.Except(dims);
        var block = new Block($"grid{Level}", entitiesWithoutDimension) { Origin = basePoint.ToVector3() };
        var insert = new Insert(block, basePoint.ToVector3());
        Entities.Clear();
        dims.ForEach(x => Entities.Add(x));
        Entities.Add(insert);

        // append xdata, which used for get grid level
        XDataUtil.RegistryAsGrid(insert, Level);
    }

    public Grid(IEnumerable<double> xCoordinates, IEnumerable<double> yCoordinates, double columnXLength,
        double columnYLength, double level) : this(Vector2d.Zero, xCoordinates, yCoordinates,
        columnXLength, columnYLength, level)
    {
    }

    #endregion
}