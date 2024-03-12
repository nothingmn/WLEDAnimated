using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SolidCompany.Wrappers.Logging.Abstractions;
using SolidCompany.Wrappers.WkHtmlToImage;
using SolidCompany.Wrappers.WkHtmlToImage.Registration;
using WLEDAnimated.Interfaces;
using WLEDAnimated.Interfaces.Services;

namespace ImageGeneration;

public class BasicTemplatedImage : IBasicTemplatedImage
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ITemplateService _templateService;

    public BasicTemplatedImage(ILoggerFactory loggerFactory, ITemplateService templateService)
    {
        _loggerFactory = loggerFactory;
        _templateService = templateService;
    }

    public async Task<MemoryStream> GenerateImage(string template, dynamic data, int width)
    {
        var result = await _templateService.Replace(template, data);

        var htmlToImage = new HtmlToImage(new HtmlToImageOptions()
        {
            ExectuionDirectory = new CustomDirectory(System.Environment.CurrentDirectory)
        }, _loggerFactory);

        using (var stm = await htmlToImage.CreateImageAsync(result, width, ImageFormat.Png))
        {
            var ms = new MemoryStream();
            await stm.CopyToAsync(ms);
            return ms;
        }
    }
}