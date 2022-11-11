using FINN.CORE.Models;

namespace FINN.SLIDE;

public class Slide : BaseEntity
{
    public int Id { get; set; }
    public int Index { get; set; }
    public string FilePath { get; set; }
    public string Thumbnail { get; set; }
}