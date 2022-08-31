using System.Text.Json.Serialization;

namespace FINN.CORE.Models;

public class Response : JsonObject
{
    public Response(string message, int code)
    {
        (Message, Code) = (message, code);
    }

    /// <summary>
    ///     The error message of the response
    /// </summary>
    [JsonPropertyName("msg")]
    [JsonPropertyOrder(0)]
    public string Message { get; set; }

    /// <summary>
    ///     The state code of the response. 0 for success.
    /// </summary>
    [JsonPropertyName("code")]
    [JsonPropertyOrder(1)]
    public int Code { get; set; }
}

public class Response<TData> : Response
{
    [JsonConstructor]
    public Response(string message, int code, TData data) : base(message, code)
    {
        Data = data;
    }

    /// <summary>
    ///     Data.
    /// </summary>
    [JsonPropertyName("data")]
    [JsonPropertyOrder(2)]
    public TData Data { get; set; }
}