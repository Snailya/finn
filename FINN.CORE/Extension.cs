using System.Text.Json;
using FINN.CORE.Models;

namespace FINN.CORE;

public static class Extension
{
    public static string ToJson<T>(this IEnumerable<T> enumerable) where T : JsonObject
    {
        return JsonSerializer.Serialize(enumerable);
    }
}