using System.Text.Json.Serialization;
using FINN.CORE.Models;

namespace FINN.SHAREDKERNEL.Dtos;

public class CostDto : JsonObject
{
    public string Type { get; set; }
    public double Total { get; set; }
}