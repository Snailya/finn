using FINN.SLIDE;
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
    options.AddDefaultPolicy(builder => builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors();

app.MapControllers();

app.MapGet("/topics/root",
    ([FromQuery] bool fast, SlideDb db) =>
        db.Topics.Where(x => x.Parent == null && x.IsFast == fast).Include(x => x.Topics)
            .Select(x => TopicDto.From(x)));

app.MapGet("/topics/{id:int}",
    (int id, SlideDb db) =>
        TopicDto.From(db.Topics.Include(x => x.Topics).Include(x => x.Slides).SingleOrDefault(x => x.Id == id)));

app.MapPost("/topics", ([FromBody] AddTopicRequestDto dto, SlideDb db) =>
{
    var topic = new Topic { Name = dto.Name, Parent = dto.ParentId != null ? db.Topics.Find(dto.ParentId) : null };
    db.Topics.Add(topic);
    db.SaveChanges();
    return topic;
});

app.MapGet("/topics/{id:int}/slides",
    (int id, SlideDb db) =>
        db.Topics.Include(x => x.Slides).SingleOrDefault(x => x.Id == id).Slides.Select(SlideDto.From));

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

app.MapGet("/topics/{id:int}/slides/download", (int id, SlideDb db) =>
{
    var slides = db.Topics.Include(x => x.Slides).SingleOrDefault(x => x.Id == id).Slides;
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

// Seed the database if in development.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    using var context = scope.ServiceProvider.GetRequiredService<SlideDb>();
    context.Database.EnsureCreated();

    context.Topics.AddRange(
        new Topic
        {
            Name = "1分钟",
            IsFast = true
        },
        new Topic
        {
            Name = "公司信息区",
            Topics = new List<Topic>
            {
                new() { Name = "基本信息" },
                new() { Name = "人力资源" },
                new() { Name = "财务状况" },
                new() { Name = "荣誉状况" }
            }
        },
        new Topic
        {
            Name = "涂装院信息区",
            Topics = new List<Topic>
            {
                new() { Name = "基本信息" },
                new() { Name = "人力资源" },
                new() { Name = "财务状况" },
                new() { Name = "客户信息" }
            }
        },
        new Topic { Name = "制造能力区" },
        new Topic { Name = "项目管理能力区" },
        new Topic { Name = "业绩文件区" },
        new Topic { Name = "专题核心技术区" },
        new Topic { Name = "专题对比区" }
    );

    context.SaveChanges();
}

app.Run();