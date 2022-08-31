using System.Text.Json;
using FINN.CORE.Models;

namespace FINN.CORE;

public static class Extension
{
    /// <summary>
    ///     ToJson() support for <see cref="IEnumerable" />.
    /// </summary>
    /// <param name="enumerable"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string ToJson<T>(this IEnumerable<T> enumerable) where T : JsonObject
    {
        return JsonSerializer.Serialize(enumerable);
    }
}