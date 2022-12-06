namespace FINN.SLIDE.Dtos;

public class AddTopicRequestDto
{
    /// <summary>
    ///     The name of the topic.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     The parent topic that this topic belongs to.
    /// </summary>
    public int? ParentId { get; set; }
}