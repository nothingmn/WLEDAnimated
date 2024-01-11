using SixLabors.ImageSharp;

namespace WLEDAnimated;

public interface IImageSender
{
    void Send(string ipAddress, int port, string path, Size dimensions, int startIndex = 0, byte wait = 10, int pauseBetweenFrames = 100, int iterations = 1);
}