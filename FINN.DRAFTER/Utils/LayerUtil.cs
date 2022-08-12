using netDxf;
using netDxf.Tables;

namespace FINN.DRAFTER.Utils;

public static class LayerUtil
{
    private static readonly Dictionary<string, Layer> _layers = new();

    public static Layer GetAxisText()
    {
        const string name = "AXIS_TEXT";
        if (_layers.TryGetValue(name, out var layer)) return layer;

        layer = new Layer(name);
        _layers.Add(name, layer);
        return layer;
    }

    public static Layer GetCentralLine()
    {
        const string name = "P-X";
        if (_layers.TryGetValue(name, out var layer)) return layer;

        layer = new Layer(name) { Color = AciColor.Red, Linetype = Linetype.DashDot };
        _layers.Add(name, layer);
        return layer;
    }

    public static Layer GetText()
    {
        const string name = "P-Text";
        if (_layers.TryGetValue(name, out var layer)) return layer;

        layer = new Layer(name);
        _layers.Add(name, layer);
        return layer;
    }

    public static Layer GetAxis()
    {
        const string name = "A_AXIS";
        if (_layers.TryGetValue(name, out var layer)) return layer;

        layer = new Layer(name) { Color = new AciColor(133) };
        _layers.Add(name, layer);
        return layer;
    }

    public static Layer GetDim()
    {
        const string name = "P-Dim";
        if (_layers.TryGetValue(name, out var layer)) return layer;

        layer = new Layer(name);
        _layers.Add(name, layer);
        return layer;
    }

    public static Layer GetLayerByName(string name)
    {
        if (_layers.TryGetValue(name, out var layer)) return layer;

        layer = new Layer(name);
        _layers.Add(name, layer);
        return layer;
    }
}