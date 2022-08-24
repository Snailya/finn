namespace FINN.SHAREDKERNEL.Constants;

public static class RoutingKeys
{
    public const string UpdateJobStatus = "api.job-status";

    #region Drawer

    public const string Draw = "drafter.draw";

    #endregion

    #region Cost

    public const string Estimate = "cost.estimate";

    #endregion

    #region Management

    public const string InsertBlock = "drafter.insert-block";

    #endregion

    #region Reader

    public const string ReadXlsx = "reader.read-excel";
    public const string ReadDxf = "reader.read-dxf";

    #endregion
}