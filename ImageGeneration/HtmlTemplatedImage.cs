using HandlebarsDotNet;
using Microsoft.Extensions.Logging;
using NetCoreHTMLToImage;
using WLEDAnimated.Interfaces;

namespace ImageGeneration;

public class HtmlTemplatedImage : IBasicTemplatedImage
{
    private readonly ILoggerFactory _loggerFactory;

    public HtmlTemplatedImage(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public async Task<MemoryStream> GenerateImage(string template, dynamic data, int width)
    {
        var t = Handlebars.Compile(template);

        var result = t(data);
        var converter = new HtmlConverter();
        return new MemoryStream(converter.FromHtmlString(result, width * 10, ImageFormat.Png, 100));
    }
}