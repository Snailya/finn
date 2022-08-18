using FINN.SHAREDKERNEL.Models;
using netDxf.Collections;
using netDxf.Entities;
using netDxf.Tables;

namespace FINN.DRAFTER.Models;

public class SimpleWrapper : DxfWrapper
{
    public SimpleWrapper(params EntityObject[] entities) : base(Layer.Default, Vector2d.Zero)
    {
        foreach (var entity in entities)
        {
            AddEntity(entity);
        }
    }
}