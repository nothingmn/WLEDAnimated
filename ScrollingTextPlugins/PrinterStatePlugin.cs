using System.Net;
using HandlebarsDotNet;
using WLEDAnimated.Interfaces;
using static System.Net.Mime.MediaTypeNames;
using WLEDAnimated.Services;
using Microsoft.Extensions.Logging;
using WLEDAnimated.Interfaces.Services;
using WLEDAnimated.Printing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ScrollingTextPlugins;

public class PrinterStatePlugin : IScrollingTextPlugin
{
    private readonly ILogger<PrinterStatePlugin> _logger;
    private readonly ITemplateService _templateService;

    public PrinterStatePlugin(ILogger<PrinterStatePlugin> logger, ITemplateService templateService)
    {
        _logger = logger;
        _templateService = templateService;
    }

    public async Task<string> GetTextToDisplay(string payload = null, object state = null)
    {
        if (state is null) return null;
        var printer = state as PrusaLinkInstance;
        _logger.LogInformation("Updating Printer State: {payload}...", payload);

        return await _templateService.Replace(payload, printer);
    }
}