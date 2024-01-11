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
            var widthOffset = image.Width - dimensions.Width;
            var heightOffset = image.Height - dimensions.Height;

            var scaleWidthI = (int)dimensions.Width;
            var scaleHeightI = (int)dimensions.Height;

            if (widthOffset > heightOffset)
            {
                var scaleRatio = (double)dimensions.Width / image.Width;
                scaleHeightI = (int)(image.Height * scaleRatio);
            }
            else
            {
                var scaleRatio = (double)dimensions.Height / image.Height;
                scaleWidthI = (int)(image.Width * scaleRatio);
            }

            Console.WriteLine($"Dimensions: {dimensions.Width}x{dimensions.Height}");
            Console.WriteLine($"Image: {image.Width}x{image.Height}");

            Console.WriteLine($"Final: {scaleWidthI}x{scaleHeightI}");
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