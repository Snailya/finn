namespace FINN.API.Dtos;

public class UploadBlockFileDto
{
    public IFormFile File { get; set; }
    public IEnumerable<string> BlockNames { get; set; } = new List<string>();
}