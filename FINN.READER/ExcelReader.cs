using System.Data;
using FINN.READER.Models;
using FINN.SHAREDKERNEL.Dtos;
using FINN.SHAREDKERNEL.Interfaces;

namespace FINN.READER;

public class ExcelReader : IReader
{
    public IEnumerable<GridDto> ReadAsGrid(DataTable dataTable)
    {
        // read column geometry
        var columnLength = Convert.ToDouble(dataTable.Rows[0][2]);
        var columnWidth = Convert.ToDouble(dataTable.Rows[0][4]);

        // find section indicator
        var dividers = dataTable.AsEnumerable().Where(x => x[0] is string str && str == "#")
            .Select(x => dataTable.Rows.IndexOf(x)).ToList();
        var sections = dividers.Take(dividers.Count - 1)
            .Zip(dividers.Skip(1), (l1, l2) => (start: l1 + 1, end: l2 - 1));

        var list = new List<Grid>();

        foreach (var (start, end) in sections)
        {
            var grid = new Grid
            {
                Label = Convert.ToString(dataTable.Rows[start][0]) ?? string.Empty,
                ColumnLength = new Length(columnLength, Length.Unit.Millimeter),
                ColumnWidth = new Length(columnWidth, Length.Unit.Millimeter)
            };

            var yDelta = new List<double>();
            var xDelta = new List<double>();
            // read in vertical direction
            for (var i = start + 1; i < end + 1; i++)
                if (dataTable.Rows[i][0] is double value1)
                    yDelta.Add(value1);
                else
                    // read in horizontal direction
                    for (var j = 1; j < dataTable.Columns.Count - 1; j++)
                        if (dataTable.Rows[i][j] is double value2)
                            xDelta.Add(value2);
                        else
                            break;

            yDelta.Reverse();
            grid.YCoordinates = yDelta.Aggregate(new List<double> { 0 }, (dict, next) =>
            {
                dict.Add(dict.LastOrDefault() + next);
                return dict;
            }).Select(x => new Length(x, Length.Unit.Meter)).ToArray();
            grid.XCoordinates = xDelta.Aggregate(new List<double> { 0 }, (dict, next) =>
            {
                dict.Add(dict.LastOrDefault() + next);
                return dict;
            }).Select(x => new Length(x, Length.Unit.Meter)).ToArray();
            list.Add(grid);
        }

        return list.Select(x => x.ToDto());
    }

    public IEnumerable<ProcessDto> ReadAsProcessList(DataTable dataTable)
    {
        // find the terminate row
        var end =
            dataTable.AsEnumerable().Where(x => x[0] is string str && str == "#")
                .Select(x => dataTable.Rows.IndexOf(x))?.Max() ?? dataTable.Rows.Count;

        // read the units;
        var timeUint = Time.ParseUnit(Convert.ToString(dataTable.Rows[6][5]) ?? string.Empty);
        var lengthUnit = Length.ParseUnit(Convert.ToString(dataTable.Rows[6][6]) ?? string.Empty);

        // data range is from the 11th row.
        // detect the segment from column A, each segment is recognized by where the first row's text is not empty and the next row of the last row is not empty
        var list = new List<Process>();
        for (var line = 10; line < end; line++)
        {
            // treat as empty row if name is empty, and skip it
            if (Convert.ToString(dataTable.Rows[line][2]) == null) continue;

            // read data
            var layer = Convert.ToString(dataTable.Rows[line][0]) ?? string.Empty;
            var index = Convert.ToString(dataTable.Rows[line][1]) ?? string.Empty;
            var name = Convert.ToString(dataTable.Rows[line][2]) ?? string.Empty;
            if (string.IsNullOrEmpty(name)) continue; // treat as empty line

            var rh = Convert.ToString(dataTable.Rows[line][6]) ?? string.Empty;
            var time = dataTable.Rows[line][5] is double vt
                ? new Time(vt, timeUint).ToString()
                : Convert.ToString(dataTable.Rows[line][5]) ?? string.Empty;
            var width = dataTable.Rows[line][6] is double vw ? vw : 0;
            var length = dataTable.Rows[line][7] is double vl ? vl : 0;
            var speed = Convert.ToString(dataTable.Rows[line][8]) ?? string.Empty;
            var cyclePitch = Convert.ToString(dataTable.Rows[line][9]) ?? string.Empty;
            var cycleTime = Convert.ToString(dataTable.Rows[line][10]) ?? string.Empty;
            var grossCapacity = Convert.ToString(dataTable.Rows[line][11]) ?? string.Empty;

            // add to list
            var process = new Process
            {
                Layer = layer,
                Index = index,
                Name = name,
                RH = rh,
                TimeString = time,
                Width = new Length(width, lengthUnit),
                Length = new Length(length, lengthUnit),
                Speed = speed,
                CyclePitch = cyclePitch,
                CycleTime = cycleTime,
                GrossCapacity = grossCapacity
            };

            // treat as process if the index text could be parse into int, subprocess otherwise
            if (int.TryParse(index, out var result))
            {
                list.Add(process);
            }
            else
            {
                var parent = list.LastOrDefault();
                if (parent != null)
                {
                    process.Layer = parent.Layer;
                    parent?.SubProcess.Add(process);
                }
            }
        }

        return list.Select(x => x.ToLayoutProcessDto());
    }
}