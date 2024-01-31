using HandlebarsDotNet;
using Microsoft.Extensions.Logging.Abstractions;
using SolidCompany.Wrappers.Logging.Abstractions;
using SolidCompany.Wrappers.WkHtmlToImage;
using SolidCompany.Wrappers.WkHtmlToImage.Registration;
using WLEDAnimated.Interfaces;

namespace ImageGeneration;

public class BasicTemplatedImage : IBasicTemplatedImage
{
    public async Task<MemoryStream> GenerateImage(string template, dynamic data, int width)
    {
        var t = Handlebars.Compile(template);

        var result = t(data);

        var htmlToImage = new HtmlToImage(new HtmlToImageOptions()
        {
            ExectuionDirectory = new CustomDirectory(System.Environment.CurrentDirectory),
            DependencyLogger = DependencyLogger.Empty
        }, new NullLoggerFactory());

        using (var stm = await htmlToImage.CreateImageAsync(result, width, ImageFormat.Png))
        {
            if (File.Exists("image.png")) File.Delete("image.png");
            var ms = new MemoryStream();
            await stm.CopyToAsync(ms);
            return ms;
        }
    }
}