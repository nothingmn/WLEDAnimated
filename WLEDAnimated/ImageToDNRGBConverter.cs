using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using WLEDAnimated.Interfaces;

namespace WLEDAnimated;

public class ImageToDNRGBConverter : IImageConverter
{
    private readonly IImageResizer _imageResizer;
    private const int MaxPayloadSize = 1000;

    public ImageToDNRGBConverter(IImageResizer imageResizer)
    {
        _imageResizer = imageResizer;
    }

    private List<byte[]> ConvertImageOrFrameToBytePayload(PixelAccessor<Rgba32> image, int startIndex, byte wait = 10)
    {
        var packets = new List<byte[]>();
        int maxPixelsPerPacket = (MaxPayloadSize - 4) / 3; // 4 bytes for header, 3 bytes per pixel
        int totalPixels = image.Width * image.Height;
        bool isLastPacket;

        for (int i = 0; i < totalPixels; i += maxPixelsPerPacket)
        {
            var frame = new List<byte>
            {
                4, wait,
                (byte)((startIndex >> 8) & 0xFF),
                (byte)(startIndex & 0xFF)
            };

            int endPixel = Math.Min(i + maxPixelsPerPacket, totalPixels);
            isLastPacket = endPixel == totalPixels;

            for (int pixelIndex = i; pixelIndex < endPixel; pixelIndex++)
            {
                int x = pixelIndex % image.Width;
                int y = pixelIndex / image.Width;
                Rgba32 pixel = image.GetRowSpan(y)[x];
                frame.AddRange(new byte[] { pixel.R, pixel.G, pixel.B });
            }

            // If this is the last packet and it's not full, pad with null bytes
            if (isLastPacket && frame.Count < MaxPayloadSize)
            {
                int paddingSize = MaxPayloadSize - frame.Count;
                frame.AddRange(new byte[paddingSize]);
            }

            packets.Add(frame.ToArray());
            startIndex += maxPixelsPerPacket;
        }

        return packets;
    }

    public List<List<byte[]>> ConvertImage(string path, Size dimensions, int startIndex = 0, byte wait = 10)
    {
        var resizedFile = _imageResizer.ResizeImage(path, dimensions);
        var allFramesPackets = new List<List<byte[]>>();

        using (var image = Image.Load<Rgba32>(resizedFile))
        {
            foreach (var frame in image.Frames.Cast<ImageFrame<Rgba32>>())
            {
                using (frame)
                {
                    frame.ProcessPixelRows(accessor =>
                    {
                        var packets = ConvertImageOrFrameToBytePayload(accessor, startIndex, wait);
                        allFramesPackets.Add(packets);
                    });
                }
            }
        }

        return allFramesPackets;
    }
}