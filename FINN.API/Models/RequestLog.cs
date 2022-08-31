using FINN.CORE.Models;

namespace FINN.API.Models;

public class RequestLog : BaseEntity
{
    public string? Ip { get; set; }
    public string? RequestType { get; set; }
    public string? Status { get; set; }
    public string? Input { get; set; }
    public string? Output { get; set; }
}