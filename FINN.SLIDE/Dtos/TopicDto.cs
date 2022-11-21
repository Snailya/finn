namespace FINN.SLIDE.Dtos;

public class TopicDto
{
    /// <summary>
    ///     The id of the topic.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     The name of the topic.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     The sub topics that belongs to this topic.
    /// </summary>
    public int[] Topics { get; set; }

    /// <summary>
    ///     The slides that belongs to this topic.
    /// </summary>
    public int[] Slides { get; set; }

    /// <summary>
    ///     Map a dto from <see cref="Topic" />.
    /// </summary>
    /// <param name="topic"></param>
    /// <returns></returns>
    public static TopicDto From(Topic topic)
    {
        return new TopicDto
        {
            Id = topic.Id,
            Name = topic.Name,
            Topics = topic.Topics.Select(x => x.Id).ToArray(),
            Slides = topic.Slides.Select(x => x.Id).ToArray()
        };
    }
}