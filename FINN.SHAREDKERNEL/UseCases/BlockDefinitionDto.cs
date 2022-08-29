namespace FINN.SHAREDKERNEL.UseCases;

public class BlockDefinitionDto : JsonObject
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Filename { get; set; }
}