using System.Text.Json.Serialization;
using FINN.CORE.Models;

namespace FINN.SLIDE;

public sealed class Topic : BaseEntity
{
    /// <summary>
    ///     The name of the topic.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     The parent topic of this topic.
    /// </summary>
    [JsonIgnore]
    public Topic? Parent { get; set; }

    /// <summary>
    ///     The children topic of this topic.
    /// </summary>
    public List<Topic> Topics { get; set; } = new();

    /// <summary>
    ///     The slides belongs to this topic.
    /// </summary>
    public List<Slide> Slides { get; set; } = new();

    /// <summary>
    ///     Is this topic is for fast creation.
    /// </summary>
    public bool IsFast { get; set; }
}