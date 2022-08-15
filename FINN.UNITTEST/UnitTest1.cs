using System.Text.Json;
using FINN.DRAFTER.Extensions;
using FINN.SHAREDKERNEL.Dtos;
using FINN.SHAREDKERNEL.Models;
using netDxf;
using netDxf.Entities;

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
}