using System.Text.Json.Serialization;

namespace FINN.CORE.Models;

public class PaginationFilter : JsonObject
{
    private int _pageNumber;
    private int _pageSize;

    public PaginationFilter()
    {
    }

    public PaginationFilter(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    [JsonPropertyName("pn")]
    public int PageNumber
    {
        get => _pageNumber;
        set
        {
            if (value < 0)
                throw new ArgumentException();
            _pageNumber = value;
        }
    }

    [JsonPropertyName("ps")]
    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (value < 0)
                throw new ArgumentException();
            _pageSize = value;
        }
    }

    public override string ToString()
    {
        return $"PageNumber: {PageNumber}, PageSize: {PageSize}";
    }
}