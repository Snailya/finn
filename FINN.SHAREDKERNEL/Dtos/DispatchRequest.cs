using FINN.CORE.Models;
using FINN.SHAREDKERNEL.UseCases;

namespace FINN.SHAREDKERNEL.Dtos;

public class DispatchRequest : JsonObject
{
    public DispatchTask Task { get; set; }
    public string Data { get; set; }
}