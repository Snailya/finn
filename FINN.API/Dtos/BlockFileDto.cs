namespace FINN.API.Dtos;

public class BlockFileDto
{
    public IFormFile File { get; set; }
    public IEnumerable<string> BlockNames { get; set; } = new List<string>();
}