using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
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

        var file = new System.IO.FileInfo(path);
        if (!file.Exists) throw new FileNotFoundException("File not found", path);
        var resizedFile = System.IO.Path.Combine($"{file.Name}-{dimensions.Height}x{dimensions.Width}{file.Extension}");

        if (!System.IO.File.Exists(resizedFile))
        {
            using (Image image = Image.Load(File.ReadAllBytes(path)))
            {
                var (scaledWidth, scaledHeight) =
                    GetScaledDimensions(image.Width, image.Height, dimensions.Width, dimensions.Height);

                if (dimensions.Width == image.Width && dimensions.Height == image.Height) return path;

                Console.WriteLine($"Dimensions: {dimensions.Width}x{dimensions.Height}");
                Console.WriteLine($"Image: {image.Width}x{image.Height}");

                Console.WriteLine($"Final: {scaledWidth}x{scaledHeight}");
                image.Mutate(x => x.Resize(scaledWidth, scaledHeight, new BoxResampler()));
                image.Save(resizedFile);
            }
        }

        return resizedFile;
    }

    public static (int, int) GetScaledDimensions(int originalWidth, int originalHeight, int maxWidth, int maxHeight)
    {
        double widthRatio = (double)maxWidth / originalWidth;
        double heightRatio = (double)maxHeight / originalHeight;
        double ratio = Math.Min(widthRatio, heightRatio);

        int newWidth = (int)(originalWidth * ratio);
        int newHeight = (int)(originalHeight * ratio);

        return (newWidth, newHeight);
    }

    // Usage example
}