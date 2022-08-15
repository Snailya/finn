using FINN.SHAREDKERNEL.Models;
using netDxf.Entities;

namespace FINN.DRAFTER.Model;

public class SimpleWrapper : DxfWrapper
{
    public SimpleWrapper(EntityObject entity) : base(entity.Layer, Vector2d.Zero)
    {
        AddEntity(entity);
    }
}