using System.Drawing;
using System.Drawing.Text;
using System.Net.Mime;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using FINN.SLIDE;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

var contextFolder = "../data/";

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

app.MapPost("/upload", async (IFormFile file, SlideDb db) =>
{
    await using var stream = file.OpenReadStream();
    using var from = PresentationDocument.Open(stream, false);

    for (var i = 0; i < from.PresentationPart.Presentation.SlideIdList.Count(); i++)
    {
        var filePath = $"{contextFolder}{i}.pptx";

        // save as individual
        using var to =
            PresentationDocument.CreateFromTemplate(
                @"C:\Users\snailya\Documents\Custom Office Templates\EmptySlide.potx");
        if (to.PresentationPart.Presentation.SlideIdList == null)
            to.PresentationPart.Presentation.AddChild(new SlideIdList());
        OpenXmlWrapper.Copy(from, to, i);
        var saved = to.SaveAs(filePath);
        saved.Close();

        // generate thumbnail
        var image = FreeSpireWrapper.ConvertToBase64ImageFromPath(filePath);
        var slide = new FINN.SLIDE.Slide
        {
            Index = i,
            FilePath = filePath,
            Thumbnail = image
        };
        db.Slides.Add(slide);
    }

    db.SaveChanges();
});

app.MapGet("/thumbnails", (SlideDb db) => Results.Ok(db.Slides.OrderBy(x => x.Index).Select(x => new ThumbnailDto()
{
    Id = x.Id,
    Image = x.Thumbnail,
})));

app.MapGet("/merge", (int[] pages, SlideDb db) =>
{
    using var to =
        PresentationDocument.CreateFromTemplate(
            @"C:\Users\snailya\Documents\Custom Office Templates\EmptySlide.potx");
    if (to.PresentationPart.Presentation.SlideIdList == null)
        to.PresentationPart.Presentation.AddChild(new SlideIdList());

    foreach (var page in pages)
    {
        // find by id
        var filePath = db.Slides.FirstOrDefault(x => x.Id == page).FilePath;
        using var from = PresentationDocument.Open(filePath, false);
        OpenXmlWrapper.Copy(from, to, 0);
    }

    var stream = new MemoryStream();
    to.Clone(stream);
    stream.Seek(0, SeekOrigin.Begin);

    return Results.File(stream,
        contentType: "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        fileDownloadName: Path.ChangeExtension(Path.GetFileNameWithoutExtension(Path.GetTempFileName()), "pptx")
    );
});

app.Run();