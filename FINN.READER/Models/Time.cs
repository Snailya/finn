using System.Text.RegularExpressions;

namespace FINN.READER.Models;

public class Time
{
    public enum Unit
    {
        Minute,
        Unknown
    }

    private readonly Unit _unit;

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

    public double Value { get; set; }

    public static Unit ParseUnit(string unitStr)
    {
        var regex = new Regex(@"(min|minute|minutes)");
        var matches = regex.Matches(unitStr);
        switch (matches[0].Value.ToLower())
        {
            case "min":
                return Unit.Minute;
            case "minute":
                return Unit.Minute;
            case "minutes":
                return Unit.Minute;
            default:
                return Unit.Unknown;
        }
    }

    public override string ToString()
    {
        switch (_unit)
        {
            case Unit.Minute:
                return $"{Value}min";
            case Unit.Unknown:
                return $"{Value}";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}