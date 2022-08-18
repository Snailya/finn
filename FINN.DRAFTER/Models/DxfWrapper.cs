using FINN.DRAFTER.Extensions;
using FINN.SHAREDKERNEL.Models;
using netDxf.Entities;
using netDxf.Tables;

namespace FINN.DRAFTER.Models;

public abstract class DxfWrapper : IEntityWrapper<EntityObject>
{
    private Vector2d _basePoint;

    /// <summary>
    /// The delegate to call when BasePoint's value change, can be overwrite in group class so that the item inside group could update correctly.
    /// </summary>
    protected Action<Vector2d> OnBasePointChanged;

    #region Constructors

    protected DxfWrapper(Layer layer, Vector2d basePoint)
    {
        Layer = layer;
        _basePoint = basePoint;

        OnBasePointChanged = value => { TransformedBy(new Scale(1), value - _basePoint); };
    }

    #endregion

    /// <summary>
    /// The dxf layer that should place the wrapper on.
    /// </summary>
    public Layer Layer { get; }

    /// <summary>
    /// The geometric base point of the wrapper.
    /// </summary>
    public Vector2d BasePoint
    {
        get => _basePoint;
        set
        {
            OnBasePointChanged(value);
            _basePoint = value;
        }
    }

    public virtual IList<EntityObject> Entities { get; } = new List<EntityObject>();

    /// <summary>
    /// The valid bounding box of the wrapper.
    /// </summary>
    public BoundingBox Box { get; } = new();

    /// <summary>
    /// The actual bounding box of the wrapper.
    /// </summary>
    public BoundingBox OuterBox { get; } = new();

    public void TransformedBy(Scale scale, Vector2d translate)
    {
        // move entities to right place
        Entities.ToList().ForEach(x => x.TransformBy(scale, translate));

        // transform inner bounding to match entities
        Box.TransformBy(scale, translate);
        // transform outer bounding to match entities
        OuterBox.TransformBy(scale, translate);

        // reset base point
        _basePoint.TransformBy(scale, translate);
    }

    protected void AddEntity(EntityObject entity, bool asInside = true)
    {
        Entities.Add(entity);

        // only add marked entity to valid box
        if (asInside) Box.AddEntity(entity);

        // add all to actual box.
        OuterBox.AddEntity(entity);
    }
}