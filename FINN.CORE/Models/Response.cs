using System.Text.Json.Serialization;
using FINN.SHAREDKERNEL.UseCases;

namespace FINN.CORE.Models;

public class Response : JsonObject
{
    private int _code;

    public Response(string message, int code)
    {
        (Message, Code) = (message, code);
    }

    [JsonPropertyName("msg")]
    [JsonPropertyOrder(0)]
    public string Message { get; set; }

    [JsonPropertyName("code")]
    [JsonPropertyOrder(1)]
    public int Code
    {
        get => _code;
        set { _code = value; }
    }
}

public class Response<TData> : Response
{
    [JsonConstructor]
    public Response(string message, int code, TData data) : base(message, code)
    {
        Data = data;
    }

    [JsonPropertyName("data")]
    [JsonPropertyOrder(2)]
    public TData Data { get; set; }
}