namespace FINN.API.Dtos;

public class RequestLogDto
{
    public int Id { get; set; }
    public string? Status { get; set; }
    public string? Input { get; set; }
    public string? Output { get; set; }
    public string? Type { get; set; }
    public string? Created { get; set; }
}