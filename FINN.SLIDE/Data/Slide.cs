using FINN.CORE.Models;

namespace FINN.SLIDE;

public class Slide : BaseEntity
{
    /// <summary>
    ///     The order to sort the slide inside the topic.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    ///     The physical file path that stores the slide as a individual powerpoint file.
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    ///     The base64 string of the slide's image.
    /// </summary>
    public string Thumbnail { get; set; }
}