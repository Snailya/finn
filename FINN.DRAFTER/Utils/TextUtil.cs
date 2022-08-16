using System.Drawing;
using FINN.DRAFTER.Extensions;
using FINN.SHAREDKERNEL.Models;
using netDxf;
using netDxf.Entities;
using netDxf.Tables;

namespace FINN.DRAFTER.Utils;

public static class TextUtil
{
    private static readonly Dictionary<string, TextStyle> TextStyles = new();

    public static TextStyle GetStyleByName(string name)
    {
        if (TextStyles.TryGetValue(name, out var style)) return style;

        style = new TextStyle(name);
        TextStyles.Add(name, style);
        return style;
    }

    public static Text CreateText(string text, Vector2d position, double scaleFactor)
    {
        var style = GetStyleByName("Standard");

        // compute width
        var graphics = Graphics.FromHwnd(IntPtr.Zero);
        var stringFont = new Font(style.FontFile, (float)(3 * scaleFactor));
        var stringSize = graphics.MeasureString(text, stringFont);

        return new Text(text, position.ToVector2(), 3 * scaleFactor, style)
        {
            Layer = LayerUtil.GetText(),
            Width = style.FontFile.EndsWith("shx") ? stringSize.Width * 1.5 : stringSize.Width
        };
    }

    public static EntityObject CreateMText(string text, Vector2d position, double scaleFactor)
    {
        return CreateMText(text, position, MTextAttachmentPoint.MiddleCenter, scaleFactor);
    }

    public static EntityObject CreateMText(string text, Vector2d position, MTextAttachmentPoint attachment
        , double scaleFactor)
    {
        var style = GetStyleByName("Standard");

        // compute width
        var graphics = Graphics.FromHwnd(IntPtr.Zero);
        var stringFont =
            new Font(style.FontFile,
                (float)(2.5 *
                        scaleFactor)); // bug: unable to load shx font, which will return the default font of font class Microsoft Sans Serif
        var stringSize = graphics.MeasureString(text, stringFont);

        return new MText(text, position.ToVector2(), 2.5 * scaleFactor,
                style.FontFile.EndsWith("shx") ? stringSize.Width * 1.5 : stringSize.Width,
                style)
            { Layer = LayerUtil.GetText(), AttachmentPoint = attachment };
    }

    public static void Initialize(DxfDocument dxf)
    {
        TextStyles.Clear();

        dxf.TextStyles.Items.ToList().ForEach(x => TextStyles.Add(x.Name, x));
    }
}