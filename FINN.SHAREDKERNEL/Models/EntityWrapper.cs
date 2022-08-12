using FINN.SHAREDKERNEL.Interfaces;

namespace FINN.SHAREDKERNEL.Models;

public interface IEntityWrapper<T> : ITransform
{
    Vector2d Location { get; set; }
    IList<T> Entities { get; }

    BoundingBox Box { get; }
    BoundingBox OuterBox { get; }
}