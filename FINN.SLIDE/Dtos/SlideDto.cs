using FINN.SLIDE.Data;

namespace FINN.SLIDE.Dtos;

public class SlideDto
{
    /// <summary>
    ///     The id of the slide.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     The base 64 string that represents the thumbnail of the presentation's slide.
    /// </summary>
    public string Image { get; set; } = string.Empty;

    /// <summary>
    ///     Map a dto from <see cref="Slide" />.
    /// </summary>
    /// <param name="slide"></param>
    /// <returns></returns>
    public static SlideDto From(Slide slide)
    {
        return new SlideDto { Id = slide.Id, Image = slide.Thumbnail };
    }
}