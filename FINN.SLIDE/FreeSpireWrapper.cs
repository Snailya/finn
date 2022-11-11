using System.Drawing;
using Spire.Presentation;

namespace FINN.SLIDE;

public static class FreeSpireWrapper
{
    public static string ConvertToBase64ImageFromPath(string path)
    {
        var imagePath = Path.ChangeExtension(path, ".png");
        var presentation = new Presentation();
        presentation.LoadFromFile(path);

        var image = presentation.Slides[0].SaveAsImage(); // assume that only one slide per pptx
        var bytes = (byte[])(new ImageConverter()).ConvertTo(image, typeof(byte[]));
        var base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
        return "data:image/png;base64," + base64String;
    }
}