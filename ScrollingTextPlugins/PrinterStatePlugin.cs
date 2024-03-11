using HandlebarsDotNet;
using WLEDAnimated.Interfaces;
using static System.Net.Mime.MediaTypeNames;
using WLEDAnimated.Services;
using Microsoft.Extensions.Logging;
using WLEDAnimated.Printing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ScrollingTextPlugins;

public class PrinterStatePlugin : IScrollingTextPlugin
{
    private readonly ILogger<PrinterStatePlugin> _logger;

    public PrinterStatePlugin(ILogger<PrinterStatePlugin> logger)
    {
        _logger = logger;
    }

    public async Task<string> GetTextToDisplay(string payload = null, object state = null)
    {
        if (state is null) return null;
        var printer = state as PrusaLinkPrinter;
        _logger.LogInformation("Updating Printer State: {payload}...", payload);

        var replacer = new PrinterVariableReplacer();
        return replacer.Replace(payload, printer);
    }
}

public class PrinterVariableReplacer
{
    public string Replace(string text, PrusaLinkPrinter state)
    {
        if (state is null) return null;

        var t = Handlebars.Compile(text);
        var result = t(state.Instance);
        return result;
    }
}