namespace WLEDAnimated.Interfaces;

public interface IImageToConverterFactory
{
    IImageConverter GetConverter(string type = null);
}