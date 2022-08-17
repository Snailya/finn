using FINN.DRAFTER.Extensions;
using FINN.SHAREDKERNEL.Models;
using netDxf.Entities;
using netDxf.Tables;

namespace FINN.DRAFTER.Models;

public abstract class DxfWrapper : IEntityWrapper<EntityObject>
{
    private Vector2d _location = Vector2d.Zero;

    protected DxfWrapper(Layer layer, Vector2d location)
    {
        Layer = layer;
        Location = location;
    }

    public Layer Layer { get; }

    public Vector2d Location
    {
        get => _location;
        set
        {
            Entities.ToList().ForEach(x => x.TransformBy(Scale.Identity, value - _location));
            Box.TransformBy(Scale.Identity, value - _location);
            _location = value;
        }
    }

    public IList<EntityObject> Entities { get; } = new List<EntityObject>();
    public BoundingBox Box { get; } = new();
    public BoundingBox OuterBox { get; } = new();

    public void TransformedBy(Scale scale, Vector2d translate)
    {
        Entities.ToList().ForEach(x =>
            x.TransformBy(scale, translate));
        Box.TransformBy(scale, translate);
    }

    protected void AddEntity(EntityObject entity, bool asInside = true)
    {
        Entities.Add(entity);
        if (asInside) Box.AddEntity(entity);

        OuterBox.AddEntity(entity);
    }
}