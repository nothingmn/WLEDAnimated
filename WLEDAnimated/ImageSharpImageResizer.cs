using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using WLEDAnimated.Interfaces;

namespace WLEDAnimated;

public class ImageSharpImageResizer : IImageResizer
{
    /// <summary>
    /// returns a byte array of the image resized to the specified dimensions
    /// </summary>
    /// <param name="path"></param>
    /// <param name="dimensions"></param>
    /// <returns></returns>
    public string ResizeImage(string path, Size dimensions)
    {
        if (dimensions.Height == 0 && dimensions.Width == 0) return path;

        var id = Guid.NewGuid().ToString();
        var file = new System.IO.FileInfo(path);
        if (!file.Exists) throw new FileNotFoundException("File not found", path);
        var resizedFile = $"{file.Name}-{id}{file.Extension}";
        using (Image image = Image.Load(File.ReadAllBytes(path)))
        {
            //Console.WriteLine($"Dimensions: {dimensions.Width}x{dimensions.Height}");
            //Console.WriteLine($"Image: {image.Width}x{image.Height}");

            var scaleWidthI = (int)dimensions.Width;
            var scaleHeightI = (int)dimensions.Height;

            //Console.WriteLine($"Final: {scaleWidthI}x{scaleHeightI}");
            image.Mutate(x => x.Resize(scaleWidthI, scaleHeightI));
            image.Save(resizedFile);
        }

        try
        {
            System.IO.File.Delete(path);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        return resizedFile;
    }
}