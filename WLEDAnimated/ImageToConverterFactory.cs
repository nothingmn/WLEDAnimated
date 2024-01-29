using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WLEDAnimated.Interfaces;

namespace WLEDAnimated;

public class ImageToConverterFactory : IImageToConverterFactory
{
    private readonly IConfiguration _config;
    private readonly IImageResizer _imageResizer;
    private readonly IServiceProvider _services;

    public ImageToConverterFactory(IConfiguration config, IImageResizer imageResizer, IServiceProvider services)
    {
        _config = config;
        _imageResizer = imageResizer;
        _services = services;
    }

    public IImageConverter GetConverter(string type = null)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            type = _config["WLED:ImageConverter"];
            if (string.IsNullOrWhiteSpace(type)) type = "TPM2NET";
        }

        return _services.GetKeyedService<IImageConverter>(type);
    }
}