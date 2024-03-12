using Microsoft.Extensions.Logging;
using NetCoreHTMLToImage;
using WLEDAnimated.Interfaces;
using WLEDAnimated.Interfaces.Services;

namespace ImageGeneration;

public class HtmlTemplatedImage : IBasicTemplatedImage
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ITemplateService _templateService;

    public HtmlTemplatedImage(ILoggerFactory loggerFactory, ITemplateService templateService)
    {
        _loggerFactory = loggerFactory;
        _templateService = templateService;
    }

    public async Task<MemoryStream> GenerateImage(string template, dynamic data, int width)
    {
        var result = await _templateService.Replace(template, data);
        var converter = new HtmlConverter();
        return new MemoryStream(converter.FromHtmlString(result, width * 10, ImageFormat.Png, 100));
    }
}