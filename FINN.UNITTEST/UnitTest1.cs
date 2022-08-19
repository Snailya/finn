using System.Text.Json;
using FINN.DRAFTER.Extensions;
using FINN.DRAFTER.Models;
using FINN.SHAREDKERNEL.Dtos;
using FINN.SHAREDKERNEL.Dtos.InsertBlock;
using FINN.SHAREDKERNEL.Dtos.UpdateJobStatus;
using FINN.SHAREDKERNEL.Models;
using netDxf;
using netDxf.Blocks;
using netDxf.Entities;
using netDxf.Tables;
using RabbitMQ.Client;

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
        var updateJobStatusDto = new UpdateJobStatusDto(0, "string");
        var serialized = JsonSerializer.Serialize(updateJobStatusDto);
        var deserialized = JsonSerializer.Deserialize<UpdateJobStatusDto>(serialized);
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
        var result = JsonSerializer.Deserialize<Response<InsertBlockResponseDto>>(str);
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