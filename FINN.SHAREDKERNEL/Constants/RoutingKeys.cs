namespace FINN.SHAREDKERNEL.Constants;

public static class RoutingKeys
{
    public static class ExcelService
    {
        public const string GetLayout = "excel-service.read-excel";
    }

    public static class DxfService
    {
        public const string ListBlockDefinitions = "dxf-service.list-block-definitions";
        public const string GetBlockDefinition = "dxf-service.get-block-definition";
        public const string AddBlockDefinitions = "dxf-service.add-block-definitions";
        public const string DeleteBlockDefinition = "dxf-service.delete-block-definition";
        public const string DrawLayout = "dxf-service.draw-layout";
        public const string ReadLayout = "dxf-service.read-layout";
    }

    public static class CostService
    {
        public const string EstimateCost = "cost-service.estimate-cost";
    }
}