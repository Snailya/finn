using System.Text.RegularExpressions;

namespace FINN.READER.Models;

public class Length
{
    public enum Unit
    {
        Meter,
        Millimeter,
        Unknown
    }

    private readonly Unit _unit;

    public Length(double value, Unit unit)
    {
        Value = value;
        _unit = unit;
    }

    public Length(double value, string unitStr)
    {
        Value = value;
        _unit = ParseUnit(unitStr);
    }

    public double Value { get; set; }

    public static Unit ParseUnit(string unitStr)
    {
        var regex = new Regex(@"(mm|m)");
        var matches = regex.Matches(unitStr);
        switch (matches[0].Value.ToLower())
        {
            case "mm":
                return Unit.Millimeter;
            case "m":
                return Unit.Meter;
            default:
                return Unit.Unknown;
        }
    }

    public override string ToString()
    {
        switch (_unit)
        {
            case Unit.Meter:
                return $"{Value}m";
            case Unit.Millimeter:
                return $"{Value}mm";
            case Unit.Unknown:
                return $"{Value}";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public double ToMillimeter()
    {
        switch (_unit)
        {
            case Unit.Meter:
                return Value * 1000;
            case Unit.Millimeter:
                return Value;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}