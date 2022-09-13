using System.Text.Json;
using FINN.CORE.Models;

namespace FINN.CORE.Extensions;

public static class JsonObjectExtension
{
    /// <summary>
    ///     ToJson() support for <see cref="IEnumerable" />.
    /// </summary>
    public static string ToJson<T>(this IEnumerable<T> enumerable) where T : JsonObject
    {
        return JsonSerializer.Serialize(enumerable);
    }
}