using netDxf;

namespace FINN.PLUGINS.DXF.Utils;

public static class DocUtil
{
    public static DxfDocument CreateDoc()
    {
        var dxf = new DxfDocument();

        LayerUtil.Initialize(dxf);
        DimUtil.Initialize(dxf);
        TextUtil.Initialize(dxf);

        return dxf;
    }
}