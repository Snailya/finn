using System.Text.Json;
using FINN.SHAREDKERNEL.Interfaces;

namespace FINN.SHAREDKERNEL.UseCases;

public abstract class JsonObject : IJson
{
    public string ToJson()
    {
        return JsonSerializer.Serialize((object)this);
    }
}