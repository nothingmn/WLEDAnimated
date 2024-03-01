using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using WLEDAnimated.Interfaces;

namespace WLEDAnimated;

public class ImageUDPSender : IImageSender
{
    private readonly EndpointConverter _endpointConverter;
    private readonly ILogger<ImageUDPSender> _log;
    private readonly IImageConverter _converter;

    public ImageUDPSender(IImageToConverterFactory converterFactory, EndpointConverter endpointConverter, ILogger<ImageUDPSender> log)
    {
        _endpointConverter = endpointConverter;
        _log = log;
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

                        _log.LogInformation($"Segment Length:{segment.Length}, Frame Count:{frame.Count} bytes to {ipAddress}:{port}");
                        //foreach (var b in segment) Console.Write($"{b:X2} ");
                        Thread.Sleep(1); //let's not flood the network
                    }
                    _log.LogInformation($"Frame: Payload Count: {payload.Count} bytes to {ipAddress}:{port}");
                    Thread.Sleep(pauseBetweenFrames);
                }
                _log.LogInformation($"Iterations: Count: {iterations} bytes to {ipAddress}:{port}");
            }
        }
        _log.LogInformation($"Done sending image {path}");

        ////force GC to clean up memory, we are dealing with images here
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }
}