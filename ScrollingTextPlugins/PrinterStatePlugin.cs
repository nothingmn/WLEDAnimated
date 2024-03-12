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
    private readonly PrinterVariableReplacer _replacer;

    public PrinterStatePlugin(ILogger<PrinterStatePlugin> logger, PrinterVariableReplacer replacer)
    {
        _logger = logger;
        _replacer = replacer;
    }

    public async Task<string> GetTextToDisplay(string payload = null, object state = null)
    {
        if (state is null) return null;
        var printer = state as PrusaLinkInstance;
        _logger.LogInformation("Updating Printer State: {payload}...", payload);

        return _replacer.Replace(payload, printer);
    }
}

public class PrinterVariableReplacer
{
    private readonly ILogger<PrinterVariableReplacer> _log;

    public PrinterVariableReplacer(ILogger<PrinterVariableReplacer> log)
    {
        _log = log;
    }

    public string Replace(string text, PrusaLinkInstance state)
    {
        if (state is null) return null;

        var t = Handlebars.Compile(text);
        var result = t(state);
        _log.LogInformation("Printer State Will change to: '{result}'", result);
        return result;
    }
}