using FINN.SHAREDKERNEL.Models;

namespace FINN.SHAREDKERNEL.Interfaces;

public interface IDrawer<T>
{
    List<T> Entities { get; }

    Vector2d Min { get; }
    Vector2d Max { get; }
}