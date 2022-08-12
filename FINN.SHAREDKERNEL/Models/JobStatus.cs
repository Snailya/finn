using System.Text.Json.Serialization;

namespace FINN.SHAREDKERNEL.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum JobStatus
{
    Error = 0,
    Pending = 10,
    Reading = 11,
    Drawing = 12,
    Ready = 13
}