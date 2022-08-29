using System.Text.Json;
using FINN.CORE.Models;
using FINN.PLUGINS.DXF;
using FINN.PLUGINS.DXF.Models;
using FINN.PLUGINS.DXF.Utils;
using FINN.SHAREDKERNEL.Dtos.Jobs;
using FINN.SHAREDKERNEL.Models;
using netDxf;
using netDxf.Blocks;
using netDxf.Entities;
using netDxf.Tables;

namespace FINN.UNITTEST;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void JsonSerializerDeserializeEnum_ReturnsAsSameAsBeforeSerialize()
    {
        var updateJobStatusDto = new UpdateJobStatusRequestDto(0, "string");
        var serialized = JsonSerializer.Serialize(updateJobStatusDto);
        var deserialized = JsonSerializer.Deserialize<UpdateJobStatusRequestDto>(serialized);
        Assert.That(deserialized, Is.Not.Null);
        Assert.That(deserialized!.Status, Is.EqualTo(JobStatus.Ready));
    }

    [Test]
    public void TransformExtensionLogicCorrectly()
    {
        var point11 = new Vector2d(1, 1);
        // zoom 2x based on (0,0)
        point11.TransformBy(new Scale(2), Vector2d.Zero);
        Assert.Multiple(() =>
        {
            Assert.That(point11.X, Is.EqualTo(2));
            Assert.That(point11.Y, Is.EqualTo(2));
        });
        var point00 = new Vector2d(0, 0);
        // zoom 2x based on (1,1)
        point00.TransformBy(new Scale(new Vector2d(1, 1), 2), Vector2d.Zero);
        Assert.Multiple(() =>
        {
            Assert.That(point00.X, Is.EqualTo(-1));
            Assert.That(point00.Y, Is.EqualTo(-1));
        });
        var point22 = new Vector2d(2, 2);
        // zoom 2x based on (0,0) and move (2,2)
        point22.TransformBy(new Scale(new Vector2d(0, 0), 2), new Vector2d(2, 2));
        Assert.That(point22.X, Is.EqualTo(6));
        Assert.That(point22.Y, Is.EqualTo(6));

        var line = new Line(new Vector2(0, 0), new Vector2(1, 1));
        // zoom 2x based on (1,1) and move (1,0)
        var scale = new Scale(new Vector2d(1, 1), 2);
        var translate = new Vector2d(1, 0);
        line.TransformBy(scale, translate);
        Assert.Multiple(() =>
        {
            Assert.That(line.StartPoint.ToVector2d(), Is.EqualTo(new Vector2d(0, -1)));
            Assert.That(line.EndPoint.ToVector2d(), Is.EqualTo(new Vector2d(2, 1)));
        });
    }

    [Test]
    public void DeserializeResponseCorrectly()
    {
        var str = "{\"msg\":\"success\",\"code\":0,\"data\":{\"Ids\":[4,3]}}";
        var result = JsonSerializer.Deserialize<Response<UploadOrUpdateBlocksReqsponse>>(str);
        Assert.That(result.Data.Blocks, Is.Not.Null);
    }

    [Test]
    public void GroupLocationChangeWillUpdateItemLocation()
    {
        const int gutter = 100;

        var points = new Group(Vector2d.Zero, GroupDirection.LeftToRight, GroupAlignment.Middle, gutter);
        var point1 = new SimpleWrapper(new Point(Vector2.Zero));
        var point2 = new SimpleWrapper(new Point(Vector2.Zero));

        points.Add(point1);
        Assert.Multiple(() =>
        {
            Assert.That(point1.BasePoint.X, Is.EqualTo(0));
            Assert.That(point1.BasePoint.Y, Is.EqualTo(0));
        });
        points.Add(point2);
        Assert.Multiple(() =>
        {
            Assert.That(point2.BasePoint.X, Is.EqualTo(100));
            Assert.That(point2.BasePoint.Y, Is.EqualTo(0));
        });
        points.BasePoint = new Vector2d(0, 100);
        Assert.Multiple(() =>
        {
            Assert.That(point1.BasePoint.Y, Is.EqualTo(100));
            Assert.That(point2.BasePoint.Y, Is.EqualTo(100));
        });

        var outer = new Group(Vector2d.Zero, GroupDirection.TopToBottom, GroupAlignment.Middle, gutter);
        outer.Add(points);
        Assert.Multiple(() =>
        {
            Assert.That(points.BasePoint.X, Is.EqualTo(-50));
            Assert.That(points.BasePoint.Y, Is.EqualTo(0));
        });
        var circle = new SimpleWrapper(new Circle(Vector2.Zero, 50));
        outer.Add(circle);
        Assert.Multiple(() =>
        {
            Assert.That(circle.BasePoint.X, Is.EqualTo(0));
            Assert.That(circle.BasePoint.Y, Is.EqualTo(-150));
        });
    }

    [Test]
    public void OuterBoundingBox()
    {
        var item1 = new TestWrapper();
        var item2 = new TestWrapper();

        var group = new Group(Vector2d.Zero, GroupDirection.LeftToRight, GroupAlignment.Middle, 100);
        group.Add(item1);
        group.Add(item2);

        Assert.Multiple(() => { Assert.That(item2.BasePoint.X, Is.EqualTo(100)); });
    }

    [Test]
    public void InnerBlockCouldExplodedInOneMethodCall()
    {
        var inner = new Block("inner", new EntityObject[] { new Circle(Vector2.Zero, 10) });
        var middle = new Block("middle",
            new EntityObject[] { new Insert(inner, Vector2.Zero), new Point(new Vector2(10, 10)) });
        var outer = new Block("outer", new[] { new Insert(middle, Vector2.Zero) });

        var entities = outer.ExplodeIteratively().ToList();

        entities.ForEach(x => Assert.That(x, Is.Not.InstanceOf(typeof(Insert))));
    }

    [Test]
    public void GetAreaFromPolyLine()
    {
        var polyline =
            new Polyline(
                new List<Vector3> { Vector3.Zero, new(10, 0, 0), new(10, 5, 0), new(0, 5, 0) }, true);


        var minX = polyline.Vertexes.MinBy(vertex => vertex.Position.X)!.Position.X;
        var minY = polyline.Vertexes.MinBy(vertex => vertex.Position.Y)!.Position.Y;
        var maxX = polyline.Vertexes.MaxBy(vertex => vertex.Position.X)!.Position.X;
        var maxY = polyline.Vertexes.MaxBy(vertex => vertex.Position.Y)!.Position.Y;

        Assert.That(maxY, Is.EqualTo(5));
        Assert.That(maxX, Is.EqualTo(10));
    }


    [Test]
    public void CreateAssociativeHatchTest()
    {
        var polyline =
            new LwPolyline(
                new List<Vector2>
                {
                    Vector2.Zero, new(10, 0), new(10, 5),
                    new(0, 5)
                }, true) { Color = AciColor.Red };


        var boundary = new HatchBoundaryPath(new List<EntityObject> { polyline });
        var hatch = new Hatch(HatchPattern.Solid, new[] { boundary }, true) { Color = AciColor.Blue };
        polyline.TransformBy(Matrix3.Identity, new Vector3(0, 50, 0));


        var polyline2 = EntityUtil.CreateLwPolyline(true, new Vector2d(0, 10), new Vector2d(10, 10),
            new Vector2d(10, 15),
            new Vector2d(0, 20));
        var boundary2 = new HatchBoundaryPath(new List<EntityObject> { polyline2 });
        var hatch2 = new Hatch(HatchPattern.Solid, new[] { boundary2 }, true) { Color = AciColor.Red };


        var dxf = new DxfDocument();
        dxf.AddEntity(hatch);
        dxf.AddEntity(hatch2);
        var filename = Path.GetRandomFileName() + ".dxf";
        dxf.Save(filename);
    }

    [Test]
    public void ScaleInsertScalesTheTextHeightOfAttribute()
    {
        var innerBlock = new Block("_inner", new[] { new Circle(Vector2.Zero, 1) }, new AttributeDefinition[]
        {
            new("X", 1, TextStyle.Default) { IsVisible = true, Value = "X", Height = 1 }
        });
        var innerInsert = new Insert(innerBlock, Vector2.Zero) { Scale = new Vector3(100, 100, 100) };
        innerInsert.TransformAttributes();

        Assert.That(innerInsert.Attributes.AttributeWithTag("X").Height, Is.EqualTo(100));
    }

    [Test]
    public void Dimension()
    {
        var line = new Line(new Vector2(0, 10), new Vector2(1000, 10));
        var ad = DimUtil.CreateAlignedDim(line, 1);

        var block = new Block("DimensionBlock", new EntityObject[] { line, ad }) { Origin = Vector3.Zero };
        var insert = new Insert(block) { Position = Vector3.Zero };
        insert.TransformAttributes();

        var dxf = new DxfDocument();
        dxf.AddEntity(insert);

        dxf.Save(Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".dxf");
    }

    [Test]
    public void SaveBinaryTest()
    {
        var path = @"C:\Users\snailya\Desktop\tmp792F.dxf";

        var loaded = DxfDocument.Load(path);
        if (loaded != null)
        {
            Assert.Pass();
        }
    }

    public class TestWrapper : DxfWrapper
    {
        public TestWrapper() : base(Layer.Default, Vector2d.Zero)
        {
            var point = new Point(Vector2.Zero);
            var line = new Line(new Vector2(-50, 0), new Vector2(50, 0));
            AddEntity(point);
            AddEntity(line, false);
        }
    }
}