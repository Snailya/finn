using System.Text.Json.Serialization;

namespace FINN.CORE.Models;

public class Response : JsonObject
{
    public Response()
    {
    }

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
    public Response()
    {
    }

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

public class PagedResponse<TData> : Response<TData>
{
    public PagedResponse()
    {
    }

    public PagedResponse(string message, int code, TData data, PaginationFilter filter) : base(message, code, data)
    {
        PageNumber = filter.PageNumber;
        PageSize = filter.PageSize;
    }

    [JsonPropertyName("pageNumber")] public int PageNumber { get; set; }
    [JsonPropertyName("pageSize")] public int PageSize { get; set; }
}