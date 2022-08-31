using FINN.SHAREDKERNEL.Interfaces;

namespace FINN.SHAREDKERNEL.Models;

/// <inheritdoc />
public interface IEntityWrapper<T> : ITransform
{
    /// <summary>
    ///     The base point (insert point) of the wrapper.
    /// </summary>
    Vector2d BasePoint { get; set; }

    /// <summary>
    ///     The entities that are included in the wrapper.
    /// </summary>
    IList<T> Entities { get; }

    /// <summary>
    ///     The valid bounding box of the wrapper, which is treat as has volume when compared with other wrapper.
    /// </summary>
    BoundingBox Box { get; }

    /// <summary>
    ///     The actual bounding box of the wrapper, which is the sum of all entities' bounding box.
    /// </summary>
    BoundingBox OuterBox { get; }
}