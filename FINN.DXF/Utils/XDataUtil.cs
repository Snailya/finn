using netDxf;
using netDxf.Entities;
using netDxf.Tables;

namespace FINN.PLUGINS.DXF.Utils;

public static class XDataUtil
{
    private static readonly Dictionary<string, ApplicationRegistry> Registries = new();

    public static ApplicationRegistry GetRegistryByName(string name)
    {
        if (Registries.TryGetValue(name, out var registry)) return registry;

        registry = new ApplicationRegistry(name);
        Registries.Add(name, registry);
        return registry;
    }

    public static void RegistryAsGrid(EntityObject entity, double level)
    {
        var appReg = GetRegistryByName("FINN.GRID");

        var xData = new XData(appReg);
        xData.XDataRecord.Add(new XDataRecord(XDataCode.String, level.ToString()));
        entity.XData.Add(xData);
    }
}