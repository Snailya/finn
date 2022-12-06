using System.Text.Json.Serialization;
using FINN.CORE.Models;
using FINN.SLIDE.Data;

namespace FINN.SLIDE;

public sealed class Topic : BaseEntity
{
    private List<Topic> _topics = new();

    /// <summary>
    ///     The name of the topic.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     The parent topic of this topic.
    /// </summary>
    [JsonIgnore]
    public Topic? Parent { get; set; }

    /// <summary>
    ///     The children topic of this topic.
    /// </summary>
    public List<Topic> Topics
    {
        get => _topics;
        set
        {
            _topics = value;
            _topics.ForEach(x => x.Parent = this);
        }
    }

    /// <summary>
    ///     The slides belongs to this topic.
    /// </summary>
    public List<Slide> Slides { get; set; } = new();

    /// <summary>
    ///     Is this topic is for fast creation.
    /// </summary>
    public bool IsFast { get; set; }
}