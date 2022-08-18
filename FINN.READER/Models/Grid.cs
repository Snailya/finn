﻿using FINN.SHAREDKERNEL.Dtos;
using FINN.SHAREDKERNEL.Dtos.Draw;

namespace FINN.READER.Models;

internal class Grid
{
    /// <summary>
    ///     The length of column in x direction.
    /// </summary>
    public Length ColumnLength { get; set; }

    /// <summary>
    ///     The length of column in y direction.
    /// </summary>
    public Length ColumnWidth { get; set; }

    /// <summary>
    ///     The coordinates of grid line in x direction.
    /// </summary>
    public Length[] XCoordinates { get; set; }

    /// <summary>
    ///     The coordinates of grid line in y direction.
    /// </summary>
    public Length[] YCoordinates { get; set; }

    /// <summary>
    ///     The label of the current grid.
    /// </summary>
    public Length Level { get; set; }

    public GridDto ToDto()
    {
        return new GridDto
        {
            Level = Level.ToMillimeter(),
            ColumnXLength = ColumnLength.ToMillimeter(),
            ColumnYLength = ColumnWidth.ToMillimeter(),
            XCoordinates = XCoordinates.Select(x => x.ToMillimeter()).ToArray(),
            YCoordinates = YCoordinates.Select(x => x.ToMillimeter()).ToArray()
        };
    }
}