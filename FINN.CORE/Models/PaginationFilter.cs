using System.Text.Json.Serialization;

namespace FINN.CORE.Models;

public class PaginationFilter : JsonObject
{
    [JsonPropertyName("pn")] public int PageNumber { get; set; }

    [JsonPropertyName("ps")] public int PageSize { get; set; }

    public PaginationFilter()
    {
        PageNumber = 1;
        PageSize = 10;
    }

    public PaginationFilter(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize > 10 ? 10 : pageSize;
    }
}