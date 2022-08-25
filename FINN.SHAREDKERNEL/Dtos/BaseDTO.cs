using System.Text.Json;
using FINN.SHAREDKERNEL.Interfaces;

namespace FINN.SHAREDKERNEL.Dtos;

public abstract class BaseDto : IJson
{
    public string ToJson()
    {
        return JsonSerializer.Serialize((object)this);
    }
}