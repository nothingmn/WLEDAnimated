using System.Security.Cryptography;
using AnimationCore.Interfaces;
using HandlebarsDotNet;
using Microsoft.Extensions.Logging;
using WLEDAnimated.Animation;
using WLEDAnimated.Interfaces.Services;

namespace HandlebarsTemplating;

public class HandleBarsTemplateService : ITemplateService
{
    private readonly ILogger<HandleBarsTemplateService> _logger;

    public HandleBarsTemplateService(ILogger<HandleBarsTemplateService> logger)
    {
        _logger = logger;
    }

    //"ScrollingTextPluginPayload": "16x16rig Printer State Animation\\Printer.tmpl",
    //"ScrollingTextPluginPayload": "https://somehost.com/3dPrinterTemplate.tmpl",

    public async Task<string> Replace(string text, object state)
    {
        if (state is null) return null;

        try
        {
            //we want to use the either a local path, a remote url or the template directly.
            if (text.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) || text.StartsWith("https://"))
            {
                Uri uri = null;
                if (Uri.TryCreate(text, UriKind.RelativeOrAbsolute, out uri))
                {
                    var hash = MD5.Create().ComputeHash(System.Text.Encoding.UTF32.GetBytes(text));
                    var parentFolder = System.IO.Path.Combine(Path.GetTempPath(), "WLEDAnimated.API\\Animations");
                    if (!System.IO.Directory.Exists(parentFolder))
                    {
                        System.IO.Directory.CreateDirectory(parentFolder);
                    }
                    var file = System.IO.Path.Combine(parentFolder, BitConverter.ToString(hash).Replace("-", "") + ".tmpl");
                    if (!System.IO.File.Exists(file))
                    {
                        var client = new HttpClient();
                        var tmpl = await client.GetStringAsync(text);
                        if (!string.IsNullOrWhiteSpace(tmpl))
                        {
                            System.IO.File.WriteAllText(file, tmpl);
                            text = tmpl;
                        }
                    }
                    else
                    {
                        text = System.IO.File.ReadAllText(file);
                    }
                };
            }
            else
            {
                var animationsFolder = System.IO.Path.Combine(AppContext.BaseDirectory, "Animations");
                var filePath = System.IO.Path.Combine(animationsFolder, text);
                if (System.IO.File.Exists(filePath))
                {
                    text = System.IO.File.ReadAllText(filePath);
                }
            }
            var t = Handlebars.Compile(text);
            var result = t(state);
            _logger.LogInformation("Template compiled and executed: '{result}'", result);
            return result;
        }
        catch (Exception e)
        {
            _logger.LogInformation("There was an issue with the templating service:\n{error}", e);
            return null;
        }
    }
}