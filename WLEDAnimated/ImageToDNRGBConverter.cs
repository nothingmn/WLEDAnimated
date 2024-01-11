using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using WLEDAnimated.Interfaces;

namespace WLEDAnimated;

public class ImageToDNRGBConverter : IImageConverter
{
    private readonly IImageResizer _imageResizer;

    public ImageToDNRGBConverter(IImageResizer imageResizer)
    {
        _imageResizer = imageResizer;
    }

    private byte[] ConvertImageOrFrameToBytePayload(PixelAccessor<Rgba32> image, int startIndex, byte wait = 10)
    {
        // Color is pixel-agnostic, but it's implicitly convertible to the Rgba32 pixel type
        var frame = new List<byte>();
        // Start index for LED (0 for the first LED)
        byte startIndexHigh = (byte)(startIndex >> 8);
        byte startIndexLow = (byte)(startIndex & 0xFF);
        frame.AddRange(new byte[] { 4, wait, startIndexHigh, startIndexLow });

        for (int y = 0; y < image.Height; y++)
        {
            Span<Rgba32> pixelRow = image.GetRowSpan(y);

            // pixelRow.Length has the same value as accessor.Width,
            // but using pixelRow.Length allows the JIT to optimize away bounds checks:
            for (int x = 0; x < image.Width; x++)
            {
                // Get a reference to the pixel at position x
                ref Rgba32 pixel = ref pixelRow[x];
                frame.AddRange(new byte[] { pixel.R, pixel.G, pixel.B });
            }
        }
        return frame.ToArray();
    }

    public List<byte[]> ConvertImage(string path, Size dimensions, int startIndex = 0, byte wait = 10)
    {
        var resizedFile = _imageResizer.ResizeImage(path, dimensions);
        var frames = new List<byte[]>();

        using (var image = Image.Load<Rgba32>(resizedFile))
        {
            if (image.Frames?.Count > 0)
            {
                foreach (var frame in image.Frames.Cast<ImageFrame<Rgba32>>())
                {
                    frame.ProcessPixelRows(accessor =>
                    {
                        frames.Add(ConvertImageOrFrameToBytePayload(accessor, startIndex, wait));
                    });
                }
            }
            else
            {
                image.ProcessPixelRows(accessor =>
                {
                    frames.Add(ConvertImageOrFrameToBytePayload(accessor, startIndex, wait));
                });
            }
        }

        try
        {
            File.Delete(resizedFile);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return frames;
    }
}