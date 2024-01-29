using WLEDAnimated.Interfaces;
using static System.Net.Mime.MediaTypeNames;
using WLEDAnimated.Services;
using Microsoft.Extensions.Logging;

namespace ScrollingTextPlugins;

public class DateTimeScrollingTextPlugin : IScrollingTextPlugin
{
    private readonly ILogger<BoredScrollingTextPlugin> _logger;

    public DateTimeScrollingTextPlugin(ILogger<BoredScrollingTextPlugin> logger)
    {
        _logger = logger;
    }

    public async Task<string> GetTextToDisplay(string payload = null)
    {
        var format = payload;
        if (string.IsNullOrWhiteSpace(payload)) format = "dd/MM/yyyy HH:mm:ss";

        _logger.LogInformation("Getting crypto: {format}...", format);

        var final = DateTime.Now.ToString(format);
        _logger.LogInformation(final);
        return final;
    }
}