namespace WLEDAnimated.Interfaces;

public interface IBasicTemplatedImage
{
    Task<MemoryStream> GenerateImage(string template, dynamic data, int width);
}