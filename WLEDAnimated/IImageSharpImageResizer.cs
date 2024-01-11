using SixLabors.ImageSharp;

namespace WLEDAnimated;

public interface IImageResizer
{
    /// <summary>
    /// returns a byte array of the image resized to the specified dimensions
    /// </summary>
    /// <param name="path"></param>
    /// <param name="dimensions"></param>
    /// <returns></returns>
    string ResizeImage(string path, Size dimensions);
}