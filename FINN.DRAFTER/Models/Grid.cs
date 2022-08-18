﻿using FINN.DRAFTER.Extensions;
using FINN.DRAFTER.Utils;
using FINN.SHAREDKERNEL.Dtos;
using FINN.SHAREDKERNEL.Dtos.Draw;
using FINN.SHAREDKERNEL.Models;
using netDxf;
using netDxf.Blocks;
using netDxf.Entities;
using netDxf.Tables;

namespace FINN.DRAFTER.Models;

public class Grid : DxfWrapper
{
    /// <summary>
    /// The origin point of the grid, used to location plates.
    /// </summary>
    public Vector2d Origin => BasePoint;

    /// <summary>
    /// 
    /// </summary>
    public double Level { get; }

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
        PopulateLevelLabel($"+{Level / 1000}m层");
    }

    public Grid(IEnumerable<double> xCoordinates, IEnumerable<double> yCoordinates, double columnXLength,
        double columnYLength, double level) : this(Vector2d.Zero, xCoordinates, yCoordinates,
        columnXLength, columnYLength, level)
    {
    }

    #endregion

    private void PopulateLevelLabel(string level)
    {
        var levelLabel = TextUtil.CreateText(level, Box.TopLeft + new Vector2d(0, 1600), 500);
        AddEntity(levelLabel);
    }

    private void PopulateLinesWidthLabelAndDims(IEnumerable<double> coordinates, double length,
        PopulateDirection direction)
    {
        var lines = coordinates.Select(x =>
        {
            var line = direction == PopulateDirection.Horizontal
                ? EntityUtil.CreateLine(new Vector2d(x, 0) - new Vector2d(0, 6400),
                    new Vector2d(x, length) + new Vector2d(0, 6400))
                : EntityUtil.CreateLine(new Vector2d(length, x) + new Vector2d(6400, 0),
                    new Vector2d(0, x) - new Vector2d(6400, 0));
            line.TransformBy(Scale.Identity, BasePoint);
            // line.TransformBy(new Scale(((line.StartPoint + line.EndPoint) / 2).ToVector2d(), 1.2), Location);
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
}