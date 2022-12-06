using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using Slide = FINN.SLIDE.Data.Slide;

namespace FINN.SLIDE.Services;

public static class SlideService
{
    private const string TemplatePath = "./Resources/_blank.potx";

    /// <summary>
    ///     Split the presentation file into many individual presentation files.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static async Task<List<Slide>> SaveAsIndividuals(IFormFile file)
    {
        var slides = new List<Slide>();

        await using var stream = file.OpenReadStream();
        using var from = PresentationDocument.Open(stream, false);

        for (var i = 0; i < from.PresentationPart.Presentation.SlideIdList.Count(); i++)
        {
            var filePath =
                $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/.finn/slides/{GenerateFileName("pptx")}";

            // save as individual
            using var to = PresentationDocument.CreateFromTemplate(TemplatePath);
            if (to.PresentationPart.Presentation.SlideIdList == null)
                to.PresentationPart.Presentation.AddChild(new SlideIdList());
            OpenXmlWrapper.Copy(from, to, i);
            var saved = to.SaveAs(filePath);
            saved.Close();

            // generate thumbnail
            var image = FreeSpireWrapper.ConvertToBase64ImageFromPath(filePath);
            var slide = new Slide
            {
                Index = i,
                FilePath = filePath,
                Thumbnail = image
            };
            slides.Add(slide);
        }

        return slides;
    }

    /// <summary>
    ///     Combine slides into a new presentation file stream.
    /// </summary>
    /// <param name="slides"></param>
    /// <returns></returns>
    public static MemoryStream Merge(IEnumerable<Slide> slides)
    {
        using var to =
            PresentationDocument.CreateFromTemplate(TemplatePath);
        if (to.PresentationPart.Presentation.SlideIdList == null)
            to.PresentationPart.Presentation.AddChild(new SlideIdList());

        foreach (var slide in slides)
        {
            using var from = PresentationDocument.Open(slide.FilePath, false);
            OpenXmlWrapper.Copy(from, to, 0);
        }

        var stream = new MemoryStream();
        to.Clone(stream);
        stream.Seek(0, SeekOrigin.Begin);

        return stream;
    }

    /// <summary>
    ///     Generate a random file name with specified extensions.
    /// </summary>
    /// <param name="extension"></param>
    /// <returns></returns>
    public static string GenerateFileName(string? extension = null)
    {
        return string.IsNullOrEmpty(extension)
            ? Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
            : Path.ChangeExtension(Path.GetRandomFileName(), extension);
    }
}