using FINN.SLIDE;
using FINN.SLIDE.Data;
using FINN.SLIDE.Dtos;
using FINN.SLIDE.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<SlideDb>(opt => opt.UseInMemoryDatabase("SlideList"));

builder.Services.Configure<KestrelServerOptions>(options => options.Limits.MaxRequestBodySize = 134217728);
builder.Services.Configure<FormOptions>(options => options.MultipartBodyLengthLimit = 134217728);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policyBuilder => policyBuilder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod()));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors();

app.MapControllers();

app.MapGet("/topics/root",
    ([FromQuery] bool? fast, SlideDb db) =>
        db.Topics.Where(x => x.Parent == null).Include(x => x.Topics).Include(x => x.Slides)
            .Where(x => fast == null || x.IsFast == fast)
            .Select(x => TopicDto.From(x)));

app.MapPost("/topics", ([FromBody] AddTopicRequestDto dto, SlideDb db) =>
{
    var topic = new Topic
        { Name = dto.Name, Parent = dto.ParentId != null ? db.Topics.Find(dto.ParentId) : null };
    db.Topics.Add(topic);
    db.SaveChanges();
    return TopicDto.From(topic);
});

app.MapGet("/topics/{id:int}",
    (int id, SlideDb db) => db.Topics.Include(x => x.Parent).Include(x => x.Topics).Include(x => x.Slides)
        .SingleOrDefault(x => x.Id == id) is { } topic
        ? TopicDto.From(topic)
        : null
);

app.MapDelete("/topics/{id:int}", (int id, SlideDb db) =>
{
    var topic = db.Topics.Include(x => x.Slides).Include(x => x.Topics).SingleOrDefault(x => x.Id == id);
    if (topic == null) return Results.NotFound();

    db.Topics.Remove(topic);
    db.SaveChanges();

    return Results.NoContent();
});

app.MapGet("/topics/{id:int}/topics",
    (int id, SlideDb db) =>
        db.Topics.Include(x => x.Topics).ThenInclude(x => x.Topics).SingleOrDefault(x => x.Id == id) is { } topic
            ? topic.Topics.Select(TopicDto.From)
            : Array.Empty<TopicDto>());

app.MapGet("/topics/{id:int}/slides",
    (int id, SlideDb db) =>
        db.Topics.Include(x => x.Slides).SingleOrDefault(x => x.Id == id) is { } topic
            ? topic.Slides.Select(SlideDto.From)
            : Array.Empty<SlideDto>());

app.MapPost("/topics/{id:int}/slides", async (int id, IFormFile file, SlideDb db) =>
{
    var topic = db.Topics.Include(x => x.Slides).SingleOrDefault(x => x.Id == id);
    if (topic == null) throw new ArgumentException(nameof(id));

    var slides = await SlideService.SaveAsIndividuals(file);

    topic.Slides.AddRange(slides);
    db.Update(topic);
    db.SaveChanges();

    return slides.Select(SlideDto.From);
});

app.MapDelete("/topics/{id:int}/slides", (int id, SlideDb db) =>
{
    var topic = db.Topics.Include(x => x.Slides).Include(x => x.Topics).SingleOrDefault(x => x.Id == id);
    if (topic == null) return Results.NotFound();

    topic.Slides.Clear();
    db.Update(topic);
    db.SaveChanges();
    return Results.NoContent();
});

app.MapGet("/topics/{id:int}/slides/download", (int id, SlideDb db) =>
{
    var slides = db.Topics.Include(x => x.Slides).SingleOrDefault(x => x.Id == id) is { } topic
        ? topic.Slides
        : null;
    if (slides == null) return Results.NoContent();
    return Results.File(SlideService.Merge(slides),
        "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        SlideService.GenerateFileName("pptx")
    );
});


app.MapGet("/slides/{id:int}", (int id, SlideDb db) => SlideDto.From(db.Find<Slide>(id)));

app.MapGet("/slides/download", (int[] ids, SlideDb db) =>
{
    var slides = ids.Select(x => db.Slides.Find(x));
    return Results.File(SlideService.Merge(slides),
        "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        SlideService.GenerateFileName("pptx")
    );
});

using var scope = app.Services.CreateScope();
using var context = scope.ServiceProvider.GetRequiredService<SlideDb>();
context.Database.EnsureCreated();

if (app.Environment.IsDevelopment())
{
    // Seed the database if in development.
    context.Seed();

    // Ensure storage folder created
    if (!Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/.finn/slides"))
        Directory.CreateDirectory(
            $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/.finn/slides");
}

app.Run();