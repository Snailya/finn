﻿using System.Text.RegularExpressions;

namespace FINN.READER.Models;

public class Length
{
    public static Length Zero = new Length(0, Unit.Millimeter);

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
        return _unit switch
        {
            Unit.Meter => $"{Value}m",
            Unit.Millimeter => $"{Value}mm",
            Unit.Unknown => $"{Value}",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public double ToMillimeter()
    {
        return _unit switch
        {
            Unit.Meter => Value * 1000,
            Unit.Millimeter => Value,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static Length operator +(Length l1, Length l2)
    {
        return new Length(l1.ToMillimeter() + l2.ToMillimeter(), Unit.Millimeter);
    }

    public static Length operator -(Length l1, Length l2)
    {
        return new Length(l1.ToMillimeter() - l2.ToMillimeter(), Unit.Millimeter);
    }
}