namespace FINN.CORE.Models;

public class Dispatch : BaseEntity
{
    public DispatchTask Task { get; set; }
    public DispatchStatus Status { get; set; } = DispatchStatus.Pending;
    public string Input { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
}

public enum DispatchTask
{
    UploadBlocks,
    UpdateBlocks,
    DeleteBlock,
    DrawLayout,
    EstimateCost
}

public enum DispatchStatus
{
    Pending,
    Assigned,
    Done,
    Error
}