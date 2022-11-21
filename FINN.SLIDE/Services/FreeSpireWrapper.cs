using System.Drawing;
using Spire.Presentation;

namespace FINN.SLIDE;

public static class FreeSpireWrapper
{
    /// <summary>
    ///     Convert a image into a base 64 string using Free.Spire. Notice that this package can only convert the first 3
    ///     slides into images if using the FREE version.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string ConvertToBase64ImageFromPath(string path)
    {
        var imagePath = Path.ChangeExtension(path, ".png");
        var presentation = new Presentation();
        presentation.LoadFromFile(path);

        var image = presentation.Slides[0].SaveAsImage(); // assume that only one slide per pptx
        var bytes = (byte[])new ImageConverter().ConvertTo(image, typeof(byte[]));
        var base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
        return "data:image/png;base64," + base64String;
    }
}