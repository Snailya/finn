using System.Text.Json;
using FINN.CORE.Interfaces;

namespace FINN.CORE.Models;

public abstract class JsonObject : IJson
{
    public string ToJson()
    {
        return JsonSerializer.Serialize((object)this);
    }
}