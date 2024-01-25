using Microsoft.Extensions.Configuration;
using WLEDAnimated.Interfaces;

namespace WLEDAnimated;

public class ImageToConverterFactory : IImageToConverterFactory
{
    private readonly IConfiguration _config;
    private readonly IImageResizer _imageResizer;

    public ImageToConverterFactory(IConfiguration config, IImageResizer imageResizer)
    {
        _config = config;
        _imageResizer = imageResizer;
    }

    public IImageConverter GetConverter(string type = null)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            type = _config["WLED:ImageConverter"];
            if (string.IsNullOrWhiteSpace(type)) type = "DNRGB";
        }

        return type switch
        {
            "TPM2.NET" => new ImageToTPM2NETConverter(_imageResizer),
            "DNRGB" => new ImageToDNRGBConverter(_imageResizer),
            _ => new ImageToDNRGBConverter(_imageResizer)
        };
    }
}