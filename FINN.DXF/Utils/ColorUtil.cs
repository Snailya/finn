using netDxf;
using netDxf.Entities;
using netDxf.Tables;

namespace FINN.PLUGINS.DXF.Utils;

public static class ColorUtil
{
    public static Hatch GetHatchMatchesLayer(Layer layer, IEnumerable<HatchBoundaryPath> paths)
    {
        return layer.Name switch
        {
            "S-SP-WA" => new Hatch(HatchPattern.Solid, paths, true) { Color = AciColor.FromCadIndex(31) },
            "P-SP-WB" => new Hatch(HatchPattern.Solid, paths, true) { Color = AciColor.FromCadIndex(40) },
            "P-PETC-ST" => new Hatch(HatchPattern.Solid, paths, true) { Color = AciColor.FromCadIndex(140) },
            "P-SP-SP" => new Hatch(HatchPattern.Solid, paths, true) { Color = AciColor.Green },
            "P-OV-H" => new Hatch(HatchPattern.Solid, paths, true) { Color = AciColor.FromCadIndex(20) },
            "P-OV-C" => new Hatch(HatchPattern.Solid, paths, true) { Color = AciColor.FromCadIndex(141) },
            "S-SP-F" => new Hatch(HatchPattern.Solid, paths, true) { Color = AciColor.FromCadIndex(51) },
            "P-rd" => new Hatch(new HatchPattern("ANSI131"), paths, true) { Color = AciColor.FromCadIndex(253) },
            "P-SP-AS" => new Hatch(HatchPattern.Solid, paths, true) { Color = AciColor.FromCadIndex(133) },
            _ => new Hatch(HatchPattern.Solid, paths, true) { Color = AciColor.FromCadIndex(8) }
        };
    }
}