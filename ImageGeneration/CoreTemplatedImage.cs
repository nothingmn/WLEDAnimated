using CoreHtmlToImage;
using HandlebarsDotNet;
using Microsoft.Extensions.Logging;
using WLEDAnimated.Interfaces;

namespace ImageGeneration;

public class CoreTemplatedImage : IBasicTemplatedImage
{
    private readonly ILoggerFactory _loggerFactory;

    public CoreTemplatedImage(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public async Task<MemoryStream> GenerateImage(string template, dynamic data, int width)
    {
        var t = Handlebars.Compile(template);

        var result = t(data);

        var converter = new HtmlConverter();
        var bytes = converter.FromHtmlString(result, width * 10, ImageFormat.Png, 100);
        return new MemoryStream(bytes);
    }
}