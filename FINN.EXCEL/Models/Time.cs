using System.Text.RegularExpressions;

namespace FINN.EXCEL.Models;

public class Time
{
    public enum Unit
    {
        Minute,
        Unknown
    }

    private readonly Unit _unit;

    public double Value { get; set; }

    public static Unit ParseUnit(string unitStr)
    {
        var regex = new Regex(@"(min|minute|minutes)");
        var matches = regex.Matches(unitStr);
        return matches[0].Value.ToLower() switch
        {
            "min" => Unit.Minute,
            "minute" => Unit.Minute,
            "minutes" => Unit.Minute,
            _ => Unit.Unknown
        };
    }

    public override string ToString()
    {
        return _unit switch
        {
            Unit.Minute => $"{Value}min",
            Unit.Unknown => $"{Value}",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    #region Constructors

    public Time(double value, Unit unit)
    {
        Value = value;
        _unit = unit;
    }

    public Time(double value, string unitStr)
    {
        Value = value;
        _unit = ParseUnit(unitStr);
    }

    #endregion
}