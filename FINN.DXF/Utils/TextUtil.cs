using FINN.DXF.Geometries;
using netDxf;
using netDxf.Entities;
using netDxf.Tables;

namespace FINN.PLUGINS.DXF.Utils;

public static class TextUtil
{
    private static readonly Dictionary<string, TextStyle> TextStyles = new();

    public static TextStyle GetStyleByName(string name)
    {
        if (TextStyles.TryGetValue(name, out var style)) return style;

        style = new TextStyle(name, "Arial.ttf");
        TextStyles.Add(name, style);
        return style;
    }

    public static Text CreateText(string text, Vector2d position, double scaleFactor,
        TextAlignment alignment = TextAlignment.BaselineLeft)
    {
        var style = GetStyleByName("Standard");

        // bug: unable to compute width in linux, this method need display server which the docker image not have.
        // var graphics = Graphics.FromHwnd(IntPtr.Zero);
        // var stringFont = new Font(style.FontFile, (float)(3 * scaleFactor));
        // var stringSize = graphics.MeasureString(text, stringFont);
        // var width = style.FontFile.EndsWith("shx") ? stringSize.Width * 1.5 : stringSize.Width

        var height = 3 * scaleFactor;
        return new Text(text, position.ToVector2(), height, style)
        {
            Layer = LayerUtil.GetText(),
            Width = text.Length * 1.25 * height,
            Alignment = alignment
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

        // bug: unable to compute width in linux, this method need display server which the docker image not have.
        // var graphics = Graphics.FromHwnd(IntPtr.Zero);
        // var stringFont =
        //     new Font(style.FontFile,
        //         (float)(2.5 *
        //                 scaleFactor)); // bug: unable to load shx font, which will return the default font of font class Microsoft Sans Serif
        // var stringSize = graphics.MeasureString(text, stringFont);
        // var width = style.FontFile.EndsWith("shx") ? stringSize.Width * 1.5 : stringSize.Width

        var height = 2.5 * scaleFactor;
        return new MText(text, position.ToVector2(), height, text.Length * 1.25 * height, style)
            { Layer = LayerUtil.GetText(), AttachmentPoint = attachment };
    }

    public static void Initialize(DxfDocument dxf)
    {
        TextStyles.Clear();

        dxf.TextStyles["Standard"].FontFamilyName = "Microsoft YaHei";

        dxf.TextStyles.Items.ToList().ForEach(x => TextStyles.Add(x.Name, x));
    }
}