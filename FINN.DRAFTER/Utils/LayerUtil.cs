using netDxf;
using netDxf.Tables;

namespace FINN.DRAFTER.Utils;

public static class LayerUtil
{
    private static readonly Dictionary<string, Layer> Layers = new();

    public static Layer GetAxisText()
    {
        const string name = "AXIS_TEXT";
        if (Layers.TryGetValue(name, out var layer)) return layer;

        layer = new Layer(name);
        Layers.Add(name, layer);
        return layer;
    }

    public static Layer GetCentralLine()
    {
        const string name = "P-X";
        if (Layers.TryGetValue(name, out var layer)) return layer;

        layer = new Layer(name) { Color = AciColor.Red, Linetype = Linetype.DashDot };
        Layers.Add(name, layer);
        return layer;
    }

    public static Layer GetText()
    {
        const string name = "P-Text";
        if (Layers.TryGetValue(name, out var layer)) return layer;

        layer = new Layer(name);
        Layers.Add(name, layer);
        return layer;
    }

    public static Layer GetAxis()
    {
        const string name = "A_AXIS";
        if (Layers.TryGetValue(name, out var layer)) return layer;

        layer = new Layer(name) { Color = new AciColor(133) };
        Layers.Add(name, layer);
        return layer;
    }

    public static Layer GetDim()
    {
        const string name = "P-Dim";
        if (Layers.TryGetValue(name, out var layer)) return layer;

        layer = new Layer(name);
        Layers.Add(name, layer);
        return layer;
    }

    public static Layer GetLayerByName(string name)
    {
        if (Layers.TryGetValue(name, out var layer)) return layer;

        layer = new Layer(name);
        Layers.Add(name, layer);
        return layer;
    }

    public static void Initialize(DxfDocument dxf)
    {
        Layers.Clear();

        dxf.Layers.StateManager.Import("Template.las", true);
        dxf.Layers.Items.ToList().ForEach(x => Layers.Add(x.Name, x));
    }
}