using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Net.Sockets;

namespace WLEDAnimated;

public class ImageUDPSender : IImageSender
{
    private readonly IImageConverter _converter;

    public ImageUDPSender(IImageConverter converter = null)
    {
        if (converter == null) converter = new ImageToDNRGBConverter(new ImageSharpImageResizer());
        _converter = converter;
    }

    public void Send(string ipAddress, int port, string path, Size dimensions, int startIndex = 0, byte wait = 1, int pauseBetweenFrames = 500, int iterations = 1)
    {
        var payload = _converter.ConvertImage(path, dimensions, startIndex, wait);

        using (var udpClient = new UdpClient())
        {
            for (var x = 0; x < iterations; x++)
            {
                foreach (var frame in payload)
                {
                    udpClient.Send(frame, frame.Length, ipAddress, port);
                    foreach (var b in frame) Console.Write($"{b} ");
                    Console.WriteLine($"\nSent {frame.Length} bytes to {ipAddress}:{port}");
                    Thread.Sleep(pauseBetweenFrames);
                }
            }
        }
    }
}

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

        return frames;
    }
}

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
            Console.WriteLine($"Dimensions: {dimensions.Width}x{dimensions.Height}");
            Console.WriteLine($"Image: {image.Width}x{image.Height}");

            var scaleWidthI = (int)dimensions.Width;
            var scaleHeightI = (int)dimensions.Height;

            Console.WriteLine($"Final: {scaleWidthI}x{scaleHeightI}");
            image.Mutate(x => x.Resize(scaleWidthI, scaleHeightI));
            image.Save(resizedFile);
        }

        return resizedFile;
    }
}