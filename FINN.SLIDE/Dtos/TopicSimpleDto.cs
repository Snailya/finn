namespace FINN.SLIDE.Dtos;

public class TopicSimpleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool HasChildren { get; set; }

    public static TopicSimpleDto From(Topic topic)
    {
        return new TopicSimpleDto()
        {
            Id = topic.Id,
            Name = topic.Name,
            HasChildren = topic.Topics.Count > 0
        };
    }
}