using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
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

        var root = System.IO.Directory.GetCurrentDirectory();
        if (!System.IO.File.Exists(resizedFile))
        {
            using (var image = Image.Load(File.ReadAllBytes(path)))
            {
                var (scaledWidth, scaledHeight) =
                    CalculateNewDimensions(dimensions.Width, dimensions.Height, image.Width, image.Height);

                Console.WriteLine($"Dimensions: {dimensions.Width}x{dimensions.Height}");
                Console.WriteLine($"Image: {image.Width}x{image.Height}");
                Console.WriteLine($"Final: {scaledWidth}x{scaledHeight}");

                if (image.Height != dimensions.Height || image.Width != dimensions.Width)
                {
                    Console.WriteLine($"Image will be scaled to {scaledWidth}x{scaledHeight}");
                    image.Mutate(x => x.Resize(scaledWidth, scaledHeight, new TriangleResampler()));
                }
                else
                {
                    Console.WriteLine($"Final: {scaledWidth}x{scaledHeight}");
                }

                image.Save(resizedFile);
            }
        }

        return resizedFile;
    }

    public (int, int) CalculateNewDimensions(int screenWidth, int screenHeight, int imageWidth, int imageHeight)
    {
        // Calculate the scaling factors for width and height
        float widthScale = (float)screenWidth / imageWidth;
        float heightScale = (float)screenHeight / imageHeight;

        // Use the smaller scaling factor to ensure the image fits within the screen dimensions
        float scale = Math.Min(widthScale, heightScale);

        // Calculate the new dimensions
        int newWidth = (int)(imageWidth * scale);
        int newHeight = (int)(imageHeight * scale);

        // Ensure that the new dimensions do not exceed the screen dimensions
        newWidth = Math.Min(newWidth, screenWidth);
        newHeight = Math.Min(newHeight, screenHeight);

        return (newWidth, newHeight);
    }
}