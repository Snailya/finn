using netDxf;

namespace FINN.DRAFTER.Utils;

public static class DocUtil
{
    public static DxfDocument CreateDoc()
    {
        var dxf = new DxfDocument();

        LayerUtil.Initialize(dxf);
        DimUtil.Initialize(dxf);

        return dxf;
    }
}