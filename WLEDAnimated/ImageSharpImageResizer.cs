using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using WLEDAnimated.Interfaces;

namespace WLEDAnimated;

public class ImageSharpImageResizer : IImageResizer
{
    private readonly IConfiguration _configuration;
    private bool _preferCenteredCrop = true;
    private string _resampler = "NearestNeighborResampler";

    public ImageSharpImageResizer(IConfiguration configuration)
    {
        _configuration = configuration;

        var resizer = _configuration.GetSection("ImageResizer");
        if (resizer.Exists())
        {
            if (resizer["PreferCenteredCrop"] != null)
            {
                bool.TryParse(resizer["PreferCenteredCrop"], out _preferCenteredCrop);
            }
            if (resizer["Resampler"] != null)
            {
                _resampler = resizer["Resampler"];
            }
        }
    }

    public string ResizeImage(string path, Size dimensions)
    {
        if (dimensions.Height == 0 && dimensions.Width == 0) return path;

        var file = new FileInfo(path);
        if (!file.Exists) throw new FileNotFoundException("File not found", path);
        var resizedFile = Path.Combine(file.DirectoryName, $"{Path.GetFileNameWithoutExtension(file.Name)}-{dimensions.Height}x{dimensions.Width}{file.Extension}");

        if (!File.Exists(resizedFile))
        {
            using (var image = Image.Load(path))
            {
                IResampler resampler = GetResamplerByName(_resampler);

                var (scaledWidth, scaledHeight) = CalculateNewDimensions(dimensions.Width, dimensions.Height, image.Width, image.Height);

                // Resize image to fit within the target dimensions while maintaining aspect ratio
                var resizedImage = image.Clone(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(scaledWidth, scaledHeight),
                    Mode = _preferCenteredCrop ? ResizeMode.Crop : ResizeMode.Min,
                    Sampler = resampler,
                    Position = _preferCenteredCrop ? AnchorPositionMode.Center : AnchorPositionMode.Top // Ensures cropping or padding is centered
                }));

                // Create a new image with the target dimensions, filled with a default background color
                using (var resultImage = new Image<Rgba32>(dimensions.Width, dimensions.Height))
                {
                    // Calculate the position to center the resized (and possibly cropped) image
                    int posX = (dimensions.Width - resizedImage.Width) / 2;
                    int posY = (dimensions.Height - resizedImage.Height) / 2;

                    // Draw the resized (and possibly cropped) image onto the new image at the calculated position
                    resultImage.Mutate(x => x.DrawImage(resizedImage, new Point(posX, posY), 1f));

                    // Save the result image
                    resultImage.Save(resizedFile);
                }
            }
        }
        return resizedFile;
    }

    private IResampler GetResamplerByName(string resamplerName)
    {
        var resamplers = new Dictionary<string, IResampler>
        {
            {"NearestNeighbor", KnownResamplers.NearestNeighbor},
            {"Bicubic", KnownResamplers.Bicubic},
            {"Box", KnownResamplers.Box},
            {"Lanczos3", KnownResamplers.Lanczos3},
            {"Lanczos5", KnownResamplers.Lanczos5},
            {"Lanczos8", KnownResamplers.Lanczos8},
            {"MitchellNetravali", KnownResamplers.MitchellNetravali},
            {"Triangle", KnownResamplers.Triangle},
            // Add other resamplers as needed
        };

        if (resamplers.TryGetValue(resamplerName, out var resampler))
        {
            return resampler;
        }
        else
        {
            throw new ArgumentException($"Unsupported resampler: {resamplerName}");
        }
    }

    public (int, int) CalculateNewDimensions(int screenWidth, int screenHeight, int imageWidth, int imageHeight)
    {
        float widthScale = (float)screenWidth / imageWidth;
        float heightScale = (float)screenHeight / imageHeight;
        float scale = Math.Min(widthScale, heightScale);

        int newWidth = (int)(imageWidth * scale);
        int newHeight = (int)(imageHeight * scale);

        return (newWidth, newHeight);
    }

    private Rectangle CalculateCropDimensions(int imageWidth, int imageHeight, int targetWidth, int targetHeight)
    {
        float imageAspect = (float)imageWidth / imageHeight;
        float targetAspect = (float)targetWidth / targetHeight;

        int cropWidth, cropHeight;
        if (imageAspect > targetAspect)
        {
            // Image is wider than target
            cropHeight = imageHeight;
            cropWidth = (int)(imageHeight * targetAspect);
        }
        else
        {
            // Image is taller than target
            cropWidth = imageWidth;
            cropHeight = (int)(imageWidth / targetAspect);
        }

        int cropX = (imageWidth - cropWidth) / 2;
        int cropY = (imageHeight - cropHeight) / 2;

        return new Rectangle(cropX, cropY, cropWidth, cropHeight);
    }
}