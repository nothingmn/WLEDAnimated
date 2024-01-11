using System.Net.Sockets;
using SixLabors.ImageSharp;
using WLEDAnimated.Interfaces;

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
                    //foreach (var b in frame) Console.Write($"{b} ");
                    //Console.WriteLine($"\nSent {frame.Length} bytes to {ipAddress}:{port}");
                    Thread.Sleep(pauseBetweenFrames);
                }
            }
        }
    }
}