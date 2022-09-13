using FINN.DXF.Geometries;

namespace FINN.DXF.Interfaces;

public interface ITransform
{
    void TransformedBy(TransformScale transformScale, Vector2d translate);
}