using System.Net.Sockets;
using SixLabors.ImageSharp;
using WLEDAnimated.Interfaces;

namespace WLEDAnimated;

public class ImageUDPSender : IImageSender
{
    private readonly EndpointConverter _endpointConverter;
    private readonly IImageConverter _converter;

    public ImageUDPSender(IImageToConverterFactory converterFactory, EndpointConverter endpointConverter)
    {
        _endpointConverter = endpointConverter;
        _converter = converterFactory.GetConverter();
    }

    public void Send(string ipAddress, int port, string path, Size dimensions, int startIndex = 0, byte wait = 1, int pauseBetweenFrames = 500, int iterations = 1)
    {
        var payload = _converter.ConvertImage(path, dimensions, startIndex, wait);
        var ipEndPoint = _endpointConverter.GetIPEndPoint(ipAddress, port);
        using (var udpClient = new UdpClient())
        {
            for (var x = 0; x < iterations; x++)
            {
                foreach (var frame in payload)
                {
                    foreach (var segment in frame)
                    {
                        //Console.WriteLine($"{DateTime.Now}");
                        udpClient.Send(segment, segment.Length, ipEndPoint);
                        Console.WriteLine($"{DateTime.Now} : Sent {segment.Length} bytes to {ipAddress}:{port}");
                        //foreach (var b in segment) Console.Write($"{b:X2} ");
                        Thread.Sleep(1);
                    }
                    Thread.Sleep(pauseBetweenFrames);
                }
            }
        }
        Console.WriteLine($"Done sending image {path}");
    }
}