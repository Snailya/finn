using FINN.SHAREDKERNEL.Models;

namespace FINN.SHAREDKERNEL.Interfaces;

public interface ITransform
{
    void TransformedBy(Scale scale, Vector2d translate);
}