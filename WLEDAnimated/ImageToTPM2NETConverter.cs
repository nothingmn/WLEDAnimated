using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;
using TPM2;
using WLEDAnimated.Interfaces;

namespace WLEDAnimated;

public class ImageToTPM2NETConverter : IImageConverter
{
    private readonly IImageResizer _imageResizer;

    public ImageToTPM2NETConverter(IImageResizer imageResizer)
    {
        _imageResizer = imageResizer;
    }

    private List<byte[]> ConvertImageOrFrameToBytePayload(PixelAccessor<Rgba32> image, int startIndex, byte wait = 10)
    {
        var client = new Tpm2UdpClient();
        var ledStrip = new LEDStrip(image.Height * image.Width);

        int index = 0;
        for (int y = 0; y < image.Height; y++)
        {
            var pixelRow = image.GetRowSpan(y);

            // pixelRow.Length has the same value as accessor.Width,
            // but using pixelRow.Length allows the JIT to optimize away bounds checks:
            for (int x = 0; x < image.Width; x++)
            {
                // Get a reference to the pixel at position x
                ref var pixel = ref pixelRow[x];
                ledStrip.SetLED(index, new LED(pixel.R, pixel.G, pixel.B));
                index++;
            }
        }

        return client.ConstructPayload(ledStrip);
    }

    public List<List<byte[]>> ConvertImage(string path, Size dimensions, int startIndex = 0, byte wait = 10)
    {
        var resizedFile = _imageResizer.ResizeImage(path, dimensions);
        var frames = new List<List<byte[]>>();

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

        return frames;
    }
}