using SixLabors.ImageSharp;

namespace WLEDAnimated;

public interface IImageConverter
{
    List<byte[]> ConvertImage(string path, Size dimensions, int startIndex = 0, byte wait = 10);
}